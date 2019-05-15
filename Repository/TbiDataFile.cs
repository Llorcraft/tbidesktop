using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TbiDesktop.Models;
using System.Text.RegularExpressions;

namespace TbiDesktop.Repository
{
    public static class TbiDataFile
    {
        public static Dictionary<string, List<Project>> Data { get; set; } = new Dictionary<string, List<Project>>();
        public static void Initialize()
        {
            Directory.EnumerateFiles(Directory.GetCurrentDirectory(), "*.tbi").ToList().ForEach(f => LoadLocalFile(f));
        }
        public static List<Project> projects
        {
            get
            {
                return Data.SelectMany(d => d.Value).ToList();
            }
        }

        public static List<Project> CopyFile(string path)
        {
            File.Copy(path, Path.Combine(Directory.GetCurrentDirectory(), Path.GetFileName(path)));
            return LoadLocalFile(Path.GetFileName(path));
        }

        public static List<Project> LoadLocalFile(string path)
        {
            List<Project> _projects = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Project>>(File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), Path.GetFileName(path))));
            _projects.ForEach(p => p.SetProjectToComponents());
            Data.Add(Path.GetFileNameWithoutExtension(path), _projects);
            projects.AddRange(_projects);
            return projects;
        }

        public static bool Exists(string path)
        {
            return Data.Keys.Any(k => k == Path.GetFileNameWithoutExtension(path));
        }

        public static IEnumerable<ReportBase> ReportsOf(User user)
        {
            return projects.SelectMany(p => p.components.SelectMany(c => c.reports)).Where(r => r.user == user.Name);
        }

        public static IEnumerable<ReportBase> ReportsOf(Project project)
        {
            return projects.FirstOrDefault(p => p == project).components.SelectMany(c => c.reports);
        }

        public static IEnumerable<TbiComponent> Filter(
            IEnumerable<User> users,
            IEnumerable<Project> projects,
            IEnumerable<TbiComponent> components,
            IEnumerable<string> reportPathes)
        {
            var _projects = TbiDataFile.projects.Where(p => projects.Contains(p));
            var _components = _projects.SelectMany(p => p.components.Where(c => components.Contains(c)));

            var _result = Newtonsoft.Json.JsonConvert.DeserializeObject<List<TbiComponent>>(Newtonsoft.Json.JsonConvert.SerializeObject(_components));
            _result.ForEach(c =>
            {
                c.reports = c.reports.Where(r => users.Select(u => u.Name).Contains(r.user)
                && reportPathes.Any(h => new Regex(@h.Replace(@"\", "")).IsMatch(@r.path.Replace(@"\", "")))).ToList();
            });

            _result = _result.Where(r => r.reports.Any()).ToList();
            _result.ForEach(c => c.project = TbiDataFile.projects.First(p => p.components.Select(m => m.id).Contains(c.id)));

            return _result;
        }
    }
}

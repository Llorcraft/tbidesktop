using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace TbiDesktop.Models
{
    public class TbiComponent
    {
        public string id { get; set; }
        [JsonIgnore]
        public Project project { get; set; }
        public List<ReportBase> reports { get; set; } = new List<ReportBase>();
        public Fields fields { get; set; } = new Fields();
        public Marker[] markers { get; set; } = new Marker[] { };
        public DateTime date { get; set; } = DateTime.Now;
        public string validation { get; set; }
        public TbiComponent validationReport { get; set; }
        public List<string> users
        {
            get
            {
                return reports.Select(c => c.user).ToList();
            }
        }
        public bool is_hot
        {
            get
            {
                return (fields.surface_temp.GetValueOrDefault(0)) > 55;
            }
        }
        public Result result
        {
            get
            {
                var report = reports.FirstOrDefault(r => r.energy);
                return null != report ? report.result : null;
            }
        }

        public bool insulated
        {
            get
            {
                return reports.Any(r => !!r.insulated);
            }
        }
        public string name
        {
            get
            {
                return $"{fields.location}";
            }
        }
        public bool energy
        {
            get
            {
                return reports.Any(r => new Regex(@"(surface|pipe|valve|flange)", RegexOptions.IgnoreCase).IsMatch(r.path));
            }
        }

        public ReportBase[] reports_by_type(string type)
        {
            var result = reports.Where(r => new Regex("(" + type + ")", RegexOptions.IgnoreCase).IsMatch(r.path)).ToList();
            if (type.LastIndexOf("hot") != -1)
            {
                var r = reports.FirstOrDefault(x => !!x.energy && this.is_hot);
                if (null != r) result.Add(r);
            }
            return result.OrderBy(r => r.date).ToArray();
        }
    }
}
using System;
using System.Text.RegularExpressions;

namespace TbiDesktop.Models
{
    public class ReportBase
    {
        public string id { get; set; }
        public string user{ get; set; }
        public string name { get; set; }
        public string path { get; set; }
        public Result result { get; set; }
        public Project project { get; set; }
        public TbiComponent component { get; set; }
        public Picture[] pictures { get; set; } = new Picture[] { };
        public DateTime date { get; set; } = DateTime.Now;
        public bool insulated { get; set; }
        public string summary_id { get; set; }
        public string readonly_summary_id { get; set; }
        public string comment { get; set; }
        public bool is_validation
        {
            get
            {
                return null != component && null != component.validation;
            }
        }
        public string potential_measure { get; } = "kWh/a";
        public bool energy
        {
            get
            {
                return new Regex(@"(surface|pipe|valve|flange)", RegexOptions.IgnoreCase).IsMatch(path);
            }
        }

        public string money_measure { get; } = "€/a";
    }
}
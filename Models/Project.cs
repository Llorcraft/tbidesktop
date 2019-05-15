using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TbiDesktop.Models
{
    public class Project: ModelWithPicture
    {
        public string name { get; set; }
        public string desc { get; set; }
        public DateTime date { get; set; }
        public string user { get; set; }
        public Document[] documents{ get; set; }
        public TbiComponent[] components { get; set; }
        public double? price { get; set; }
        public double price_delta { get; set; } = 1;
        public People people { get; set; } = new People();
        public double? co2{ get; set; }
        public string currency { get; set; } = "€";
        public int currency_index { get; set; } = 1;
        public int co2_index { get; set; } = 0;

        public List<string> users
        {
            get
            {
                return components.SelectMany(c => c.users).ToList();
            }
        }

        public void SetProjectToComponents()
        {
            components.ToList().ForEach(c => c.project = this);
        }
    }
}

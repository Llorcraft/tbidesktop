using System;

namespace TbiDesktop.Models
{
    public class Marker
    {
        public string id { get; set; } = (new Random()).ToString().Substring(2);
        public double x { get; set; } = 0;
        public double y { get; set; } = 0;
        public double? temperature { get; set; }
        public bool hasValue
        {
            get { return temperature != null; }
        }

        public string position
        {
            get
            {
                return $"translate({ x}px,{y}px)";
            }
        }
        public string transform
        {
            get
            {
                var _temp = temperature.ToString().Length == 1 ? 150 : temperature.ToString().Length == 2 ? 120 : 90;
                return $"matrix(1 0 0 1 {_temp} 228)";
            }
        }
        public string color
        {
            get
            {
                return temperature < 0 ? "cold" : temperature < 100 ? "templade" : "warm";
            }
        }
    }
}
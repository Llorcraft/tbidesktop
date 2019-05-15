using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TbiDesktop.Interfaces;

namespace TbiDesktop.Models
{
    public class User: ISelectable
    {
        public string Name { get; set; }
        public string DeviceId { get; set; }
        public bool Selected { get ; set; }
    }
}

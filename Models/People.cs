using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TbiDesktop.Models
{
    public class People
    {
        public Contact leader { get; set; } =  new Contact();
        public Contact energy_manager { get; set; } = new Contact();
        public Contact maintenance_manager { get; set; } = new Contact();
        public Contact hse_manager { get; set; } = new Contact();

    }
}

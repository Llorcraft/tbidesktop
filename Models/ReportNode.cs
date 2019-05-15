using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TbiDesktop.Interfaces;

namespace TbiDesktop.Models
{
    public class ReportNode : TreeNode, ISelectable
    {
        public List<ReportNode> Children { get; set; } = new List<ReportNode>();
        public bool Selected { get => this.Checked; set => this.Checked = value; }

        public bool Contains(string name)
        {
            return Text.ToLowerInvariant().Contains(name.ToLowerInvariant())
                || Children.Any(c => c.Contains(name));
        }

        public void Clear()
        {
            this.Children.ForEach(c => c.Clear());
            this.Nodes.Clear();
        }

        public void SetChecked(bool value)
        {
            Checked = value;
            Children.ForEach(c => c.SetChecked(value));
        }
    }
}

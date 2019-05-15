using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using TbiDesktop.Enums;
using TbiDesktop.Interfaces;
using TbiDesktop.Models;
using TbiDesktop.Repository;

namespace TbiDesktop
{
    public partial class frmSearch : Form
    {
        protected frmMain MdiForm;
        private List<User> _users = new List<User>();
        private List<Project> _projects = new List<Project>();
        private List<TbiComponent> _components = new List<TbiComponent>();

        private static string _maintenance = "Maintenance\\Leakege|Maintenance\\Damaged|Maintenance\\Condensation|Maintenance\\Structural|Maintenance\\Mechanical|Maintenance\\Electrical";
        private static string _safety = "Safety\\Fire Protection|Safety\\Housekeeping|Safety\\Traffic|Safety\\Other";

        private List<ReportNode> _reports = new List<ReportNode>
        {
            new ReportNode {
                Text= "Energy",
                Children = new List<ReportNode>{
                    new ReportNode {
                        Text= "Un-Insulated Equipments",
                        Children = new List<ReportNode> {
                            new ReportNode{ Text= "Pipe", Name = "Insulation\\Un-Insulated Equipments\\Pipe" },
                            new ReportNode{ Text= "Flange", Name = "Insulation\\Un-Insulated Equipments\\Flange"  },
                            new ReportNode{ Text= "Valve", Name = "Insulation\\Un-Insulated Equipments\\Valve"  },
                            new ReportNode{ Text= "Surface", Name = "Insulation\\Un-Insulated Equipments\\Surface"  }
                        }
                    },
                    new ReportNode {
                        Text= "Insulated Equipments",
                        Children = new List<ReportNode> {
                            new ReportNode{ Text= "Pipe", Name = "Insulation\\Insulated Equipments\\Pipe"  },
                            new ReportNode{ Text= "Surface", Name = "Insulation\\Insulated Equipments\\Surface"  }
                        }
                    }
                }
            },
            new ReportNode {Text= "Maintenance", Name= _maintenance},
            new ReportNode {Text= "Safety", Name= _safety},
            new ReportNode {Text= "Others", Name="Custom"},
            new ReportNode {Text= "Energy & Maintenance", Name=_maintenance},
            new ReportNode {Text= "Energy & Safety", Name=_safety},
            new ReportNode {Text= "Energy & Others", Name="Custom"}
        };
        public frmSearch(frmMain parent) 
        {
            InitializeComponent();
            MdiForm = parent;
            //prepareComponentsToSearch();
            (new Timer() { Enabled = true, Interval = 100 }).Tick += (object sender, EventArgs e) =>
            {
                new[] { lvwProjects, lvwComponents, lvwUsers }.ToList().ForEach(fillColumnSpace);
                (sender as Timer).Enabled = false;
            };
        }


        public void prepareComponentsToSearch()
        {
            _users = TbiDataFile.projects.SelectMany(p => p.users).Distinct().Select(u => new User { Name = u }).ToList();
            _projects.AddRange(TbiDataFile.projects);
            _components.AddRange(TbiDataFile.projects.SelectMany(p => p.components.Where(c => string.IsNullOrEmpty(c.validation))));

            fillUsers();
            fillProjects();
            fillComponents();
            fillReports();

            lnkFieldsSelectAll_LinkClicked(null, null);
        }

        private void fillUsers()
        {
            lvwUsers.Items.Clear();
            _users.Where(u => string.IsNullOrEmpty(txtUsers.Text) || u.Name.ToLowerInvariant().Contains(txtUsers.Text.ToLowerInvariant()))
                  .ToList()
                  .ForEach(u =>
                  {
                      var item = new ListViewItem { Text = u.Name, Tag = u, ImageIndex = 0};
                      item.SubItems.Add(TbiDataFile.ReportsOf(u).Count().ToString());
                      lvwUsers.Items.Add(item);
                  });
            lnkUsersSelectAll_LinkClicked(null, null);
        }

        private void fillProjects()
        {
            lvwProjects.Items.Clear();

            _projects.Where(u => string.IsNullOrEmpty(txtProjects.Text) || u.name.ToLowerInvariant().Contains(txtProjects.Text.ToLowerInvariant()))
                    .ToList()
                    .ForEach(p =>
                    {
                        var item = new ListViewItem { Text = p.name, Tag = p, ImageIndex = 1};
                        item.SubItems.Add(p.components.Count().ToString());
                        item.SubItems.Add(TbiDataFile.ReportsOf(p).Count().ToString());
                        lvwProjects.Items.Add(item);
                    });
            lnkProyectSelectAll_LinkClicked(null, null);
        }

        private void fillComponents()
        {
            lvwComponents.Items.Clear();
            _components.Where(u => string.IsNullOrEmpty(txtComponents.Text) || u.name.ToLowerInvariant().Contains(txtComponents.Text.ToLowerInvariant()))
                       .ToList()
                       .ForEach(c =>
                       {
                           var item = new ListViewItem { Text = c.name, Tag = c, ImageIndex = 2};
                           item.SubItems.Add(c.reports.Count().ToString());
                           lvwComponents.Items.Add(item);
                       });
            lnkComponentSelectAll_LinkClicked(null, null);
            //lstComponents.Items.Clear();
            //lstComponents.DisplayMember = "name";
            //lstComponents.Items.AddRange(_components.Where(u => string.IsNullOrEmpty(txtComponents.Text)
            //                                    || u.name.ToLowerInvariant().Contains(txtComponents.Text.ToLowerInvariant())
            //                        ).ToArray());
        }

        private void fillReports()
        {
            trvReports.Nodes.Cast<ReportNode>().ToList().ForEach(r => r.Clear());
            trvReports.Nodes.Clear();
            this._reports.Where(r => string.IsNullOrEmpty(txtReports.Text) || r.Contains(txtReports.Text))
                        .ToList()
                        .ForEach(r => addToReportTreeView(r, null));
            trvReports.ExpandAll();

            lnkReportsSelectAll_LinkClicked(null, null);
        }

        private void addToReportTreeView(ReportNode node, ReportNode parent)
        {
            if (null == parent)
                trvReports.Nodes.Add(node);
            else
                parent.Nodes.Add(node);

            node.Children.Where(c => c.Contains(txtReports.Text))
                .ToList()
                .ForEach(n => addToReportTreeView(n, node));
        }

        private void frmSearch_Load(object sender, EventArgs e)
        {
            resizeSplitContainers();
            fillReports();
        }

        private void resizeSplitContainers()
        {
            splMain.SplitterDistance = splRightVertical.SplitterDistance = Width / 3;
            splRightHorizontal.SplitterDistance = Convert.ToInt16(Math.Ceiling(Convert.ToDouble((Height - pnlFooter.Height - 5) / 1.4)));
            splHorizontalLeft.SplitterDistance = (Height - pnlFooter.Height - 5) / 2;
            splMain.Panel1.Padding = new Padding(5, 0, 0, 0);
            lstFields.ColumnWidth = lstFields.Width / 2;
        }

        private void txtUsers_TextChanged(object sender, EventArgs e)
        {
            fillUsers();
        }

        private void txtProjects_TextChanged(object sender, EventArgs e)
        {
            fillProjects();
        }

        private void txtReports_TextChanged(object sender, EventArgs e)
        {
            fillReports();
        }

        #region Clear
        private void btnClearUsers_Click(object sender, EventArgs e)
        {
            txtUsers.Clear();
        }

        private void btnClearProjects_Click(object sender, EventArgs e)
        {
            txtProjects.Clear();
        }

        private void btnClearComponents_Click(object sender, EventArgs e)
        {
            txtComponents.Clear();
        }

        private void btnClearReports_Click(object sender, EventArgs e)
        {
            txtReports.Clear();
        }
        #endregion

        private void modifySelection(CheckedListBox list, SelectionTypes type)
        {
            int[] selectedArray = new int[list.SelectedIndices.Count];
            list.SelectedIndices.CopyTo(selectedArray, 0);
            list.SelectedIndices.Clear();

            for (var i = 0; i < list.Items.Count; i++)
            {
                if (type == SelectionTypes.All)
                {
                    list.SelectedIndices.Add(i);
                }
                else if (type == SelectionTypes.Inverse)
                {
                    if (!selectedArray.Contains(i)) list.SelectedIndices.Add(i);
                }
                list.SetItemChecked(i, list.SelectedIndices.Contains(i));
            }
        }

        private void modifySelection(ListView list, SelectionTypes type)
        {
            list.Items.OfType<ListViewItem>().ToList().ForEach(item =>
            {
                item.Checked = type == SelectionTypes.All ? true : type == SelectionTypes.None ? false : !item.Checked;
            });
        }

        private void lnkUsersSelectAll_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            modifySelection(lvwUsers, SelectionTypes.All);
        }

        private void lnkUsersSelectNone_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            modifySelection(lvwUsers, SelectionTypes.None);
        }

        private void lnkProyectSelectAll_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            modifySelection(lvwProjects, SelectionTypes.All);
        }

        private void lnkProyectSelectNone_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            modifySelection(lvwProjects, SelectionTypes.None);
        }

        private void lnkComponentSelectAll_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            modifySelection(lvwComponents, SelectionTypes.All);
        }

        private void lnkComponentSelectNone_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            modifySelection(lvwComponents, SelectionTypes.None);
        }

        private void txtComponents_TextChanged(object sender, EventArgs e)
        {
            fillComponents();
        }

        private void lnkFieldsSelectAll_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            modifySelection(lstFields, SelectionTypes.All);
        }

        private void lnkFieldsSelectNone_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            modifySelection(lstFields, SelectionTypes.None);
        }

        private void lnkReportsSelectAll_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            trvReports.Nodes.Cast<ReportNode>().ToList().ForEach(r => r.SetChecked(true));
        }

        private void lnkReportsSelectNone_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            trvReports.Nodes.Cast<ReportNode>().ToList().ForEach(r => r.SetChecked(false));
        }

        private void trvReports_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (e.Action != TreeViewAction.Unknown)
                (e.Node as ReportNode).Children.ForEach(c => c.SetChecked(e.Node.Checked));
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            txtComponents.Text =
            txtProjects.Text =
            txtReports.Text =
            txtUsers.Text = string.Empty;

            lnkComponentSelectNone_LinkClicked(lnkComponentSelectNone, null);
            lnkProyectSelectNone_LinkClicked(lnkProyectSelectNone, null);
            lnkReportsSelectNone_LinkClicked(lnkReportsSelectNone, null);
            lnkUsersSelectNone_LinkClicked(lnkUsersSelectNone, null);
            lnkFieldsSelectNone_LinkClicked(lnkFieldsSelectNone, null);
        }

        private void fillColumnSpace(ListView list)
        {
            var index = int.Parse(list.Tag.ToString());
            var sum = list.Columns.Cast<ColumnHeader>().Where(c => c.Index != index).Sum(c => c.Width);
            list.Columns[index].Width = list.Width - sum;
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            var x = filter();
        }

        private DialogResult filter()
        {
            if (lvwUsers.CheckedItems.Count == 0)
                return MessageBox.Show("You have not selected any user.\nSelect at least one and try again.", "Search", MessageBoxButtons.OK, MessageBoxIcon.Error);
            if (lvwProjects.CheckedItems.Count == 0)
                return MessageBox.Show("You have not selected any project.\nSelect at least one and try again.", "Search", MessageBoxButtons.OK, MessageBoxIcon.Error);
            if (lvwComponents.CheckedItems.Count == 0)
                return MessageBox.Show("You have not selected any component.\nSelect at least one and try again.", "Search", MessageBoxButtons.OK, MessageBoxIcon.Error);
            var pathes = getCheckedReportPathes(trvReports.Nodes, new List<string>());
            if (!pathes.Any())
                return MessageBox.Show("You have not selected any report.\nSelect at least one and try again.", "Search", MessageBoxButtons.OK, MessageBoxIcon.Error);

            var result = TbiDataFile.Filter(
                users: lvwUsers.CheckedItems.Cast<ListViewItem>().Select(i => i.Tag as User),
                projects: lvwProjects.CheckedItems.Cast<ListViewItem>().Select(i => i.Tag as Project),
                components: lvwComponents.CheckedItems.Cast<ListViewItem>().Select(i => i.Tag as TbiComponent),
                reportPathes: pathes
                );

            var fields = lstFields.Items.Cast<string>()
                    .Select((text, index) => new { text, index, selected = lstFields.CheckedItems.Contains(lstFields.Items[index]) })
                    .Where(i => i.selected)
                    .Select(i => i.index);

            MdiForm.DockForm(new frmSummary(MdiForm, result, fields));
            return DialogResult.OK;
        }

        private List<string> getCheckedReportPathes(TreeNodeCollection nodes, List<string> list)
        {
            list.AddRange(nodes.Cast<TreeNode>().Where(n => n.Checked && n.Nodes.Count == 0).Select(n => n.Name));
            foreach (TreeNode node in nodes)
            {
                getCheckedReportPathes(node.Nodes, list);
            }
            return list;
        }

    }
}

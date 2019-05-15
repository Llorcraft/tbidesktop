using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using TbiDesktop.Repository;

namespace TbiDesktop
{
    public partial class frmMain : Form
    {
        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_CLIENTEDGE = 0x200;

        private frmSearch _frmSearch;
        public frmMain()
        {
            InitializeComponent();
            createToolTips();

            _frmSearch = DockForm(new frmSearch(this)) as frmSearch;
            TbiDataFile.Initialize();
            _frmSearch.prepareComponentsToSearch();
        }

        private void createToolTips()
        {
            var tips = new Dictionary<Control, string>{
                {btnLoadData, "Load TBI data file" },
                {btnSettings, "Show configuration options form" },
                {btnMinimize, "Minimize" },
                {btnClose, "Exit" }
            };
            tips.ToList()
                .ForEach(tip => toolTip.SetToolTip(tip.Key, tip.Value));
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Do you want to exit?", "TBI", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                Application.Exit();
        }

        private void btnMinimize_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void btnLoadData_Click(object sender, EventArgs e)
        {
            if (dlgOpenFile.ShowDialog() == DialogResult.OK) importData(dlgOpenFile.FileName);
        }

        private void importData(string fileName)
        {
            if (!(new Regex(".tbi", RegexOptions.IgnoreCase).IsMatch(new System.IO.FileInfo(fileName).Extension)))
            {
                if (MessageBox.Show("Selected file is not a TBI Data file.\nDo you want try again?", "TBI", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.Yes)
                    btnLoadData_Click(null, null);
            }
            else
            {
                if (!TbiDataFile.Exists(fileName)) {
                    TbiDataFile.CopyFile(fileName);
                    _frmSearch.prepareComponentsToSearch();
                }
            }
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            DockForm(new frmSettings());
        }

        public Form DockForm(Form form)
        {
            form.MdiParent = this;
            form.FormBorderStyle = FormBorderStyle.None;
            form.Dock = DockStyle.Fill;
            form.Show();
            return form;
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            //foreach (Control ctrl in this.Controls)
            //{
            //    if (ctrl is MdiClient)
            //    {
            //        ctrl.BackColor = Color.White;
            //        ctrl.st
            //    }
            //}
            customizeLayout(sender, e);
        }

        private void customizeLayout(object sender, EventArgs e)
        {
            foreach (var mdi in this.Controls.OfType<MdiClient>())
            {
                mdi.BackColor = Color.White;
                mdi.Dock = DockStyle.None;
                int window = GetWindowLong(mdi.Handle, GWL_EXSTYLE);
                window &= ~WS_EX_CLIENTEDGE;
                SetWindowLong(mdi.Handle, GWL_EXSTYLE, window);
                mdi.Dock = DockStyle.Fill;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TbiDesktop.Services;
using Excel = Microsoft.Office.Interop.Excel;

namespace TbiDesktop
{
    public partial class frmSummary : Form
    {
        IEnumerable<Models.TbiComponent> _components;

        public frmSummary(frmMain parent, IEnumerable<Models.TbiComponent> components, IEnumerable<int> fields)
        {
            InitializeComponent();
            _components = components;
            var asHtml = true;
            webBrowser1.Navigate(Path.Combine(Directory.GetCurrentDirectory(), createSummary(asHtml, fields)), false);

            (new Timer { Enabled = !asHtml, Interval = 2000 }).Tick += ((object sender, EventArgs e) =>
            {
                Application.Exit();
            });
            
        }

        void removeFilesFrom(string directory)
        {
            Directory.GetDirectories(directory).ToList().ForEach(removeFilesFrom);
            var files = Directory.GetFiles(directory).ToList();
            files.ForEach(File.Delete);
            if(directory.Split('\\').Last() != "temp") Directory.Delete(directory, true);
        }

        private string createSummary(bool asHtml, IEnumerable<int> fields)
        {
            Excel.Application xlApp = new Excel.Application();
            Excel.Workbook xlWorkBook = xlApp.Workbooks.Open(Path.Combine(Directory.GetCurrentDirectory(), "Tabla.xlsx"));
            Excel.Worksheet xlWorksheet = xlWorkBook.Worksheets["Summary"];

            var tempFolder = Path.Combine(Directory.GetCurrentDirectory(), "temp");
            if (!Directory.Exists(tempFolder))
            {
                Directory.CreateDirectory(tempFolder);
            }
            else
            {
                removeFilesFrom(tempFolder);
            }
            string  htmlFile = asHtml ?  Path.Combine(tempFolder, $"{Guid.NewGuid()}.html") : Path.Combine(tempFolder, $"{Guid.NewGuid()}.xlsx");
            xlWorkBook.Saved = true;

            ExcelServices.PrepareSummary(_components, xlWorksheet, fields);

            xlWorkBook.SaveAs(htmlFile, asHtml ? Excel.XlFileFormat.xlHtml : Excel.XlFileFormat.xlOpenXMLWorkbook);
            
            xlWorkBook.Saved = true;
            xlWorkBook.Close();
            xlApp.Quit();

            release(xlApp);
            release(xlWorkBook);
            release(xlWorksheet);

            return htmlFile;
        }

        private void release(Object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
            }
            catch
            {
                obj = null;
            }
            finally
            {
                GC.Collect();
            }
        }
        private void frmSummary_Load(object sender, EventArgs e)
        {
            var x = _components;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}

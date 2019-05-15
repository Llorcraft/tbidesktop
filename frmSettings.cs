using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TbiDesktop.Extensions;

namespace TbiDesktop
{
    public partial class frmSettings : Form
    {
        public frmSettings()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmSettings_Load(object sender, EventArgs e)
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en");

            var svgPath = @"C:\Users\Pc\Documents\TBI Chart.svg";
            HtmlAgilityPack.HtmlDocument svg= new HtmlAgilityPack.HtmlDocument();
            svg.Load(svgPath);
            var text = svg.DocumentNode.Descendants("text");
            //var rect = svg.DocumentNode.Descendants("rect");

            var max = 66.2;
            var basic = 24.2;
            var good = 45.2;

            double height = svg.GetElementbyId("bar_current").GetAttributeValue("height", 0);
            double top = svg.GetElementbyId("bar_current").GetAttributeValue("y", 0);

            text.ElementAt(6).InnerHtml = text.ElementAt(0).InnerHtml = $"{max}".ToNumber();
            text.ElementAt(5).InnerHtml = text.ElementAt(1).InnerHtml = $"{max/2}".ToNumber();

            svg.GetElementbyId("bar_basic_as").SetAttributeValue("height", $"{basic * height / max}".ToNumber());
            svg.GetElementbyId("bar_basic_as").SetAttributeValue("y", $"{(top + height) - (basic * height / max)}".ToNumber());

            svg.GetElementbyId("bar_good_as").SetAttributeValue("height", $"{good * height / max}".ToNumber());
            svg.GetElementbyId("bar_good_as").SetAttributeValue("y", $"{(top + height) - (good * height / max)}".ToNumber());

            var savePath = $@"C:\Users\Pc\Documents\temp\{Guid.NewGuid()}.svg";
            svg.Save(savePath);
            webBrowser1.Navigate(svgPath);
        }
    }
}

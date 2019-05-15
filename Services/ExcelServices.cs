using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TbiDesktop.Models;
using Excel = Microsoft.Office.Interop.Excel;

namespace TbiDesktop.Services
{
    public static class ExcelServices
    {
        public static void PrepareSummary(IEnumerable<Models.TbiComponent> components, Excel.Worksheet xlWorksheet, IEnumerable<int> fields)
        {
            createWhiteBorders(xlWorksheet);
            createBlueColumns(xlWorksheet, components.Count());
            xlWorksheet.Cells.Range[$"A3:T{components.Count() * 3 + 2}"].RowHeight = 16d;

            components.Select((c, i) => new { c, i }).ToList().ForEach(e =>
            {
                var index = (e.i + 1) * 3;
                Excel.Border border;

                xlWorksheet.Cells.Range[$"A{index}"].Value = e.c.name;

                border = xlWorksheet.Cells.Range[$"A{index + 2}:T{index + 2}"].Borders[Excel.XlBordersIndex.xlEdgeBottom];
                border.LineStyle = Excel.XlLineStyle.xlContinuous;
                border.Weight = 2d;
                border.Color = Color.Black;

                (new[] { "A", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T" })
                    .ToList()
                    .ForEach(col =>
                    {
                        xlWorksheet.Cells.Range[$"{col}{index}:{col}{index + 2}"].Merge();
                        switch (col)
                        {
                            case "A":
                                xlWorksheet.Cells.Range[$"{col}{index}"].HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
                                break;
                            case "H":
                            case "I":
                            case "J":
                            case "K":
                            case "L":
                                xlWorksheet.Cells.Range[$"{col}{index}"].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                                break;
                            default:
                                xlWorksheet.Cells.Range[$"{col}{index}"].HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
                                break;
                        }
                    });

                createEnergyColumns(xlWorksheet, e.c, index);
                createSafetyColumns(xlWorksheet, e.c, index);
            });

            hideFields(xlWorksheet, fields);
        }

        private static void hideFields(Excel.Worksheet xlWorksheet, IEnumerable<int> fields)
        {
            Enumerable.Range(0, 8)
                    .Where(c => !fields.Contains(c))
                    .Select(c => (char)(77/*H*/+ c))
                    .OrderByDescending(c => c)
                    .ToList().ForEach(c =>xlWorksheet.Range[$"{c}:{c}"].Delete());
        }

        private static void createSafetyColumns(Excel.Worksheet xlWorksheet, TbiComponent c, int index)
        {
            if (c.energy && c.is_hot) xlWorksheet.Range[$"H{index}"].Value = "Hot surface";
            c.reports.Where(r => !r.energy)
                    .ToList()
                    .ForEach(r =>
                    {
                        var spacer = string.IsNullOrEmpty(xlWorksheet.Range[$"H{index}"].Value) ? "" : "\n";
                        xlWorksheet.Range[$"H{index}"].Value += $"{spacer}{r.name}";
                    });
        }

        private static void createEnergyColumns(Excel.Worksheet xlWorksheet, TbiComponent component, int index)
        {
            if (!component.energy)
            {
                xlWorksheet.Range[$"B{index}:G{index + 2}"].Merge();
                xlWorksheet.Range[$"B{index}"].Value = "Not considered >";
                xlWorksheet.Range[$"B{index}"].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            }
            else
            {
                var sufix = component.fields.unknow_surface ? "/m2" : "";
                xlWorksheet.Range[$"B{index}"].Value = component.result.headLost.power / 100;
                xlWorksheet.Range[$"B{index + 1}"].Value = component.result.headLost.money;
                xlWorksheet.Range[$"B{index + 1}"].NumberFormat = "0";
                xlWorksheet.Range[$"B{index + 2}"].Value = component.result.co2[0];

                //xlWorksheet.Range[$"C{index}"].Value = $"MWh{sufix}";
                //xlWorksheet.Range[$"C{index + 1}"].Value = $"{component.project.currency}{sufix}";
                //xlWorksheet.Range[$"C{index + 2}"].Value = $"tn CO2{sufix}";

                xlWorksheet.Range[$"D{index}"].Value = component.result.savingPotentialMin.power / 100;
                xlWorksheet.Range[$"D{index + 1}"].Value = component.result.savingPotentialMin.money;
                xlWorksheet.Range[$"D{index + 1}"].NumberFormat = "0";
                xlWorksheet.Range[$"D{index + 2}"].Value = component.result.co2[1];

                xlWorksheet.Range[$"E{index}"].Value = xlWorksheet.Range[$"E{index + 1}"].Value = xlWorksheet.Range[$"E{index + 2}"].Value = "-";

                xlWorksheet.Range[$"F{index}"].Value = component.result.savingPotentialMax.power / 100;
                xlWorksheet.Range[$"F{index + 1}"].Value = component.result.savingPotentialMax.money;
                xlWorksheet.Range[$"F{index + 1}"].NumberFormat = "0";
                xlWorksheet.Range[$"F{index + 2}"].Value = component.result.co2[2];

                xlWorksheet.Range[$"G{index}"].Value = $"MWh{sufix}";
                xlWorksheet.Range[$"G{index + 1}"].Value = $"{component.project.currency}{sufix}";
                xlWorksheet.Range[$"G{index + 2}"].Value = $"tn CO2{sufix}";

                xlWorksheet.Range[$"B{index}:G{index}"].Interior.Color = xlWorksheet.Range[$"B{index + 2}:G{index + 2}"].Interior.Color = Color.FromArgb(242, 242, 242);
                xlWorksheet.Range[$"B{index}:G{index + 2}"].Font.Color = component.fields.unknow_surface ? Color.Gray : Color.Black;
            }
        }

        private static void createBlueColumns(Excel.Worksheet xlWorksheet, int rows)
        {
            xlWorksheet.Cells.Range[$"H3:H{rows * 3 + 2}"].Interior.Color = Color.FromArgb(180, 198, 231);
            xlWorksheet.Cells.Range[$"I3:I{rows * 3 + 2}"].Interior.Color = Color.FromArgb(217, 225, 242);
            xlWorksheet.Cells.Range[$"J3:J{rows * 3 + 2}"].Interior.Color = Color.FromArgb(231, 235, 247);
        }

        private static void createWhiteBorders(Excel.Worksheet xlWorksheet)
        {
            Excel.Border border;
            new[] { "H", "I", "J", "K", "L", "M" }.ToList().ForEach(c =>
            {
                border = xlWorksheet.Cells.Range[$"{c}:{c}"].Borders[Excel.XlBordersIndex.xlEdgeLeft];
                border.LineStyle = Excel.XlLineStyle.xlDouble;
                border.Weight = 4d;
                border.Color = Color.White;
            });
        }
    }
}

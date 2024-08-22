using Autodesk.Revit.DB;
using CableTraySection.Model;
using Microsoft.Win32;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CableTraySection
{
    public class Utils
    {
        public static void DrawTableGrid(Document doc, ViewDrafting view, XYZ startPoint, int numRows, int numCols, double cellWidth, double cellHeight)
        {
            using (Transaction t = new Transaction(doc, "Draw Table Grid"))
            {
                t.Start();

                // Draw horizontal lines
                for (int i = 0; i <= numRows; i++)
                {
                    XYZ start = startPoint + new XYZ(0, -i * cellHeight, 0);
                    XYZ end = start + new XYZ(numCols * cellWidth, 0, 0);
                    Line line = Line.CreateBound(start, end);
                    doc.Create.NewDetailCurve(view, line);
                }

                // Draw vertical lines
                for (int j = 0; j <= numCols; j++)
                {
                    XYZ start = startPoint + new XYZ(j * cellWidth, 0, 0);
                    XYZ end = start + new XYZ(0, -numRows * cellHeight, 0);
                    Line line = Line.CreateBound(start, end);
                    doc.Create.NewDetailCurve(view, line);
                }

                t.Commit();
            }
        }
        public void AddTextToTable(Document doc, ViewDrafting view, XYZ startPoint, int numRows, int numCols, double cellWidth, double cellHeight)
        {
            using (Transaction t = new Transaction(doc, "Add Text to Table"))
            {
                t.Start();

                for (int i = 0; i < numRows; i++)
                {
                    for (int j = 0; j < numCols; j++)
                    {
                        XYZ textPosition = startPoint + new XYZ(j * cellWidth + cellWidth / 2, -i * cellHeight - cellHeight / 2, 0);
                        TextNoteOptions opts = new TextNoteOptions
                        {
                            HorizontalAlignment = HorizontalTextAlignment.Center,
                            VerticalAlignment = VerticalTextAlignment.Middle
                        };
                        TextNote.Create(doc, view.Id, textPosition, $"R{i + 1}C{j + 1}", opts);
                    }
                }

                t.Commit();
            }
        }


        public static void Excel(string core, string conductor, string type)
        {
            DataHelper.CSOD = new List<Dictionary<string, string>>();
            if (DataHelper.FilePath == null)
            {
                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    Filter = "Excel Files|*.xls;*.xlsx;*.xlsm"
                };
                openFileDialog.ShowDialog();
                DataHelper.FilePath = openFileDialog.FileName;

            }
            FileInfo fi = new FileInfo(DataHelper.FilePath);
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (ExcelPackage excelPackage = new ExcelPackage(fi))
            {
                //Get a WorkSheet by index. Note that EPPlus indexes are base 1, not base 0!
                ExcelWorksheet workSheet = excelPackage.Workbook.Worksheets.Where(x => x.Name == $"{core}_{conductor}").FirstOrDefault();
                ViewModel.ViewModel.CableSizes.Clear();

                for (int col = 1; col <= workSheet.Dimension.End.Column; col++)
                {
                    var celValue = workSheet.Cells[1, col].Text;
                    if (celValue == $"{core}_{conductor}/{type}")
                    {

                        for (int row = 2; row < workSheet.Dimension.End.Row; row++)
                        {
                            if (workSheet.Cells[row, col].Text != null && workSheet.Cells[row, col].Text != "")
                            {
                                ViewModel.ViewModel.CableSizes.Add(workSheet.Cells[row, col].Text);
                                DataHelper.CSOD.Add(new Dictionary<string, string> { { workSheet.Cells[row, col].Text, workSheet.Cells[row, col + 1].Text } });

                            }


                        }
                    }
                }
                if (ViewModel.ViewModel.EarthingCS.Count == 0)
                {

                    DataHelper.EOD = new List<Dictionary<string, string>>();
                    ViewModel.ViewModel.EarthingCS.Clear();
                    ViewModel.ViewModel.EarthingCS.Add("0");
                    DataHelper.EOD.Add(new Dictionary<string, string> { { "0", "0" } });
                    workSheet = excelPackage.Workbook.Worksheets.Where(x => x.Name == "1E_CU").FirstOrDefault();
                    for (int index = 2; index <= workSheet.Dimension.End.Row; index++)
                    {
                        if (workSheet.Cells[index, 1].Text != null && workSheet.Cells[index, 1].Text != "")
                        {
                            ViewModel.ViewModel.EarthingCS.Add(workSheet.Cells[index, 1].Text);
                            DataHelper.EOD.Add(new Dictionary<string, string> { { workSheet.Cells[index, 1].Text, workSheet.Cells[index, 2].Text } });
                        }
                    }


                }
            }

        }

        public static double SizeCableTray(List<double> cableDiameters, double spare, double first, double between)
        {
            double maxDiameter = cableDiameters[0];
            double sumDiameters = maxDiameter;

            double total = first * maxDiameter;

            for (int i = 1; i < cableDiameters.Count; i++)
            {
                maxDiameter = Math.Max(cableDiameters[i - 1], cableDiameters[i]);
                total += between * maxDiameter;
                sumDiameters += cableDiameters[i];
            }

            total += sumDiameters;
            // Lowest Standard is 100
            if(total < 100)
            {
                total = 100;
            }

            //round to Standars

            if (spare == 0)
            {
                return Math.Ceiling(total / 50) * 50;
            }
            else
            {
                return Math.Ceiling(spare * total / 50) * 50; 
           
            }

        }

        public static double Convert_to_Feet(double value)
        {
            return UnitUtils.ConvertToInternalUnits(value, UnitTypeId.Millimeters);
        }

        public static double Convert_to_mm(double value)
        {
            return UnitUtils.ConvertFromInternalUnits(value, UnitTypeId.Millimeters);
        }


    }
}

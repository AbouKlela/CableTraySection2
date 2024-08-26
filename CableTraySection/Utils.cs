using Autodesk.Revit.DB;
using CableTraySection.Model;
using Microsoft.Win32;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static Autodesk.Revit.DB.SpecTypeId;

namespace CableTraySection
{
    public static class  Utils
    {

        public static void DrawTableGrid(Document doc, ViewDrafting view, XYZ startPoint, int numRows)
        {
            // Define fixed column widths
            double[] columnWidths = { Utils.Convert_to_Feet(75), Utils.Convert_to_Feet(150), Utils.Convert_to_Feet(270), Utils.Convert_to_Feet(100), Utils.Convert_to_Feet(100) };
            double cellHeight = Utils.Convert_to_Feet(60.0); // Fixed cell height

            using (Transaction t = new Transaction(doc, "Draw Table Grid"))
            {
                t.Start();

                // Calculate the total width of the table by summing up the column widths
                double totalWidth = columnWidths.Sum();

                // Draw horizontal lines
                for (int i = 0; i <= numRows; i++)
                {
                    // The Y position decreases as we move down rows
                    XYZ start = startPoint + new XYZ(0, -i * cellHeight, 0);
                    XYZ end = start + new XYZ(totalWidth, 0, 0); // Draw across the total width of the table
                    Line line = Line.CreateBound(start, end);
                    doc.Create.NewDetailCurve(view, line);
                }

                // Draw vertical lines
                double currentX = 0.0; // Track the X position as we move across columns
                for (int j = 0; j <= columnWidths.Length; j++)
                {
                    // For each column, we draw from top to bottom
                    XYZ start = startPoint + new XYZ(currentX, 0, 0);
                    XYZ end = start + new XYZ(0, -numRows * cellHeight, 0); // Draw down the total height of the table
                    Line line = Line.CreateBound(start, end);
                    doc.Create.NewDetailCurve(view, line);

                    // Move to the next X position based on the column width (unless it's the last line)
                    if (j < columnWidths.Length)
                    {
                        currentX += columnWidths[j];
                    }
                }

                t.Commit();
            }
        }

        public static void AddTextToTable(Document doc, ViewDrafting view, XYZ startPoint, int numRows, int numCols, double cellWidth, double cellHeight)
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
            if (total < 100)
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
        public static double CTF(this double value)
        {
            return UnitUtils.ConvertToInternalUnits(value, UnitTypeId.Millimeters);
        }


        public static double Convert_to_mm(double value)
        {
            return UnitUtils.ConvertFromInternalUnits(value, UnitTypeId.Millimeters);
        }


        public static string GetSecondAndThirdChanarcters(string input)
        {
            int startIndex = input.IndexOf('(');
            int endIndex = input.IndexOf('C');

            // If both '(' and 'C' are found, extract the part between them
            if (startIndex != -1 && endIndex != -1 && endIndex > startIndex)
            {
                return input.Substring(startIndex + 1, endIndex - startIndex); // Extract between '(' and 'C'
            }
            return input; // Return the input as is if '(' or 'C' are not found
        }

        public static double CalculateAreaFromDiameter(double diameter)
        {
            return Math.PI * Math.Pow(diameter / 2, 2);
        }





    }
}

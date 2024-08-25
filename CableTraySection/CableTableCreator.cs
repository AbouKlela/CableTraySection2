using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;
using Autodesk.Revit.UI;
using CableTraySection.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Document = Autodesk.Revit.DB.Document;

namespace CableTraySection
{
  

    public class KLELA
    {

        public static void DrawTable(Document doc, ViewDrafting view, XYZ startPoint, int numRows, List<CableData> cableDataList)
        {
            // Column widths as per the image
            double[] columnWidths = { Utils.Convert_to_Feet(75), Utils.Convert_to_Feet(150), Utils.Convert_to_Feet(270), Utils.Convert_to_Feet(100), Utils.Convert_to_Feet(100) };
            double cellHeight = Utils.Convert_to_Feet(60.0); // Row height
            int numCols = columnWidths.Length;

            // Header labels
            string[] headers = { "Cable No.", "Feeder Details", "Feeder", "Cable Diameter", "Earthing Diameter" };

            using (Transaction t = new Transaction(doc, "Draw Table"))
            {
                t.Start();

                // Calculate the total width of the table
                double totalWidth = columnWidths.Sum();

                // Draw horizontal lines
                for (int i = 0; i <= numRows; i++)
                {
                    XYZ start = startPoint + new XYZ(0, -i * cellHeight, 0);
                    XYZ end = start + new XYZ(totalWidth, 0, 0);
                    Line line = Line.CreateBound(start, end);
                    doc.Create.NewDetailCurve(view, line);
                }

                // Draw vertical lines
                double currentX = 0.0;
                for (int j = 0; j <= numCols; j++)
                {
                    XYZ start = startPoint + new XYZ(currentX, 0, 0);
                    XYZ end = start + new XYZ(0, -numRows * cellHeight, 0);
                    Line line = Line.CreateBound(start, end);
                    doc.Create.NewDetailCurve(view, line);

                    if (j < columnWidths.Length)
                    {
                        currentX += columnWidths[j];
                    }
                }

                // Write header text
                currentX = 0.0;
                XYZ headerY = startPoint + new XYZ(0, 0, 0); // Start at the top row for headers
                for (int j = 0; j < numCols; j++)
                {
                    XYZ textLocation = startPoint + new XYZ(currentX + columnWidths[j] / 2, -cellHeight / 2, 0);
                    WriteText(doc, view, textLocation, headers[j]);
                    currentX += columnWidths[j];
                }

                // Fill in data rows
                for (int i = 0; i < cableDataList.Count; i++)
                {
                    CableData data = cableDataList[i];
                    double rowY = -cellHeight * (i + 1); // Y position for this row
                    XYZ rowStart = startPoint + new XYZ(0, rowY, 0);
                    currentX = 0.0;

                    // Cable No.
                    WriteText(doc, view, rowStart + new XYZ(columnWidths[0]/2 , -cellHeight / 2, 0), (i + 1).ToString());

                    // Feeder Details
                    WriteText(doc, view, rowStart + new XYZ(currentX + columnWidths[1] , -cellHeight / 2, 0), data.DfromTo);
                    currentX += columnWidths[1];

                    // Feeder
                    WriteText(doc, view, rowStart + new XYZ(currentX + columnWidths[2]/1.25 , -cellHeight / 2, 0), data.DSelectedCable);
                    currentX += columnWidths[2];

                    // Cable Diameter
                    WriteText(doc, view, rowStart + new XYZ(currentX + columnWidths[3]*1.25 , -cellHeight / 2, 0), data.DOD);
                    currentX += columnWidths[3];

                    // Earthing Diameter
                    WriteText(doc, view, rowStart + new XYZ(currentX + columnWidths[4]*1.25, -cellHeight / 2, 0), data.DEOD);
                }

                t.Commit();


            }




        }
        public static void WriteText(Document doc, ViewDrafting view, XYZ location, string text)
        {
            TextNoteType textNoteType = new FilteredElementCollector(doc)
                .OfClass(typeof(TextNoteType))
                .Cast<TextNoteType>()
                .FirstOrDefault();

            if (textNoteType != null)
            {
                TextNote textNote = TextNote.Create(doc, view.Id, location, text, textNoteType.Id);
                textNote.HorizontalAlignment = HorizontalTextAlignment.Center;
                textNote.VerticalAlignment = VerticalTextAlignment.Middle;
            }
        }
    }

    // Helper method to write text at a specific location
    

}

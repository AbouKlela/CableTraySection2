using Autodesk.Revit.Creation;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Document = Autodesk.Revit.DB.Document;

namespace CableTraySection
{
    public class RevitUtils
    {
        public static void CreateView(UIDocument uidoc, double trayWidth, double trayHeight, double initial, double between, string sectionName, double fillingRatio)
        {
            Document doc = DataHelper.Doc;
            XYZ CurrentPosition = new XYZ(0, 0, 0);

            // Start a transaction
            using (Transaction trans = new Transaction(doc, "Create Drafting View"))
            {
                trans.Start();

                // Define the view scale (Optional)
                int viewScale = 3;

                // Get a default ViewFamilyType for drafting views
                ViewFamilyType draftingViewFamilyType = new FilteredElementCollector(doc)
                    .OfClass(typeof(ViewFamilyType))
                    .Cast<ViewFamilyType>()
                    .FirstOrDefault(vft => vft.ViewFamily == ViewFamily.Drafting);

                // Create the drafting view
                ViewDrafting draftingView = ViewDrafting.Create(doc, draftingViewFamilyType.Id);

                // Set the name of the drafting view
                draftingView.Name = sectionName;

                // Set the scale of the drafting view
                draftingView.Scale = viewScale;

                trans.Commit();
                trans.Start("Create Family Sympol");

                var cableTrayFamilySymbol = new FilteredElementCollector(doc).OfClass(typeof(FamilySymbol)).ToList().FirstOrDefault(x => x.Name == "CABLE TRAY SECTION") as FamilySymbol;
                if (cableTrayFamilySymbol.IsActive == false)
                {
                    cableTrayFamilySymbol.Activate();

                }

                var EarthingFamilySympol = new FilteredElementCollector(doc).OfClass(typeof(FamilySymbol)).ToList().FirstOrDefault(x => x.Name == "EARTHING CABLE") as FamilySymbol;
                if (EarthingFamilySympol.IsActive == false)
                {
                    EarthingFamilySympol.Activate();
                }

                var tray = doc.Create.NewFamilyInstance(CurrentPosition, cableTrayFamilySymbol, draftingView);
                tray.LookupParameter("Tray Length").Set(Utils.Convert_to_Feet(trayWidth));
                tray.LookupParameter("Tray Height").Set(Utils.Convert_to_Feet(trayHeight));
                for (int i = 0; i < DataHelper.Data.Count; i++)
                {
                    var CTypeForSympol = Utils.GetSecondAndThirdChanarcters(DataHelper.Data[i].DSelectedCable);
                    var ConductorFamilySympol = new FilteredElementCollector(doc).OfClass(typeof(FamilySymbol)).ToList().FirstOrDefault(x => x.Name == $"{CTypeForSympol} CABLE") as FamilySymbol;
                    if (ConductorFamilySympol.IsActive == false)
                    {
                        ConductorFamilySympol.Activate();
                    }
                   


                    if (i == 0)
                    {
                        var diameter = Utils.Convert_to_Feet(double.Parse(DataHelper.Data[i].DOD));
                        var eDiameter = Utils.Convert_to_Feet(double.Parse(DataHelper.Data[i].DEOD));
                        // To Centre Of the First Cable
                        CurrentPosition += new XYZ((diameter * initial + diameter / 2), 0, 0);
                        var cable = doc.Create.NewFamilyInstance(CurrentPosition, ConductorFamilySympol, draftingView);
                        cable.LookupParameter("Diameter").Set(Utils.Convert_to_Feet(double.Parse(DataHelper.Data[i].DOD)));
                        cable.LookupParameter("Comments").Set(DataHelper.Data[i].DSelectedCable + " - " + DataHelper.Data[i].DfromTo);

                        // To End Of the First Calbe
                        CurrentPosition += new XYZ((diameter / 2), 0, 0);


                        //EARTHING CABLE
                        if (eDiameter != 0)
                        {
                            var earthingPosition = CurrentPosition + new XYZ(eDiameter / 2, 0, 0);
                            var earthing = doc.Create.NewFamilyInstance(earthingPosition, EarthingFamilySympol, draftingView);
                            earthing.LookupParameter("Diameter").Set(Utils.Convert_to_Feet(double.Parse(DataHelper.Data[i].DEOD)));

                        }




                        continue;
                    }
                    else
                    {
                        var diameter = Utils.Convert_to_Feet(double.Parse(DataHelper.Data[i].DOD));
                        var eDiameter = Utils.Convert_to_Feet(double.Parse(DataHelper.Data[i].DEOD));

                        var bet = Math.Max(Utils.Convert_to_Feet(double.Parse(DataHelper.Data[i - 1].DOD)), Utils.Convert_to_Feet(double.Parse(DataHelper.Data[i].DOD)));
                        // To Centre Of the Next Cable
                        CurrentPosition += new XYZ(bet * between + diameter / 2, 0, 0);
                        var cable = doc.Create.NewFamilyInstance(CurrentPosition, ConductorFamilySympol, draftingView);
                        cable.LookupParameter("Diameter").Set(Utils.Convert_to_Feet(double.Parse(DataHelper.Data[i].DOD)));
                        cable.LookupParameter("Comments").Set(DataHelper.Data[i].DSelectedCable + " - " + DataHelper.Data[i].DfromTo);

                        // To End Of the Next Calbe
                        CurrentPosition += new XYZ(diameter / 2, 0, 0);


                        // Earthing
                        if (eDiameter != 0)
                        {
                            var earthingPosition = CurrentPosition + new XYZ(eDiameter / 2, 0, 0);
                            var earthing = doc.Create.NewFamilyInstance(earthingPosition, EarthingFamilySympol, draftingView);
                            earthing.LookupParameter("Diameter").Set(Utils.Convert_to_Feet(double.Parse(DataHelper.Data[i].DEOD)));

                        }


                        continue;

                    }
                }


                trans.Commit();

                TableAndData.DrawTable(doc, draftingView, new XYZ(Utils.Convert_to_Feet(-100), Utils.Convert_to_Feet(-100), 0), DataHelper.Data.Count + 1, DataHelper.Data, trayWidth.ToString(), fillingRatio , sectionName);



            }
        }
        public static void LoadFamiliesFromFolder(Document doc, string folderPath)
        {
            string[] familyFiles = Directory.GetFiles(folderPath, "*.rfa");

            using (Transaction tr = new Transaction(doc, "LoadFamilies"))
            {
                var collector = new FilteredElementCollector(doc).WhereElementIsElementType().ToList();
                tr.Start();

                foreach (var familyFile in familyFiles)
                {
                    string familyName = Path.GetFileNameWithoutExtension(familyFile);
                    bool familyExists = collector.Any(x => x.Name == familyName);

                    if (!familyExists)
                    {
                        doc.LoadFamily(familyFile);
                    }
                }

                tr.Commit();
            }
        }


    }
}

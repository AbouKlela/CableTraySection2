using Autodesk.Revit.Creation;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using Document = Autodesk.Revit.DB.Document;

namespace CableTraySection
{
    public class RevitUtils
    {
        public static void CreateView(UIDocument uidoc, double trayWidth, double trayHeight, double initial, double between, string sectionName, double fillingRatio, bool table, bool dimention, double trayThickness)
        {
            Document doc = DataHelper.Doc;
            ReferenceArray referenceArray = new ReferenceArray();
            ReferenceArray referenceArrayEarthing = new ReferenceArray();
            XYZ currentPosition = new XYZ(0, 0, 0);


            // Start a transaction
            using (Transaction trans = new Transaction(doc, "Create Drafting View"))
            {
                trans.Start();
                var dimType = CreateDimentionType(DataHelper.Doc);

                // Define the view scale (Optional)
                int viewScale = 3;

                // Get a default ViewFamilyType for drafting views
                ViewFamilyType draftingViewFamilyType = new FilteredElementCollector(doc)
                    .OfClass(typeof(ViewFamilyType))
                    .Cast<ViewFamilyType>()
                    .FirstOrDefault(vft => vft.ViewFamily == ViewFamily.Drafting);

                // Create the drafting view
                ViewDrafting draftingView = ViewDrafting.Create(doc, draftingViewFamilyType.Id);

                //Get Unique Name
                string uniqueName = GetUniqueViewName(doc, sectionName);

                // Set the name of the drafting view
                draftingView.Name = uniqueName;

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

                var tray = doc.Create.NewFamilyInstance(currentPosition, cableTrayFamilySymbol, draftingView);
                tray.LookupParameter("Tray Length").Set((trayWidth).CTF());
                tray.LookupParameter("Tray Height").Set((trayHeight).CTF());
                tray.LookupParameter("Tray Thickness").Set((trayThickness).CTF());



                if (dimention)
                {
                    #region Dimention The Trray
                    referenceArray.Append(GetReferenceVertical(currentPosition, draftingView));
                    referenceArray.Append(GetReferenceVertical(currentPosition + new XYZ(trayWidth.CTF(), 0, 0), draftingView));
                    var line = Line.CreateBound(new XYZ(0, 0, 0) + new XYZ(0, (trayHeight + 50).CTF(), 0), new XYZ(trayWidth.CTF(), (trayHeight + 50).CTF(), 0));
                    doc.Create.NewDimension(draftingView, line, referenceArray, dimType);
                    #endregion 
                }




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

                        if (dimention)
                        {

                            #region Dimention First Cable
                            referenceArray.Append(GetReferenceVertical((currentPosition + new XYZ(diameter * initial, 0, 0)), draftingView));
                            referenceArray.Append(GetReferenceVertical((currentPosition + new XYZ((diameter * initial) + (diameter), 0, 0)), draftingView));
                            #endregion

                        }

                        // To Centre Of the First Cable
                        currentPosition += new XYZ((diameter * initial + diameter / 2), 0, 0);
                        var cable = doc.Create.NewFamilyInstance(currentPosition, ConductorFamilySympol, draftingView);

                        cable.LookupParameter("Diameter").Set(Utils.Convert_to_Feet(double.Parse(DataHelper.Data[i].DOD)));
                        cable.LookupParameter("Comments").Set(DataHelper.Data[i].DSelectedCable + " - " + DataHelper.Data[i].DfromTo);

                        // To End Of the First Calbe
                        currentPosition += new XYZ((diameter / 2), 0, 0);


                        //EARTHING CABLE
                        if (eDiameter != 0)
                        {
                            if (dimention)
                            {
                                referenceArrayEarthing.Append(GetReferenceVertical(currentPosition, draftingView));
                                referenceArrayEarthing.Append(GetReferenceVertical(currentPosition + new XYZ(eDiameter, 0, 0), draftingView));
                                var lineE = Line.CreateBound(currentPosition + new XYZ(0, (-20.0 - trayThickness).CTF(), 0), currentPosition + new XYZ(eDiameter, 0, 0) + new XYZ(0, (-20.0 - trayThickness).CTF(), 0));
                                doc.Create.NewDimension(draftingView, lineE, referenceArrayEarthing, dimType);
                                referenceArrayEarthing.Clear();
                            }

                            var earthingPosition = currentPosition + new XYZ(eDiameter / 2, 0, 0);
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

                        #region Dimention Rest of Cables
                        referenceArray.Append(GetReferenceVertical((currentPosition + new XYZ(diameter * between, 0, 0)), draftingView));
                        referenceArray.Append(GetReferenceVertical((currentPosition + new XYZ((diameter * between) + (diameter), 0, 0)), draftingView));
                        #endregion

                        // To Centre Of the Next Cable
                        currentPosition += new XYZ(bet * between + diameter / 2, 0, 0);
                        var cable = doc.Create.NewFamilyInstance(currentPosition, ConductorFamilySympol, draftingView);
                        cable.LookupParameter("Diameter").Set(Utils.Convert_to_Feet(double.Parse(DataHelper.Data[i].DOD)));
                        cable.LookupParameter("Comments").Set(DataHelper.Data[i].DSelectedCable + " - " + DataHelper.Data[i].DfromTo);

                        // To End Of the Next Calbe
                        currentPosition += new XYZ(diameter / 2, 0, 0);


                        // Earthing
                        if (eDiameter != 0)
                        {
                            if (dimention)
                            {
                                referenceArrayEarthing.Append(GetReferenceVertical(currentPosition, draftingView));
                                referenceArrayEarthing.Append(GetReferenceVertical(currentPosition + new XYZ(eDiameter, 0, 0), draftingView));
                                var lineE = Line.CreateBound(currentPosition + new XYZ(0, (-20.0 - trayThickness).CTF(), 0), currentPosition + new XYZ(eDiameter, 0, 0) + new XYZ(0, (-20.0 - trayThickness).CTF(), 0));
                                doc.Create.NewDimension(draftingView, lineE, referenceArrayEarthing, dimType);
                                referenceArrayEarthing.Clear();
                            }



                            var earthingPosition = currentPosition + new XYZ(eDiameter / 2, 0, 0);
                            var earthing = doc.Create.NewFamilyInstance(earthingPosition, EarthingFamilySympol, draftingView);
                            earthing.LookupParameter("Diameter").Set(Utils.Convert_to_Feet(double.Parse(DataHelper.Data[i].DEOD)));

                        }


                        continue;

                    }
                }

                if (dimention)
                {
                    var line2 = Line.CreateBound(new XYZ(0, 0, 0) + new XYZ(0, (trayHeight + 20).CTF(), 0), new XYZ(trayWidth.CTF(), (trayHeight + 20).CTF(), 0));
                    var dim = doc.Create.NewDimension(draftingView, line2, referenceArray, dimType);

                    // Move the text up
                    var segments = dim.Segments.Cast<DimensionSegment>().OrderBy(x => x.TextPosition.X).ToList();
                    for (int i = 0; i < segments.Count; i = i + 2)
                    {
                        var seg = segments[i];
                        seg.TextPosition = seg.TextPosition + new XYZ(0, 10.0.CTF(), 0);
                    }
                    //Tray Height Dimention
                    referenceArray.Clear();
                    var lineHeight = Line.CreateBound(new XYZ((trayWidth +trayThickness +  50).CTF(), 0, 0), new XYZ((trayWidth+ trayThickness+ 50).CTF(), (trayHeight).CTF(), 0));
                    referenceArray.Append(GetReferenceHorizontal(new XYZ((trayWidth).CTF(), 0, 0), draftingView));
                    referenceArray.Append(GetReferenceHorizontal(new XYZ((trayWidth).CTF(), (trayHeight).CTF(), 0), draftingView));
                    doc.Create.NewDimension(draftingView, lineHeight, referenceArray, dimType);

                    //Tray Thikness Dimention
                    referenceArray.Clear();
                    var lineThikness = Line.CreateBound(new XYZ(0, (-20.0 - trayThickness).CTF(), 0), new XYZ((-trayThickness).CTF(), (-20.0 - trayThickness).CTF(), 0));
                    referenceArray.Append(GetReferenceVertical(new XYZ(0, 0, 0), draftingView));
                    referenceArray.Append(GetReferenceVertical(new XYZ((-trayThickness).CTF(), 0, 0), draftingView));
                    var d = doc.Create.NewDimension(draftingView, lineThikness, referenceArray, dimType);

                    referenceArray.Clear();





                }

                trans.Commit();

                if (table)
                {
                    TableAndData.DrawTable(doc, draftingView, new XYZ(Utils.Convert_to_Feet(0), Utils.Convert_to_Feet(-50), 0), DataHelper.Data.Count + 1, DataHelper.Data, trayWidth.ToString(), fillingRatio, sectionName);
                }



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


        public static Reference GetReferenceVertical(XYZ point, ViewDrafting view)
        {
            var line = Line.CreateBound(point, point + new XYZ(0, (1.0).CTF(), 0));

            var curve = DataHelper.Doc.Create.NewDetailCurve(view, line) as CurveElement;

            var geometryObjects = curve.get_Geometry(new Options() { ComputeReferences = true }).ToList().FirstOrDefault() as Line;

            var reference = geometryObjects.Reference;

            return reference;
        }
        public static Reference GetReferenceHorizontal(XYZ point, ViewDrafting view)
        {
            var line = Line.CreateBound(point, point + new XYZ((1.0).CTF(), 0, 0));

            var curve = DataHelper.Doc.Create.NewDetailCurve(view, line) as CurveElement;

            var geometryObjects = curve.get_Geometry(new Options() { ComputeReferences = true }).ToList().FirstOrDefault() as Line;

            var reference = geometryObjects.Reference;

            return reference;
        }


        public static string GetUniqueViewName(Document doc, string baseName)
        {
            // Start with the original section name
            string uniqueName = baseName;
            int counter = 1;

            // Get all views in the document
            FilteredElementCollector collector = new FilteredElementCollector(doc)
                .OfClass(typeof(ViewDrafting));

            // Loop until we find a unique name
            while (collector.Any(v => v.Name == uniqueName))
            {
                // Add a dash and a number to the base name
                uniqueName = $"{baseName}-{counter}";
                counter++;
            }

            return uniqueName;
        }


        public static DimensionType CreateDimentionType(Document doc)
        {


            var allDimensionTypesDimStyle = new FilteredElementCollector(doc).OfClass(typeof(DimensionType)).WhereElementIsElementType().Cast<DimensionType>().ToList();
            var dimensionTypeCehck = allDimensionTypesDimStyle.FirstOrDefault(x => x.Name == "CT-TOOLKIT");

            if (dimensionTypeCehck != null)
            {
                var check = allDimensionTypesDimStyle.FirstOrDefault(x => x.Name == "CT-TOOLKIT");
                return check;
            }
            var dimType = allDimensionTypesDimStyle.Where(x => x.FamilyName != x.Name && x.StyleType == DimensionStyleType.Linear).ToList().FirstOrDefault();

            // Duplicate the DimensionType
            DimensionType newDimType = dimType.Duplicate("CT-TOOLKIT") as DimensionType;

            if (newDimType != null)
            {
                FormatOptions formatOptions = newDimType.GetUnitsFormatOptions();
                formatOptions.UseDefault = false;
                formatOptions.SetUnitTypeId(UnitTypeId.Millimeters);
                formatOptions.Accuracy = 0.1;
                newDimType.SetUnitsFormatOptions(formatOptions);
                newDimType.get_Parameter(BuiltInParameter.TEXT_SIZE).Set(2.0.CTF());
                newDimType.get_Parameter(BuiltInParameter.TEXT_STYLE_BOLD).Set(1);
                newDimType.get_Parameter(BuiltInParameter.ALTERNATE_UNITS).Set(0);
            }



            return newDimType;


        }


    }
}

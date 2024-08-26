using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using CableTraySection.View;

namespace CableTraySection
{
    [Transaction(TransactionMode.Manual)]
    public class ExtCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                DataHelper.UiDoc = commandData.Application.ActiveUIDocument;
                DataHelper.Doc = DataHelper.UiDoc.Document;
                ExternalEvent externalEvent = ExternalEvent.Create(new EventHandeler());
                DataHelper.ExEvent = externalEvent;
                //Load Families
                EventHandeler.Event = Request.event2;
                DataHelper.ExEvent.Raise();

                DataHelper.FilePath = "C:\\KLELA-SECTION\\Cable Diameters.xlsx";

                CTView view = new CTView();
                view.Show();

                return Result.Succeeded;
            }
            catch (System.Exception e)
            {

                TaskDialog.Show("Error", e.Message);
                return Result.Failed;
                throw;
            }
        }
    }
}

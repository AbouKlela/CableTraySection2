using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using CableTraySection.View;
using CableTraySection.ViewModel;

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
                DataHelper.FilePath = "C:\\KLELA-SECTION\\Cable Diameters.xlsx";



                LogIn logIn = new LogIn();
                logIn.Show();


                ViewModelLogIn.RequestClose += (s, e) =>
                {


                    //Load Families
                    EventHandeler.Event = Request.event2;
                    DataHelper.ExEvent.Raise();



                };


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

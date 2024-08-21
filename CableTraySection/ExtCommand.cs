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

            CTView view = new CTView();
            view.Show();

            return Result.Succeeded;
        }
    }
}

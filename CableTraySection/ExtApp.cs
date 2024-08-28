using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace CableTraySection
{
    internal class ExtApp : IExternalApplication
    {
        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        public Result OnStartup(UIControlledApplication application)
        {
            try
            {
                application.CreateRibbonTab("IBIMS");
            }
            catch (Exception)
            {

                
            }

            var panel = application.CreateRibbonPanel("IBIMS", "Cable Tray Section");

            var button = panel.AddItem(new PushButtonData("Cable Tray Section", "Cable Tray Section", typeof(ExtCommand).Assembly.Location, typeof(ExtCommand).FullName)) as PushButton;

            button.LargeImage = new BitmapImage(new Uri("pack://application:,,,/CableTraySection;component/Resources/Icon.png"));
            button.ToolTip = "Toolkit For Helping in Making Cable Tray Sections in Details";


            return Result.Succeeded;

        }
    }
}

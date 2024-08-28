using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;
using System.Windows;
using System.Windows.Media;
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

            button.LargeImage = new BitmapImage();

            button.ToolTip = "Toolkit For Helping in Making Cable Tray Sections in Details";

            button.LargeImage = Properties.Resources.CableTray.ToImageSource();

            return Result.Succeeded;

        }
       

    }
    public static class Extension
    {
        public static ImageSource ToImageSource(this Icon icon)
        {
            ImageSource imageSource = Imaging.CreateBitmapSourceFromHIcon(
                icon.Handle,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());

            return imageSource;
        }
    }
}

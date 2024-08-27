﻿using Autodesk.Revit.UI;

namespace CableTraySection
{
    public class EventHandeler : IExternalEventHandler
    {
        public static Request Event { get; set; }
        public static double TrayWidht { get; set; }
        public static double Trayheight { get; set; }
        public static double Initial { get; set; }
        public static double Between { get; set; }
        public static string SectionName { get; set; }
        public static double FillingRatio { get; set; }

        public static bool Table { get; set; }
        public static bool Dimention { get; set; }

        public void Execute(UIApplication app)
        {
            try
            {
                switch (Event)
                {
                    case Request.event1:
                        //Create View
                        RevitUtils.CreateView(DataHelper.UiDoc, TrayWidht, Trayheight, Initial, Between, SectionName, FillingRatio, Table, Dimention);
                        break;
                    case Request.event2:
                        //Load Families
                        RevitUtils.LoadFamiliesFromFolder(DataHelper.Doc, "C:\\KLELA-SECTION\\Families");

                        break;
                    case Request.event3:
                        //Do something
                        break;
                    default:
                        break;
                }
            }
            catch (System.Exception e )
            {

                TaskDialog.Show("Error", e.Message,TaskDialogCommonButtons.Ok);
            }


        }

        public string GetName()
        {
            return "EventHandeler";
        }
    }



    public enum Request
    {
        event1,
        event2,
        event3
    }
}

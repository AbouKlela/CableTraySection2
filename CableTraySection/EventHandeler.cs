using Autodesk.Revit.UI;

namespace CableTraySection
{
    public class EventHandeler : IExternalEventHandler
    {
        public static EventEnum Event { get; set; }
        public void Execute(UIApplication app)
        {
            switch (Event)
            {
                case EventEnum.event1:
                    //Do something
                    break;
                case EventEnum.event2:
                    //Do something
                    break;
                case EventEnum.event3:
                    //Do something
                    break;
                default:
                    break;
            }


        }

        public string GetName()
        {
            return "EventHandeler";
        }
    }



    public enum EventEnum
    {
        event1,
        event2,
        event3
    }
}

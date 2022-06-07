using System.Collections.Generic;
using Unity.Services.Analytics;
using Unity.Services.Analytics.Internal;

namespace Unity.Services.PushNotifications
{
    interface IPushNotificationEventsWrapper
    {
        void RecordCustomEvent(string eventName, Dictionary<string, object> parameters, int version);
    }

    class EventsWrapper: IPushNotificationEventsWrapper
    {
        public void RecordCustomEvent(string eventName, Dictionary<string, object> parameters, int version)
        {
            Event evt = new Event(eventName, version);

            foreach (KeyValuePair<string, object> parameter in parameters)
            {
                evt.Parameters.Set(parameter.Key, parameter.Value);
            }

            evt.Parameters.AddUserCountry();



            AnalyticsService.Instance.RecordInternalEvent(evt);
        }
    }
}

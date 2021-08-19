using UnityEngine;

namespace Unity.Services.PushNotifications
{
    interface IPushNotificationAnalyticsPlatformWrapper
    {
        string ApplicationVersion();
        RuntimePlatform RuntimePlatform();
        bool IsApplicationFocused();
        string AnalyticsPlatform();
        string UserCountry();
    }
    
    class PushNotificationsAnalyticsPlatformWrapper: IPushNotificationAnalyticsPlatformWrapper
    {
        public string ApplicationVersion()
        {
            return Application.version;
        }

        public RuntimePlatform RuntimePlatform()
        {
            return Application.platform;
        }

        public bool IsApplicationFocused()
        {
            return Application.isFocused;
        }

        public string AnalyticsPlatform()
        {
#if UNITY_IOS
            return "IOS";
#elif UNITY_ANDROID
            return "ANDROID";            
#else
            return "UNKNOWN";
#endif
        }

        public string UserCountry()
        {
            return "GB";
        }
    }
}

using System;
using Unity.Services.Analytics.Internal;
using UnityEngine;

namespace Unity.Services.PushNotifications
{
    interface IPushNotificationAnalyticsPlatformWrapper
    {
        string ApplicationVersion();
        RuntimePlatform RuntimePlatform();
        bool IsApplicationFocused();
        string AnalyticsPlatform();
    }
    
    class PushNotificationsAnalyticsPlatformWrapper: IPushNotificationAnalyticsPlatformWrapper
    {
        const string k_UnknownCountryCode = "ZZ";
        
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
    }
}

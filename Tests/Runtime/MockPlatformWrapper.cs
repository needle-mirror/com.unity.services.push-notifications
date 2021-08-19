using UnityEngine;

namespace Unity.Services.PushNotifications.Tests
{
    class MockPlatformWrapper: IPushNotificationAnalyticsPlatformWrapper
    {
        public const string mockApplicationVersion = "1.0.0";
        public RuntimePlatform mockRuntimePlatform = UnityEngine.RuntimePlatform.Android;
        public const string mockAnalyticsPlatform = "IPHONE";
        public const string mockUserCountry = "UK";
        public bool mockIsApplicationFocused = true;

        public string ApplicationVersion()
        {
            return mockApplicationVersion;
        }

        public RuntimePlatform RuntimePlatform()
        {
            return mockRuntimePlatform;
        }

        public string AnalyticsPlatform()
        {
            return mockAnalyticsPlatform;
        }

        public string UserCountry()
        {
            return mockUserCountry;
        }

        public bool IsApplicationFocused()
        {
            return mockIsApplicationFocused;
        }
    }
}

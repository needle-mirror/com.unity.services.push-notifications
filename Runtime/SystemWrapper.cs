using UnityEngine;

namespace Unity.Services.PushNotifications
{
    interface ISystemWrapper
    {
        string ApplicationVersion();
        RuntimePlatform RuntimePlatform();
        bool IsApplicationFocused();

        PushNotificationSettings GetSettings();
    }

    class SystemWrapper : ISystemWrapper
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

        public PushNotificationSettings GetSettings()
        {
            return PushNotificationSettings.GetAssetInstance();
        }
    }
}

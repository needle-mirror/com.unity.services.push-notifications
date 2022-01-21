using System;
using UnityEditor;
using UnityEngine;

namespace Unity.Services.PushNotifications
{
    /// <summary>
    /// A class for configuring the Push Notifications SDK.
    ///
    /// Some of the fields in this class are platform specific, and are prefixed with the platform they relate to. However, it is not required to
    /// use a separate settings object on each platform, as the SDK will automatically only retrieve settings for the platform it is running on.
    /// </summary>
    public class PushNotificationSettings : ScriptableObject
    {
        internal const string settingsAssetName = "pushNotificationsSettings";
        internal const string resourcesContainer = "Assets";
        internal const string resourcesDirectory = "Resources";
        internal const string assetDirectory = resourcesContainer + "/" + resourcesDirectory;
        internal const string fullAssetPath = assetDirectory + "/" + settingsAssetName + ".asset";
        
        public string androidApiKey;
        public string androidSenderId;
        public string androidApplicationId;
        public string androidProjectId;

        /// <summary>
        /// The Firebase API Key for the project to use for Firebase Cloud Messaging. This should match the project you have configured
        /// in the Push Notification dashboard.
        /// </summary>
        [Obsolete("This field is obsolete - use the androidApiKey serialised field directly", false)]
        public string AndroidApiKey {
            get => androidApiKey;
            set => androidApiKey = value;
        }
        
        /// <summary>
        /// The Firebase sender ID for the project to use for Firebase Cloud Messaging. This should match the project you have configured
        /// in the Push Notification dashboard.
        /// </summary>
        [Obsolete("This field is obsolete - use the androidSenderId serialised field directly", false)]
        public string AndroidSenderId {
            get => androidSenderId;
            set => androidSenderId = value;
        }
        
        /// <summary>
        /// The Firebase application ID for the project to use for Firebase Cloud Messaging. This should match the project you have configured
        /// in the Push Notification dashboard.
        /// </summary>
        [Obsolete("This field is obsolete - use the androidApplicationId serialised field directly", false)]
        public string AndroidApplicationId {
            get => androidApplicationId;
            set => androidApplicationId = value;
        }
        
        /// <summary>
        /// The Firebase project ID for the project to use for Firebase Cloud Messaging. This should match the project you have configured
        /// in the Push Notification dashboard.
        /// </summary>
        [Obsolete("This field is obsolete - use the androidProjectId serialised field directly", false)]
        public string AndroidProjectId {
            get => androidProjectId;
            set => androidProjectId = value;
        }

        /// <summary>
        /// Retrieves the copy of the settings persisted as an asset. Will return an empty settings object if no asset is available.
        /// The settings in this asset can be updated in the Editor from Project Settings > Services > Push Notifications.
        /// </summary>
        /// <returns>The settings persisted as an asset in the project, or a blank object if no settings are persisted.</returns>
        public static PushNotificationSettings GetAssetInstance()
        {
            PushNotificationSettings cfg = Resources.Load<PushNotificationSettings>(settingsAssetName);

            if(cfg == null)
            {
                cfg = CreateInstance<PushNotificationSettings>();
            }
            
            return cfg;
        }
    }
}

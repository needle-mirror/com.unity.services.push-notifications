using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

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

        [Header("Android (Firebase) Settings")]
        [FormerlySerializedAs("androidApiKey")]
        public string firebaseWebApiKey;
        [FormerlySerializedAs("androidSenderId")]
        public string firebaseProjectNumber;
        [FormerlySerializedAs("androidApplicationId")]
        public string firebaseAppID;
        [FormerlySerializedAs("androidProjectId")]
        public string firebaseProjectID;

        /// <summary>
        /// Retrieves the copy of the settings persisted as an asset. Will return an empty settings object if no asset is available.
        /// The settings in this asset can be updated in the Editor from Project Settings > Services > Push Notifications.
        /// </summary>
        /// <returns>The settings persisted as an asset in the project, or a blank object if no settings are persisted.</returns>
        public static PushNotificationSettings GetAssetInstance()
        {
            PushNotificationSettings cfg = Resources.Load<PushNotificationSettings>(settingsAssetName);

            if (cfg == null)
            {
                cfg = CreateInstance<PushNotificationSettings>();
            }

            return cfg;
        }
    }
}

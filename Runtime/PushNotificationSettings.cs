namespace Unity.Services.PushNotifications
{
    /// <summary>
    /// A class for configuring the Push Notifications SDK.
    ///
    /// Some of the fields in this class are platform specific, and are prefixed with the platform they relate to. However, it is not required to
    /// use a separate settings object on each platform, as the SDK will automatically only retrieve settings for the platform it is running on.
    /// </summary>
    public class PushNotificationSettings
    {
        /// <summary>
        /// The Firebase API Key for the project to use for Firebase Cloud Messaging. This should match the project you have configured
        /// in the Push Notification dashboard.
        /// </summary>
        public string AndroidApiKey;
        
        /// <summary>
        /// The Firebase sender ID for the project to use for Firebase Cloud Messaging. This should match the project you have configured
        /// in the Push Notification dashboard.
        /// </summary>
        public string AndroidSenderId;
        
        /// <summary>
        /// The Firebase application ID for the project to use for Firebase Cloud Messaging. This should match the project you have configured
        /// in the Push Notification dashboard.
        /// </summary>
        public string AndroidApplicationId;
        
        /// <summary>
        /// The Firebase project ID for the project to use for Firebase Cloud Messaging. This should match the project you have configured
        /// in the Push Notification dashboard.
        /// </summary>
        public string AndroidProjectId;
    }
}

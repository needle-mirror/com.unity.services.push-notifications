using Unity.Services.Core.Editor;
using UnityEditor;

namespace Unity.Services.PushNotifications.Editor
{
    public struct PushNotificationIdentifier : IEditorGameServiceIdentifier
    {
        public string GetKey() => "Push Notifications";
    }

    public class PushNotificationEditorGameService : IEditorGameService
    {
        static readonly PushNotificationIdentifier k_Identifier = new PushNotificationIdentifier();

        public string GetFormattedDashboardUrl()
        {
            return null;
        }

        public string Name => "Push Notifications";
        public IEditorGameServiceIdentifier Identifier => k_Identifier;
        public bool RequiresCoppaCompliance => false;
        public bool HasDashboard => false;
        public IEditorGameServiceEnabler Enabler => null;
    }
}

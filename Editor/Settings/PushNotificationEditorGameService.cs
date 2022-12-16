using Unity.Services.Core.Editor;
using Unity.Services.Core.Editor.OrganizationHandler;
using UnityEditor;

namespace Unity.Services.PushNotifications.Editor
{
    public struct PushNotificationIdentifier : IEditorGameServiceIdentifier
    {
        public string GetKey() => "Push Notifications";
    }

    internal class PushNotificationEditorGameService : IEditorGameService
    {
        static readonly PushNotificationIdentifier k_Identifier = new PushNotificationIdentifier();

        public string GetFormattedDashboardUrl()
        {
            string organizationId = OrganizationProvider.Organization.Key;
            string projectId = CloudProjectSettings.projectId;

            bool isProjectConfigured = !string.IsNullOrEmpty(organizationId) && !string.IsNullOrEmpty(projectId);

            if (isProjectConfigured)
            {
                return $"https://dashboard.unity3d.com/organizations/{organizationId}/projects/{projectId}/environments/default/campaigns/push/overview";
            }

            return "https://dashboard.unity3d.com";
        }

        public string Name => "Push Notifications";
        public IEditorGameServiceIdentifier Identifier => k_Identifier;
        public bool RequiresCoppaCompliance => false;
        public bool HasDashboard => false;
        public IEditorGameServiceEnabler Enabler => null;
    }
}

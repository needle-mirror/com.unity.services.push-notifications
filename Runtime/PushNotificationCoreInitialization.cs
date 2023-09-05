using System.Threading.Tasks;
using Unity.Services.Core.Analytics.Internal;
using Unity.Services.Core.Internal;
using UnityEngine;

namespace Unity.Services.PushNotifications
{
    class PushNotificationCoreInitialization : IInitializablePackage
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Register()
        {
            CoreRegistry.Instance.RegisterPackage(new PushNotificationCoreInitialization())
                .DependsOn<IAnalyticsStandardEventComponent>();
        }

        public Task Initialize(CoreRegistry registry)
        {
            var analyticsSdk = registry.GetServiceComponent<IAnalyticsStandardEventComponent>();
            var platformWrapper = new SystemWrapper();

            var containerObject = PushNotificationsContainer.CreateContainer();
            var mainThreadHelper = new MainThreadHelper();
            var analytics = new PushAnalytics(analyticsSdk, platformWrapper);

            IPushPlatform platformLogic;
#if UNITY_IOS && !UNITY_EDITOR
            platformLogic = new IOSPushNotifications(containerObject);
#elif UNITY_ANDROID && !UNITY_EDITOR
            platformLogic = new AndroidPushNotifications(containerObject);
#else
            platformLogic = new UnsupportedPlatformLogic();
#endif

            PushNotificationsService.internalInstance = new PushNotificationsServiceInstance(
                analytics,
                platformLogic,
                platformWrapper,
                mainThreadHelper);

            platformLogic.Initialize(PushNotificationsService.internalInstance);
            containerObject.Initialize(PushNotificationsService.internalInstance);

            return Task.CompletedTask;
        }
    }
}

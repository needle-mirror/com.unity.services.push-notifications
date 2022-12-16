using System.Threading.Tasks;
using Unity.Services.Core.Internal;
using UnityEngine;

namespace Unity.Services.PushNotifications
{
    class PushNotificationCoreInitialization : IInitializablePackage
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Register()
        {
            CoreRegistry.Instance.RegisterPackage(new PushNotificationCoreInitialization());
        }

        public Task Initialize(CoreRegistry registry)
        {
            PushNotificationsService.internalInstance = new PushNotificationsServiceInstance();

            return Task.CompletedTask;
        }
    }
}

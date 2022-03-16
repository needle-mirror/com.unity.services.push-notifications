using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Unity.Services.PushNotifications
{
    static class MainThreadHelper
    {
        static SynchronizationContext s_UnitySynchronizationContext;
        static TaskScheduler s_TaskScheduler;
        static int s_MainThreadId;

#if UNITY_EDITOR
        [InitializeOnLoadMethod]
#endif
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Init()
        {
            s_UnitySynchronizationContext = SynchronizationContext.Current;
            s_TaskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
            s_MainThreadId = Thread.CurrentThread.ManagedThreadId;
        }

        internal static async void RunOnMainThread(Action methodToRun)
        {
            await Task.Factory.StartNew(methodToRun, CancellationToken.None, TaskCreationOptions.None,
                s_TaskScheduler);
        }
    }
}

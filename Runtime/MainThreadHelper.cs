using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Unity.Services.PushNotifications
{
    interface IMainThreadHelper
    {
        void RunOnMainThread(Action methodToRun);
    }

    class MainThreadHelper : IMainThreadHelper
    {
        static SynchronizationContext s_UnitySynchronizationContext;
        static TaskScheduler s_TaskScheduler;
        static int s_MainThreadId;

        public MainThreadHelper()
        {
            s_UnitySynchronizationContext = SynchronizationContext.Current;
            s_TaskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
            s_MainThreadId = Thread.CurrentThread.ManagedThreadId;
        }

        public void RunOnMainThread(Action methodToRun)
        {
            Task.Factory.StartNew(methodToRun, CancellationToken.None, TaskCreationOptions.None, s_TaskScheduler);
        }
    }
}

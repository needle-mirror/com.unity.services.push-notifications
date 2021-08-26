using System;
using System.Collections.Generic;
using NUnit.Framework;
using Unity.Services.PushNotifications;
using Unity.Services.PushNotifications.Tests;
using UnityEngine;

[TestFixture]
class PushNotificationAnalyticsTests
{
    PushNotificationAnalytics m_Analytics;
    MockPlatformWrapper m_MockPlatformWrapper;
    EventMock m_MockEventWrapper;

    Dictionary<string, object> m_NotificationUserInfoDataFull = new Dictionary<string, object>
    {
        {"_ddCampaign", "23456"},
        {"_ddCohort", "34567"},
        {"_ddCommunicationSender", "IOS"},
        {"_ddId", "12345"},
        {"_ddName", "ddName"},
    };


    [SetUp]
    public void SetUp()
    {
        m_MockEventWrapper = new EventMock();
        m_MockPlatformWrapper = new MockPlatformWrapper();
        m_Analytics = new PushNotificationAnalytics(m_MockEventWrapper, m_MockPlatformWrapper);
    }

    [Test]
    public void WhenPushTokenIsUpdatedAndThePlatformIsIOSRecordTheRightData()
    {
        string token = "myPushNotificationToken";
        m_MockPlatformWrapper.mockRuntimePlatform = RuntimePlatform.IPhonePlayer;
        
        m_Analytics.RecordPushTokenUpdated(token);

        Assert.AreEqual("notificationServices", m_MockEventWrapper.LastCalledEventName);
        Assert.AreEqual(1, m_MockEventWrapper.LastCalledVersion);

        Dictionary<string, object> calledParams = m_MockEventWrapper.LastCalledParams;
        Assert.AreEqual(MockPlatformWrapper.mockApplicationVersion, calledParams["clientVersion"]);
        Assert.AreEqual(MockPlatformWrapper.mockAnalyticsPlatform, calledParams["platform"]);
        Assert.AreEqual(SdkVersion.SDK_VERSION, calledParams["sdkVersion"]);
        Assert.AreEqual("com.unity.services.pushNotifications.PushNotificationsAnalytics.RecordPushTokenUpdated", calledParams["sdkMethod"]);
        Assert.AreEqual(token, calledParams["pushNotificationToken"]);
    }
    
    [Test]
    public void WhenPushTokenIsUpdatedAndThePlatformIsAppleTVRecordTheRightData()
    {
        string token = "myPushNotificationToken";
        m_MockPlatformWrapper.mockRuntimePlatform = RuntimePlatform.tvOS;
        
        m_Analytics.RecordPushTokenUpdated(token);

        Assert.AreEqual("notificationServices", m_MockEventWrapper.LastCalledEventName);
        Assert.AreEqual(1, m_MockEventWrapper.LastCalledVersion);

        Dictionary<string, object> calledParams = m_MockEventWrapper.LastCalledParams;
        Assert.AreEqual(MockPlatformWrapper.mockApplicationVersion, calledParams["clientVersion"]);
        Assert.AreEqual(MockPlatformWrapper.mockAnalyticsPlatform, calledParams["platform"]);
        Assert.AreEqual(SdkVersion.SDK_VERSION, calledParams["sdkVersion"]);
        Assert.AreEqual("com.unity.services.pushNotifications.PushNotificationsAnalytics.RecordPushTokenUpdated", calledParams["sdkMethod"]);
        Assert.AreEqual(token, calledParams["pushNotificationToken"]);
    }
    
    [Test]
    public void WhenPushTokenIsUpdatedAndThePlatformIsAndroidRecordTheRightData()
    {
        string token = "myPushNotificationToken";
        m_MockPlatformWrapper.mockRuntimePlatform = RuntimePlatform.Android;
        
        m_Analytics.RecordPushTokenUpdated(token);

        Assert.AreEqual("notificationServices", m_MockEventWrapper.LastCalledEventName);
        Assert.AreEqual(1, m_MockEventWrapper.LastCalledVersion);

        Dictionary<string, object> calledParams = m_MockEventWrapper.LastCalledParams;
        Assert.AreEqual(MockPlatformWrapper.mockApplicationVersion, calledParams["clientVersion"]);
        Assert.AreEqual(MockPlatformWrapper.mockAnalyticsPlatform, calledParams["platform"]);
        Assert.AreEqual(SdkVersion.SDK_VERSION, calledParams["sdkVersion"]);
        Assert.AreEqual("com.unity.services.pushNotifications.PushNotificationsAnalytics.RecordPushTokenUpdated", calledParams["sdkMethod"]);
        Assert.AreEqual(token, calledParams["androidRegistrationID"]);
    }

    [Test]
    public void WhenNotificationOpenedIsCalledWithFullDataAndDidLaunchTheRightFieldsAreSet()
    {
        m_Analytics.RecordNotificationOpened(m_NotificationUserInfoDataFull, true);
        
        Assert.AreEqual("notificationOpened", m_MockEventWrapper.LastCalledEventName);
        Assert.AreEqual(1, m_MockEventWrapper.LastCalledVersion);

        Dictionary<string, object> calledParams = m_MockEventWrapper.LastCalledParams;
        Assert.AreEqual(MockPlatformWrapper.mockApplicationVersion, calledParams["clientVersion"]);
        Assert.AreEqual(MockPlatformWrapper.mockAnalyticsPlatform, calledParams["platform"]);
        Assert.AreEqual(SdkVersion.SDK_VERSION, calledParams["sdkVersion"]);
        Assert.AreEqual("com.unity.services.pushNotifications.PushNotificationsAnalytics.RecordNotificationOpened", calledParams["sdkMethod"]);
        Assert.AreEqual(true, calledParams["notificationLaunch"]);
        Assert.AreEqual("OPEN", calledParams["communicationState"]);
        
        Assert.AreEqual(23456, calledParams["campaignId"]);
        Assert.AreEqual(34567, calledParams["cohortId"]);
        Assert.AreEqual("IOS", calledParams["communicationSender"]);
        Assert.AreEqual(12345, calledParams["notificationId"]);
        Assert.AreEqual("ddName", calledParams["notificationName"]);
    }

    [Test]
    public void WhenNotificationOpenedIsCalledWithMinimalDataAndDidLaunchTheRightFieldsAreSet()
    {
        m_Analytics.RecordNotificationOpened(new Dictionary<string, object>(), true);
        
        Assert.AreEqual("notificationOpened", m_MockEventWrapper.LastCalledEventName);
        Assert.AreEqual(1, m_MockEventWrapper.LastCalledVersion);

        Dictionary<string, object> calledParams = m_MockEventWrapper.LastCalledParams;
        Assert.AreEqual(MockPlatformWrapper.mockApplicationVersion, calledParams["clientVersion"]);
        Assert.AreEqual(MockPlatformWrapper.mockAnalyticsPlatform, calledParams["platform"]);
        Assert.AreEqual(SdkVersion.SDK_VERSION, calledParams["sdkVersion"]);
        Assert.AreEqual("com.unity.services.pushNotifications.PushNotificationsAnalytics.RecordNotificationOpened", calledParams["sdkMethod"]);
        Assert.AreEqual(true, calledParams["notificationLaunch"]);
        Assert.AreEqual("OPEN", calledParams["communicationState"]);
    }
    
    [Test]
    public void WhenNotificationOpenedIsCalledWithMinimalDataAndDidNotLaunchTheRightFieldsAreSet()
    {
        m_Analytics.RecordNotificationOpened(m_NotificationUserInfoDataFull, false);
        
        Assert.AreEqual("notificationOpened", m_MockEventWrapper.LastCalledEventName);
        Assert.AreEqual(1, m_MockEventWrapper.LastCalledVersion);

        Dictionary<string, object> calledParams = m_MockEventWrapper.LastCalledParams;
        Assert.AreEqual(MockPlatformWrapper.mockApplicationVersion, calledParams["clientVersion"]);
        Assert.AreEqual(MockPlatformWrapper.mockAnalyticsPlatform, calledParams["platform"]);
        Assert.AreEqual(SdkVersion.SDK_VERSION, calledParams["sdkVersion"]);
        Assert.AreEqual("com.unity.services.pushNotifications.PushNotificationsAnalytics.RecordNotificationOpened", calledParams["sdkMethod"]);
        Assert.AreEqual(false, calledParams.ContainsValue("notificationLaunch"));
        Assert.AreEqual("OPEN", calledParams["communicationState"]);
    }
    
    class EventMock : IPushNotificationEventsWrapper
    {
        public string LastCalledEventName;
        public Dictionary<string, object> LastCalledParams;
        public int LastCalledVersion;

        public void RecordCustomEvent(string eventName, Dictionary<string, object> parameters, int version)
        {
            LastCalledParams = parameters;
            LastCalledEventName = eventName;
            LastCalledVersion = version;
        }
    }
}

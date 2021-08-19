using System.Collections.Generic;
using NUnit.Framework;
using Unity.Services.PushNotifications;
using Unity.Services.PushNotifications.Tests;

[TestFixture]
public class PushNotificationReceivedHandlerTest
{
    PushNotificationReceivedHandler m_NotificationReceivedHandler;
    PushNotificationsAnalyticsMock m_PushNotificationsAnalyticsMock;
    MockPlatformWrapper m_AnalyticsPlatformWrapper;

    static string s_SampleJson = "{\"test\":true}";

    [SetUp]
    public void SetUp()
    {
        m_AnalyticsPlatformWrapper = new MockPlatformWrapper();
        m_PushNotificationsAnalyticsMock = new PushNotificationsAnalyticsMock();
        m_NotificationReceivedHandler = new PushNotificationReceivedHandler(m_PushNotificationsAnalyticsMock, m_AnalyticsPlatformWrapper);
    }
    
    [Test]
    public void WhenHandlingAReceivedNotification_AndTheDataIsMissing_NoAnalyticsDataShouldBeSent_AndEventShouldNotBeProcessed()
    {
        Dictionary<string, object> data = m_NotificationReceivedHandler.HandleReceivedNotification(null);
        
        Assert.IsNull(data);
        Assert.IsFalse(m_PushNotificationsAnalyticsMock.NotificationOpenedWasCalled);
    }

    [Test]
    public void WhenHandlingAReceivedNotification_AndTheDataIsNotValidJson_NoAnalyticsDataShouldBeSent_AndEventShouldNotBeProcessed()
    {
        Dictionary<string, object> data = m_NotificationReceivedHandler.HandleReceivedNotification("notValidJson");
        
        Assert.IsNull(data);
        Assert.IsFalse(m_PushNotificationsAnalyticsMock.NotificationOpenedWasCalled);
    }

    [Test]
    public void WhenHandlingAReceivedNotification_AndTheDataIsValid_AndItIsACleanStart_TheCorrectAnalyticsDataShouldBeSent()
    {
        Dictionary<string, object> data = m_NotificationReceivedHandler.HandleReceivedNotification(s_SampleJson);
        
        Assert.AreEqual(true, data["test"]);
        Assert.AreEqual(data, m_PushNotificationsAnalyticsMock.LastUpdatedPayload);
        Assert.AreEqual(true, m_PushNotificationsAnalyticsMock.LastUpdatedAppDidLaunch);
    }

    [Test]
    public void WhenHandlingAReceivedNotification_AndTheDataIsValid_AndItIsNotACleanStart_AndTheApplicationIsNotFocused_AsANotificationHasBeenPreviouslyProcessed_TheCorrectAnalyticsShouldBeSent()
    {
        m_AnalyticsPlatformWrapper.mockIsApplicationFocused = false;
        m_NotificationReceivedHandler.HandleReceivedNotification(s_SampleJson);
        Dictionary<string, object> secondData = m_NotificationReceivedHandler.HandleReceivedNotification(s_SampleJson);
        
        Assert.AreEqual(true, secondData["test"]);
        Assert.AreEqual(secondData, m_PushNotificationsAnalyticsMock.LastUpdatedPayload);
        Assert.AreEqual(true, m_PushNotificationsAnalyticsMock.LastUpdatedAppDidLaunch);
    }
    
    [Test]
    public void WhenHandlingAReceivedNotification_AndTheDataIsValid_AndItIsNotACleanStart_AndTheApplicationIsFocused_AsANotificationHasBeenPreviouslyProcessed_TheCorrectAnalyticsShouldBeSent()
    {
        m_AnalyticsPlatformWrapper.mockIsApplicationFocused = true;
        m_NotificationReceivedHandler.HandleReceivedNotification(s_SampleJson);
        Dictionary<string, object> secondData = m_NotificationReceivedHandler.HandleReceivedNotification(s_SampleJson);
        
        Assert.AreEqual(true, secondData["test"]);
        Assert.AreEqual(secondData, m_PushNotificationsAnalyticsMock.LastUpdatedPayload);
        Assert.AreEqual(false, m_PushNotificationsAnalyticsMock.LastUpdatedAppDidLaunch);
    }

    class PushNotificationsAnalyticsMock : IPushNotificationsAnalytics
    {
        public Dictionary<string, object> LastUpdatedPayload;
        public bool LastUpdatedAppDidLaunch;

        public bool NotificationOpenedWasCalled;
        
        public void RecordPushTokenUpdated(string pushToken)
        {}

        public void RecordNotificationOpened(Dictionary<string, object> payload, bool didLaunch)
        {
            LastUpdatedPayload = payload;
            LastUpdatedAppDidLaunch = didLaunch;
            NotificationOpenedWasCalled = true;
        }
    }
}
using System.Collections.Generic;
using Unity.Services.Core.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.Services.PushNotifications.Editor
{
    internal class PushSettingsProvider : EditorGameServiceSettingsProvider
    {
        const string k_Title = "Push Notifications";
        const string k_GoToDashboardContainer = "dashboard-button-container";
        const string k_GoToDashboardBtn = "dashboard-link-button";


        protected override IEditorGameService EditorGameService => k_GameService;
        protected override string Title => k_Title;
        protected override string Description => "This package adds support for Push Notifications to your game. It allows sending rich push notifications with images, and provides analytics on the number of received push notifications.";

        static readonly PushNotificationEditorGameService k_GameService = new PushNotificationEditorGameService();

        PushSettingsProvider(SettingsScope scopes, IEnumerable<string> keywords = null)
            : base(GenerateProjectSettingsPath(k_Title), scopes, keywords) {}

        protected override VisualElement GenerateServiceDetailUI()
        {
            SerializedObject serializedSettings = GetSerializedSettings();

            VisualElement containerVisualElement = new VisualElement();

            Label headerLabel = new Label("Android (Firebase) Settings");
            headerLabel.style.fontSize = 14;
            headerLabel.style.unityFontStyleAndWeight = UnityEngine.FontStyle.Bold;
            headerLabel.style.marginLeft = 4;
            containerVisualElement.Add(headerLabel);

            CreateFormRow("Firebase Web API Key", nameof(PushNotificationSettings.firebaseWebApiKey), serializedSettings, containerVisualElement);
            CreateFormRow("Firebase Project Number", nameof(PushNotificationSettings.firebaseProjectNumber), serializedSettings, containerVisualElement);
            CreateFormRow("Firebase App ID", nameof(PushNotificationSettings.firebaseAppID), serializedSettings, containerVisualElement);
            CreateFormRow("Firebase Project ID", nameof(PushNotificationSettings.firebaseProjectID), serializedSettings, containerVisualElement);

            return containerVisualElement;
        }

        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            base.OnActivate(searchContext, rootElement);
            SetDashboardButton(rootElement);
        }

        protected override VisualElement GenerateUnsupportedDetailUI()
        {
            return GenerateServiceDetailUI();
        }

        [SettingsProvider]
        public static SettingsProvider CreateSettingsProvider()
        {
            return new PushSettingsProvider(SettingsScope.Project);
        }

        static void CreateFormRow(string title, string fieldName, SerializedObject settings, VisualElement container)
        {
            string initialValue = settings.FindProperty(fieldName).stringValue;

            VisualElement rowContainer = new VisualElement
            {
                name = "push-settings-row"
            };

            TextField textField = new TextField(title)
            {
                value = initialValue
            };

            textField.RegisterValueChangedCallback(delegate(ChangeEvent<string> evt)
            {
                settings.FindProperty(fieldName).stringValue = evt.newValue;
                if (settings.ApplyModifiedProperties())
                {
                    AssetDatabase.SaveAssets();
                }
            });

            rowContainer.Add(textField);

            container.Add(rowContainer);
        }

        static void SetDashboardButton(VisualElement rootElement)
        {
            rootElement.Q(k_GoToDashboardContainer).style.display = DisplayStyle.Flex;
            var goToDashboard = rootElement.Q(k_GoToDashboardBtn);

            if (goToDashboard != null)
            {
                var clickable = new Clickable(() =>
                {
                    Application.OpenURL(k_GameService.GetFormattedDashboardUrl());
                });
                goToDashboard.AddManipulator(clickable);
            }
        }

        static SerializedObject GetSerializedSettings()
        {
            PushNotificationSettings cfg = AssetDatabase.LoadAssetAtPath<PushNotificationSettings>(PushNotificationSettings.fullAssetPath);

            if (cfg != null)
            {
                return new SerializedObject(cfg);
            }

            // If we couldn't load the asset we should create a new instance.
            cfg = PushNotificationSettings.CreateInstance<PushNotificationSettings>();

            if (!AssetDatabase.IsValidFolder(PushNotificationSettings.assetDirectory))
            {
                AssetDatabase.CreateFolder(PushNotificationSettings.resourcesContainer, PushNotificationSettings.resourcesDirectory);
            }
            AssetDatabase.CreateAsset(cfg, PushNotificationSettings.fullAssetPath);
            AssetDatabase.SaveAssets();

            return new SerializedObject(cfg);
        }
    }
}

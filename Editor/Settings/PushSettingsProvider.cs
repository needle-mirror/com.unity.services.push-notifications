using System.Collections.Generic;
using Editor.Settings;
using Unity.Services.Core.Editor;
using UnityEditor;
using UnityEngine.UIElements;

namespace Unity.Services.PushNotifications.Editor
{
    public class PushSettingsProvider: EditorGameServiceSettingsProvider
    {
        const string k_Title = "Push Notifications";        
        
        protected override IEditorGameService EditorGameService => k_GameService;
        protected override string Title => k_Title;
        protected override string Description => "This package adds support for Push Notifications to your game. It allows sending rich push notifications with images, and provides analytics on the number of received push notifications.";
        
        static readonly PushNotificationEditorGameService k_GameService = new PushNotificationEditorGameService();
        
        PushSettingsProvider(SettingsScope scopes, IEnumerable<string> keywords = null)
            : base(GenerateProjectSettingsPath(k_Title), scopes, keywords) { }

        protected override VisualElement GenerateServiceDetailUI()
        {
            SerializedObject serializedSettings = GetSerializedSettings();
            
            VisualElement containerVisualElement = new VisualElement();

            Label headerLabel = new Label("Android");
            headerLabel.style.fontSize = 14;
            headerLabel.style.unityFontStyleAndWeight = UnityEngine.FontStyle.Bold;
            headerLabel.style.marginLeft = 4;
            containerVisualElement.Add(headerLabel);
            
            CreateFormRow("API Key", nameof(PushNotificationSettings.androidApiKey), serializedSettings, containerVisualElement);
            CreateFormRow("Sender ID", nameof(PushNotificationSettings.androidSenderId), serializedSettings, containerVisualElement);
            CreateFormRow("Application ID", nameof(PushNotificationSettings.androidApplicationId), serializedSettings, containerVisualElement);
            CreateFormRow("Project ID", nameof(PushNotificationSettings.androidProjectId), serializedSettings, containerVisualElement);
            
            return containerVisualElement;
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
                if (settings.ApplyModifiedProperties()) {
                    AssetDatabase.SaveAssets();
                }
            });

            rowContainer.Add(textField);

            container.Add(rowContainer);
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

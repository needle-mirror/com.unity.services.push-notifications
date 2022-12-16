#if UNITY_IOS
using System;
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using UnityEngine;

namespace Unity.Services.PushNotifications.Editor
{
    internal class IOSBasicPushNotificationPostProcess : MonoBehaviour
    {
        [PostProcessBuild(0)]
        public static void OnPostProcessBuild(BuildTarget buildTarget, String path)
        {
            if (buildTarget != BuildTarget.iOS)
            {
                return;
            }

            string projectPath = PBXProject.GetPBXProjectPath(path);
            PBXProject project = new PBXProject();
            project.ReadFromFile(projectPath);

            string mainTargetGuid = project.GetUnityMainTargetGuid();
            string unityFrameworkGuid = project.GetUnityFrameworkTargetGuid();
            project.AddFrameworkToProject(unityFrameworkGuid, "UserNotifications.framework", true);

            AddCapabilities(project, projectPath, mainTargetGuid, path);
            UpdateInfoPlist(path);
            UpdatePreprocessorFile(path);

            project.WriteToFile(projectPath);
        }

        static void AddCapabilities(PBXProject project, string projectPath, string mainTargetGuid, string buildPath)
        {
            var entitlementsFileName = project.GetBuildPropertyForConfig(mainTargetGuid, "CODE_SIGN_ENTITLEMENTS");
            if (entitlementsFileName == null)
            {
                string bundleIdentifier = PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.iOS);
                entitlementsFileName = $"{bundleIdentifier.Substring(bundleIdentifier.LastIndexOf(".") + 1)}.entitlements";

                // Add the entitlements file to the build
                project.AddFile(Path.Combine(buildPath, entitlementsFileName), entitlementsFileName);
                project.AddBuildProperty(mainTargetGuid, "CODE_SIGN_ENTITLEMENTS", entitlementsFileName);
            }

            var capManager = new ProjectCapabilityManager(projectPath, entitlementsFileName, "Unity-iPhone");

            // TODO: Do we want to allow the user to specify which push environment they want to use, or should we always assume
            //       live? If so, the below will need updating with a new setting.
            const bool useDevEnvironment = false;
            capManager.AddPushNotifications(useDevEnvironment);

            capManager.WriteToFile();
        }

        /*
         * Add the remote notification background mode to the Xcode Info.plist file. Without this, we won't
         * get access to the correct background modes and iOS won't successfully register for remote notifications.
         */
        static void UpdateInfoPlist(string projectPath)
        {
            string plistPath = projectPath + "/Info.plist";
            PlistDocument plist = new PlistDocument();
            plist.ReadFromFile(plistPath);

            PlistElementArray existingBackgroundModes = (PlistElementArray)plist.root["UIBackgroundModes"] ?? plist.root.CreateArray("UIBackgroundModes");
            existingBackgroundModes.AddString("remote-notification");

            plist.WriteToFile(plistPath);
        }

        /*
         * Update the preprocessor file to ensure Unity sends us registration callbacks when we register for remote notifications.
         * If we don't do this, we are able to register, but we can't get access to the token in the native plugin to pass back
         * to Unity.
         */
        static void UpdatePreprocessorFile(string projectPath)
        {
            string preprocessorPath = projectPath + "/Classes/Preprocessor.h";
            var preprocessor = File.ReadAllText(preprocessorPath);
            preprocessor = preprocessor.Replace("UNITY_USES_REMOTE_NOTIFICATIONS 0", "UNITY_USES_REMOTE_NOTIFICATIONS 1");
            File.WriteAllText(preprocessorPath, preprocessor);
        }
    }
}
#endif

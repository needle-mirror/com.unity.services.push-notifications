#if UNITY_IOS
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using UnityEditor.iOS.Xcode.Extensions;
using UnityEngine;

namespace Unity.Services.PushNotifications.Editor
{
    internal class IOSRichPushNotificationPostProcess : MonoBehaviour
    {
        const string k_PathToInfoPlistInsideTarget = "Info.plist";

        [PostProcessBuild(1)]
        public static void CreateRichPushNotificationTarget(BuildTarget buildTarget, string buildOutputPath)
        {
            if (buildTarget != BuildTarget.iOS)
            {
                return;
            }

            string xcodeProjectPath = PBXProject.GetPBXProjectPath(buildOutputPath);
            PBXProject project = new PBXProject();
            project.ReadFromFile(xcodeProjectPath);

            string guidOfInitialTarget = project.GetUnityMainTargetGuid();

            string bundleIdentifierForNotificationService = Application.identifier + ".notificationservice";
            int indexOfLastIdentifierSection = bundleIdentifierForNotificationService.LastIndexOf('.') + 1;
            string displayName = bundleIdentifierForNotificationService.Substring(indexOfLastIdentifierSection);

            string pathToNotificationServiceImplementation = Path.Combine(buildOutputPath, displayName);

            if (!Directory.Exists(pathToNotificationServiceImplementation))
            {
                Directory.CreateDirectory(pathToNotificationServiceImplementation);
            }

            PlistDocument notificationServicePlist = new PlistDocument();
            string plistTemplatePath = Path.Combine(GetPathToSourceDirectory(), "Info.plist");
            notificationServicePlist.ReadFromFile(plistTemplatePath);
            notificationServicePlist.root.SetString("CFBundleDisplayName", displayName);
            notificationServicePlist.root.SetString("CFBundleIdentifier", bundleIdentifierForNotificationService);
            notificationServicePlist.root.SetString("CFBundleShortVersionString", PlayerSettings.bundleVersion);
            notificationServicePlist.root.SetString("CFBundleVersion", PlayerSettings.iOS.buildNumber);
            string pathToNotificationServicePlist = Path.Combine(pathToNotificationServiceImplementation, k_PathToInfoPlistInsideTarget);
            notificationServicePlist.WriteToFile(pathToNotificationServicePlist);

            string guidOfExtension = project.AddAppExtension(guidOfInitialTarget,
                displayName,
                bundleIdentifierForNotificationService,
                pathToNotificationServicePlist
            );
            string buildPhaseId = project.AddSourcesBuildPhase(guidOfExtension);

            AddSourceFileToProject(
                project,
                "NotificationService.h",
                displayName,
                guidOfExtension,
                buildPhaseId,
                pathToNotificationServiceImplementation
            );
            AddSourceFileToProject(
                project,
                "NotificationService.m",
                displayName,
                guidOfExtension,
                buildPhaseId,
                pathToNotificationServiceImplementation
            );
            AddFileToProject(
                project,
                pathToNotificationServicePlist,
                "Info.plist",
                displayName
            );

            project.AddFrameworkToProject(guidOfExtension, "NotificationCenter.framework", true);
            project.AddFrameworkToProject(guidOfExtension, "UserNotifications.framework", true);
            project.SetBuildProperty(guidOfExtension, "ARCHS", "$(ARCHS_STANDARD");
            project.SetBuildProperty(guidOfExtension, "DEVELOPMENT_TEAM", PlayerSettings.iOS.appleDeveloperTeamID);

            string[] copyableProperties =
            {
                "IPHONEOS_DEPLOYMENT_TARGET",
                "TARGETED_DEVICE_FAMILY"
            };
            foreach (string copyableProperty in copyableProperties)
            {
                string originalBuildProperty = project.GetBuildPropertyForAnyConfig(guidOfInitialTarget, copyableProperty);
                project.SetBuildProperty(guidOfExtension, copyableProperty, originalBuildProperty);
            }

            project.WriteToFile(xcodeProjectPath);
        }

        static void AddSourceFileToProject(
            PBXProject project,
            string filename,
            string extensionDisplayName,
            string extensionGuid,
            string buildPhase,
            string pathToImplementation
        )
        {
            string sourceFilepath = Path.Combine(GetPathToSourceDirectory(), filename);
            string destinationFilepath = Path.Combine(pathToImplementation, filename);
            if (File.Exists(destinationFilepath))
            {
                File.Delete(destinationFilepath);
            }
            File.Copy(sourceFilepath, destinationFilepath);
            string fileGUID = AddFileToProject(project, destinationFilepath, filename, extensionDisplayName);
            project.AddFileToBuildSection(extensionGuid, buildPhase, fileGUID);
        }

        static string GetPathToSourceDirectory()
        {
            return Path.Combine("Packages", "com.unity.services.push-notifications", "Editor", "iOS", "NotificationServiceFiles");
        }

        static string AddFileToProject(PBXProject project, string filepath, string filename, string extensionDisplayName)
        {
            return project.AddFile(filepath, extensionDisplayName + "/" + filename);
        }
    }
}
#endif

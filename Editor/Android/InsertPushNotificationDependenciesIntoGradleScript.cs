using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Android;
using UnityEngine;

namespace Unity.Services.PushNotifications.Editor
{
    internal class InsertPushNotificationDependenciesIntoGradleScript : IPostGenerateGradleAndroidProject
    {
        public int callbackOrder => 0;

        const string k_GradleDependencyOpeningTag = "dependencies {";

        readonly Dictionary<string, string> m_Dependencies = new Dictionary<string, string>
        {
            {"com.google.firebase:firebase-messaging-ktx", "22.0.0"}
        };

        public void OnPostGenerateGradleAndroidProject(string path)
        {
            string libraryBuildGradlePath = Path.Combine(path, "build.gradle");
            string buildGradleFileContent = File.ReadAllText(libraryBuildGradlePath);

            string dependencyString = "";

            foreach (var keyValuePair in m_Dependencies)
            {
                string library = keyValuePair.Key;
                string version = keyValuePair.Value;

                if (!buildGradleFileContent.Contains(library))
                {
                    dependencyString = $"{dependencyString}    implementation '{library}:{version}'\n";
                }
            }

            string updatedBuildGradleFileContent = buildGradleFileContent.Replace(k_GradleDependencyOpeningTag, $"{k_GradleDependencyOpeningTag}\n{dependencyString}");
            File.WriteAllText(libraryBuildGradlePath, updatedBuildGradleFileContent);

#if UNITY_2020_1_OR_NEWER
            string projectRoot = path.Substring(0, path.LastIndexOf(Path.DirectorySeparatorChar));
            string gradlePropertiesFilePath = Path.Combine(projectRoot, "gradle.properties");

            string gradlePropertiesFileContent = File.Exists(gradlePropertiesFilePath) ? File.ReadAllText(gradlePropertiesFilePath) : "";

            string updatedPropertiesFileContent = gradlePropertiesFileContent;
            if (!gradlePropertiesFileContent.Contains("android.useAndroidX="))
            {
                updatedPropertiesFileContent = $"{gradlePropertiesFileContent}\nandroid.useAndroidX=true";
            }
            else if (gradlePropertiesFileContent.Contains("android.useAndroidX=false"))
            {
                Debug.LogWarning("The Unity Push Notifications SDK requires androidx support. We've updated your gradle.properties file to enable androidX, check this is appropriate for your use case.");
                updatedPropertiesFileContent = gradlePropertiesFileContent.Replace("android.useAndroidX=false", "android.useAndroidX=true");
            }
            File.WriteAllText(gradlePropertiesFilePath, updatedPropertiesFileContent);
#endif
        }
    }
}

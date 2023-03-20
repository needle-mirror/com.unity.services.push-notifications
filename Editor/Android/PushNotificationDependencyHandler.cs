using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Xml.Linq;
using UnityEditor;
using UnityEngine;

namespace Unity.Services.PushNotifications.Editor
{
    internal class PushNotificationsDependencyHandler
    {
        const string k_FileName = "PushSDKDependencies.xml";
        const string k_DefaultFolder = "Assets/Push Notifications/Editor/Android";
        const string k_DefaultPath = k_DefaultFolder + "/" + k_FileName;

        public static readonly Dictionary<string, string> k_Dependencies = new Dictionary<string, string>
        {
            {"com.google.firebase:firebase-messaging-ktx", "22.0.0"}
        };

        [InitializeOnLoadMethod]
        internal static void GenerateDependenciesFile()
        {
            // If EDM4U or MDR is not installed, there is no need for a dependencies file.
            // The "InsertPushNotificationDependenciesIntoGradleScript.cs" script takes over instead to make use of gradle.
            if (!IsPlayServicesResolverInstalled()) return;

            var xmlDocument = GenerateXmlContentWithAllDependencies();
            var file = new FileInfo(k_DefaultPath);
            file.Directory.Create();
            if (!File.Exists(file.FullName))
            {
                File.WriteAllText(file.FullName, xmlDocument);
            }
        }

        static string GenerateXmlContentWithAllDependencies()
        {
            return (new XComment("Auto-generated file to be used by the External Dependency Manager for Unity (EDM4U) or the Mobile Dependency Resolver (MDR). \n Please do not modify by hand.") + Environment.NewLine +
                new XElement("dependencies",
                    new XElement("androidPackages",
                        new XElement("repositories",
                            new XElement("repository", "https://repo.maven.apache.org/maven2"),
                            new XElement("repository", "https://maven.google.com")
                        ),
                        k_Dependencies.Select(key =>
                            new XElement("androidPackage",
                                new XAttribute("spec", key.Key + ":" + key.Value)
                            )
                        )
                        ,
                        new XElement("settings",
                            new XElement("setting",
                                new XAttribute("name", "useAndroidX"),
                                new XAttribute("value", "true")
                            )
                        )
                    )
                )
            );
        }

        internal static bool IsPlayServicesResolverInstalled()
        {
            try
            {
                return Type.GetType("Google.VersionHandler, Google.VersionHandler") != null;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }
    }
}

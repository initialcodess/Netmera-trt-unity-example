using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.Collections;
using UnityEditor.iOS.Xcode;
using System.IO;
using UnityEditor.iOS.Xcode.Extensions;
using System.Text.RegularExpressions;


namespace Netmera {

    public class BuildPostProcessor {
        
        private const string infoPlist = "/Info.plist";
        private const string netmeraFolderPath = "./Assets/Plugins/iOS/Netmera/";
        private const string netmeraInfoPlist = "./Assets/Netmera-Info.plist";
        private const string targetName = "Unity-iPhone";
        
        // NotificationServiceExtension
        private const string serviceExtensionName = "NetmeraUnityNotificationServiceExtension";
        private const string notificationServiceBridgingHeaderPath = serviceExtensionName + "/NetmeraUnityNotificationServiceExtension-Bridging-Header.h";
        private const string notificationServiceFilePath = serviceExtensionName + "/NotificationService.swift";
        private const string notificationServicePath = netmeraFolderPath + serviceExtensionName;
        private const string notificationServicePlistPath = serviceExtensionName + infoPlist;

        // NotificationContentExtension
        private const string contentExtensionName = "NetmeraUnityNotificationContentExtension";
        private const string contentExtensionBridgingHeaderPath = contentExtensionName + "/NetmeraUnityNotificationContentExtension-Bridging-Header.h";
        private const string contentExtensionEnitlementsPath = contentExtensionName + "/NetmeraUnityNotificationContentExtension.entitlements";
        private const string contentExtensionMainInterfacePath = contentExtensionName + "/MainInterface.storyboard";
        private const string contentExtensionMainInterfacePhysicalPath = contentExtensionName + "/Base.lproj/MainInterface.storyboard";
        private const string contentExtensionPath = netmeraFolderPath + contentExtensionName;
        private const string contentExtensionPlistPath = contentExtensionName + infoPlist;
        private const string notificationViewControllerFilePath = contentExtensionName + "/NotificationViewController.swift";
        
        [PostProcessBuildAttribute(45)]
        public static void OnPostProcessBuildAttribute(BuildTarget target, string pathToBuiltProject) {
            if(target == BuildTarget.iOS) {
                string podfilePath = pathToBuiltProject + "/" + "Podfile";
                AddTargetsToPodFile(podfilePath);
            }
        }

        [PostProcessBuild]
        public static void OnPostProcessBuild(BuildTarget target, string pathToBuiltProject) {
            if(target == BuildTarget.iOS) {
                string projPath = PBXProject.GetPBXProjectPath(pathToBuiltProject);
                string serviceExtensionNameDestFolder = pathToBuiltProject + "/" + serviceExtensionName;
                string contentExtensionNameDestFolder = pathToBuiltProject + "/" + contentExtensionName;
                string plistPath = pathToBuiltProject + infoPlist;
                
                PBXProject proj = new PBXProject();
                proj.ReadFromFile (projPath);
                string mainTarget = proj.GetUnityMainTargetGuid();

                // Add appGroupName to the project's Info.plist.
                string appGroupName = CopyNetmeraInfoPlist(plistPath);
                
                // Copy NotificationServiceExtension folder.
                if (!File.Exists(serviceExtensionNameDestFolder)) {
                    FileUtil.CopyFileOrDirectory(notificationServicePath,  serviceExtensionNameDestFolder);
                }

                // Copy NotificationContentExtension folder.
                if (!File.Exists(contentExtensionNameDestFolder)) {
                    FileUtil.CopyFileOrDirectory(contentExtensionPath,  contentExtensionNameDestFolder);
                }

                // Create Info.plist file for both NotificationServiceExtension and NotificationContentExtension
                ExtensionCreatePlist(serviceExtensionNameDestFolder + infoPlist, serviceExtensionName);
                ExtensionCreatePlist(contentExtensionNameDestFolder + infoPlist, contentExtensionName);

                // Add necessary NotificationServiceExtension files to the target.
                AddServiceExtensionFiles(proj, mainTarget);

                // Add necessary NotificationContentExtension files to the target.
                AddContentExtensionFiles(proj, mainTarget, pathToBuiltProject, appGroupName);
                
                // Add required capabilities to the main project.
                AddProjectCapabilities(proj, pathToBuiltProject, appGroupName);

                // Write all changes to xcodeproj file.
                proj.WriteToFile (projPath);
            }  
        }

        private static string CopyNetmeraInfoPlist(string mainPlistPath) {
            // Read main plist.
            PlistDocument mainPlist = new PlistDocument();
            mainPlist.ReadFromString(File.ReadAllText(mainPlistPath));

            // Read netmera plist.
            PlistDocument netmeraPlist = new PlistDocument();
            netmeraPlist.ReadFromString(File.ReadAllText(netmeraInfoPlist));

            // Add netmera plist values as a dictionary to the main plist.
            string appGroupName = "";
            foreach (var item in netmeraPlist.root.values) {
                if (item.Value.GetType() == typeof(PlistElementBoolean)) {
                    mainPlist.root.SetBoolean(item.Key, item.Value.AsBoolean());
                } else if (item.Value.GetType() == typeof(PlistElementString)) {
                    if(item.Key == "netmera_app_group_name") {
                        appGroupName = item.Value.AsString();
                    }
                    mainPlist.root.SetString(item.Key, item.Value.AsString());
                }
            }
            File.WriteAllText(mainPlistPath, mainPlist.WriteToString());
            return appGroupName;
        }

        private static void AddServiceExtensionFiles(PBXProject proj, string mainTarget) {
            var serviceExtensionTarget = PBXProjectExtensions.AddAppExtension (proj, mainTarget, serviceExtensionName, PlayerSettings.GetApplicationIdentifier (BuildTargetGroup.iOS) + "." + serviceExtensionName, notificationServicePlistPath);            
            proj.AddFileToBuild (serviceExtensionTarget, proj.AddFile (notificationServiceFilePath, notificationServiceFilePath));
            proj.AddFile (notificationServicePlistPath, notificationServicePlistPath);
            proj.AddFile (notificationServiceBridgingHeaderPath, notificationServiceBridgingHeaderPath);
            SetBuildProperties(proj, serviceExtensionTarget);
        }

        private static void AddContentExtensionFiles(PBXProject proj, string mainTarget, string pathToBuiltProject, string appGroupName) {
            var contentExtensionTarget = PBXProjectExtensions.AddAppExtension (proj, mainTarget, contentExtensionName, PlayerSettings.GetApplicationIdentifier (BuildTargetGroup.iOS) + "." + contentExtensionName, contentExtensionPlistPath);            
            proj.AddFileToBuild (contentExtensionTarget, proj.AddFile (notificationViewControllerFilePath, notificationViewControllerFilePath));
            proj.AddFileToBuild (contentExtensionTarget, proj.AddFile (contentExtensionMainInterfacePhysicalPath, contentExtensionMainInterfacePath));
            proj.AddFile (contentExtensionPlistPath, contentExtensionPlistPath);
            proj.AddFile (contentExtensionBridgingHeaderPath, contentExtensionBridgingHeaderPath);
            SetBuildProperties(proj, contentExtensionTarget);

            var entitlementsPlist = new PlistDocument();
            PlistElementArray appGroupsArray = entitlementsPlist.root.CreateArray("com.apple.security.application-groups");
            appGroupsArray.AddString(appGroupName);
            File.WriteAllText(pathToBuiltProject + "/" + contentExtensionEnitlementsPath, entitlementsPlist.WriteToString());

            proj.AddFile(contentExtensionEnitlementsPath, contentExtensionEnitlementsPath);
            proj.SetBuildProperty(mainTarget, "CODE_SIGN_ENTITLEMENTS", contentExtensionEnitlementsPath);
        }

        private static void AddTargetsToPodFile(string podfilePath) {
            var dependenciesFile = File.ReadAllText("./Assets/Netmera/Editor/NetmeraDependencies.xml");
            var dependenciesRegex = new Regex("(?<=<iosPod name=\"Netmera/NetmeraWithoutAdId\" version=\").+(?=\" minTargetSdk=\"10.0\" addToAllTargets=\"true\"></iosPod>)");

            if (!dependenciesRegex.IsMatch(dependenciesFile)) {
                Debug.Log($"Could not read current iOS framework dependency version");
                return;
            }
            var requiredVersion = dependenciesRegex.Match(dependenciesFile).ToString();

            var serviceTarget = $"target '{serviceExtensionName}' do\n  pod 'Netmera/NetmeraWithoutAdId', '{requiredVersion}'\n  pod 'Netmera/NotificationContentExtension', '{requiredVersion}'\n  pod 'Netmera/NotificationServiceExtension', '{requiredVersion}'\nend\n";
            var contentTarget = $"target '{contentExtensionName}' do\n  pod 'Netmera/NetmeraWithoutAdId', '{requiredVersion}'\n  pod 'Netmera/NotificationContentExtension', '{requiredVersion}'\n  pod 'Netmera/NotificationServiceExtension', '{requiredVersion}'\nend\n";
            
            var podfile = File.ReadAllText(podfilePath);
            File.WriteAllText(podfilePath, podfile + serviceTarget + contentTarget);            
        }

        private static void ExtensionCreatePlist(string destPath, string name) {
            PlistDocument plist = new PlistDocument();
            plist.ReadFromString(File.ReadAllText(destPath));
            plist.root.SetString("CFBundleShortVersionString", PlayerSettings.bundleVersion);
            plist.root.SetString("CFBundleVersion", PlayerSettings.iOS.buildNumber);
            plist.root.SetString("CFBundleIdentifier", "$(PRODUCT_BUNDLE_IDENTIFIER)");
            plist.root.SetString("CFBundleName", "$(PRODUCT_NAME)");
            plist.root.SetString("CFBundleExecutable", "$(EXECUTABLE_NAME)");
            plist.root.SetString("CFBundleDisplayName", name);
            plist.root.SetString("CFBundleInfoDictionaryVersion", "6.0");
            File.WriteAllText(destPath, plist.WriteToString());
        }

        private static void SetBuildProperties(PBXProject proj, string target) {
            proj.SetBuildProperty(target, "TARGETED_DEVICE_FAMILY", "1,2");
            proj.SetBuildProperty(target, "IPHONEOS_DEPLOYMENT_TARGET", "10.0");
            proj.SetBuildProperty(target, "SWIFT_VERSION", "5.0");
            proj.SetBuildProperty(target, "ARCHS", "arm64");
            proj.SetBuildProperty(target, "DEVELOPMENT_TEAM", PlayerSettings.iOS.appleDeveloperTeamID);
        }
        
        private static void AddProjectCapabilities(PBXProject proj, string pathToBuiltProject, string appGroupName) {
            string pbxPath = PBXProject.GetPBXProjectPath(pathToBuiltProject);
            var targetGuid = proj.GetUnityMainTargetGuid();

            string entitlementsFileName = targetName + ".entitlements";
            string entitlementsPath = targetName + "/" + entitlementsFileName;
        
            var entitlementsPlist = new PlistDocument();
            entitlementsPlist.root.SetString("aps-environment", "development");
            PlistElementArray appGroupsArray = entitlementsPlist.root.CreateArray("com.apple.security.application-groups");
            appGroupsArray.AddString(appGroupName);
            File.WriteAllText(pathToBuiltProject + "/" + entitlementsPath, entitlementsPlist.WriteToString());

            proj.AddFile(entitlementsPath, entitlementsPath);
            proj.SetBuildProperty(targetGuid, "CODE_SIGN_ENTITLEMENTS", entitlementsPath);
            
            var projCapability = new ProjectCapabilityManager(pbxPath, entitlementsFileName, targetName);
            projCapability.AddBackgroundModes(BackgroundModesOptions.RemoteNotifications);
            projCapability.AddPushNotifications(true);
            projCapability.AddAppGroups(new[] { appGroupName });
            projCapability.WriteToFile();
        }
    }
}
 
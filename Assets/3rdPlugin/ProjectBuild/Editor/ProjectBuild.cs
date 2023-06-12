using System.Collections;
using System.IO;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.Rendering;

namespace HW.PB_Android {
    public class ProjectBuild : Editor {
        //在这里找出你当前工程所有的场景文件，假设你只想把部分的scene文件打包 那么这里可以写你的条件判断 总之返回一个字符串数组。


        static string[] GetBuildScenes() {
            List<string> names = new List<string>();
            foreach (EditorBuildSettingsScene e in EditorBuildSettings.scenes) {
                if (e == null)
                    continue;
                if (e.enabled)
                    names.Add(e.path);
            }
            return names.ToArray();
        }

        public static void SetAndroidData(PB_Settings _setting) {
            // 签名文件配置，若不配置，则使用Unity默认签名

            PlayerSettings.defaultInterfaceOrientation = _setting.uIOrientation;
            PlayerSettings.SplashScreen.show = _setting.enableSplashScreen;
            PlayerSettings.bundleVersion = _setting.verstion;

            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, _setting.packageName);

            PlayerSettings.Android.bundleVersionCode = _setting.bundleVersionCode;
            PlayerSettings.Android.minSdkVersion = _setting.miniAPI;
            PlayerSettings.Android.targetSdkVersion = _setting.targetAPI;
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64 | AndroidArchitecture.ARMv7;

            PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.Android, false);
            //如果使用其它版本OpenGL请自行设置
            GraphicsDeviceType[] graphicsDeviceType = _setting.graphicsDeviceTypes;
            PlayerSettings.SetGraphicsAPIs(BuildTarget.Android, graphicsDeviceType);
            PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.Android, true);

            PlayerSettings.Android.keyaliasName = _setting.keystoreAliasName;
            PlayerSettings.Android.keyaliasPass = _setting.keystoreAliasPassword;
            PlayerSettings.Android.keystoreName = Application.dataPath.Replace("/Assets", "") + $"/{_setting.keystoreName}.keystore";
            PlayerSettings.Android.keystorePass = _setting.keystorePassword;
        }

        /// <summary>
        /// 打包Android应用
        /// </summary>
        public static void BuildForAndroid(PB_Settings _setting, bool isAPK) {
            // 签名文件配置，若不配置，则使用Unity默认签名
            SetAndroidData(_setting);
            if (!isAPK) {
                EditorUserBuildSettings.androidCreateSymbolsZip = _setting.createSymbols;
            }
            EditorUserBuildSettings.buildAppBundle = !isAPK;
            string end = isAPK ? ".apk" : ".aab";
            // APK路径、名字配置
            string path = _setting.buildPath + "/" + $"{Application.productName}_{Application.version}({PlayerSettings.Android.bundleVersionCode})" + end;
            BuildPipeline.BuildPlayer(GetBuildScenes(), path, BuildTarget.Android, BuildOptions.None);
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace HW.PB_Android {
    public class PB_Settings : ScriptableObject {

        private const string SETTING_RESOURCES_PATH = "Assets/3rdPlugin/ProjectBuild/Settings/PB_Settings.asset";

        public static PB_Settings Load() => AssetDatabase.LoadAssetAtPath<PB_Settings>(SETTING_RESOURCES_PATH);

        public string buildPath;
        public bool enableExtendEditor = false;
        public UIOrientation uIOrientation = UIOrientation.Portrait;
        public bool enableSplashScreen = false;
        public string packageName;
        public string verstion = "0.0.1";
        public int bundleVersionCode = 1;
        public AndroidSdkVersions miniAPI = AndroidSdkVersions.AndroidApiLevel19;
        public AndroidSdkVersions targetAPI = (AndroidSdkVersions)33;
        public string keystoreAliasName = "hw";
        public string keystoreName = "user";
        public string keystorePassword = "123456";
        public string keystoreAliasPassword = "123456";
        public bool createSymbols = false;
        public GraphicsDeviceType[] graphicsDeviceTypes = new GraphicsDeviceType[3] { GraphicsDeviceType.OpenGLES2, GraphicsDeviceType.OpenGLES3, GraphicsDeviceType.Vulkan };
    }
}

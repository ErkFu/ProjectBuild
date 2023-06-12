using System.Collections;
using System.Collections.Generic;
using System;
using UnityEditor;
using UnityEngine;
using System.Runtime.InteropServices;
using UnityEngine.Rendering;
using System.Text.RegularExpressions;

namespace HW.PB_Android.Setup {
    [CustomEditor(typeof(PB_Settings))]
    public class PB_SettingsInspector : Editor {

        private PB_Settings pb_settings => target as PB_Settings;

        private string addChannel;

        [MenuItem("HW Games/Project Build", false, 500)]
        private static void BuildAPK() {
            Selection.activeObject = CreateSettings();
        }

        private static PB_Settings CreateSettings() {
            PB_Settings settings = PB_Settings.Load();
            if (settings == null) {
                settings = CreateInstance<PB_Settings>();
                if (!AssetDatabase.IsValidFolder("Assets/3rdPlugin/ProjectBuild/Settings"))
                    AssetDatabase.CreateFolder("Assets/3rdPlugin/ProjectBuild", "Settings");
                AssetDatabase.CreateAsset(settings, "Assets/3rdPlugin/ProjectBuild/Settings/PB_Settings.asset");
                settings = PB_Settings.Load();
            }
            return settings;
        }

        public override void OnInspectorGUI() {
            EditorGUILayout.BeginVertical();

            GUILayout.BeginVertical();
            GUILayout.Space(-4);
            GUILayout.Label("Project Setting", EditorStyles.largeLabel);
            GUILayout.EndVertical();
            EditorGUILayout.Space();

            GUILayout.BeginHorizontal();
            GUILayout.Space(-10);
            pb_settings.enableExtendEditor = EditorGUILayout.Toggle("Default Editor", pb_settings.enableExtendEditor);
            GUILayout.EndHorizontal();

            GUI.enabled = pb_settings.enableExtendEditor;
            GUILayout.BeginHorizontal();
            GUILayout.Space(0);
            EditorGUILayout.LabelField("Default Orientation", GUILayout.MaxWidth(150));
            pb_settings.uIOrientation = (UIOrientation)EditorGUILayout.EnumPopup(pb_settings.uIOrientation, GUILayout.MaxWidth(150));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(0);
            pb_settings.enableSplashScreen = EditorGUILayout.Toggle("Show Splash Screen", pb_settings.enableSplashScreen);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(0);
            EditorGUILayout.LabelField("Graphics Device Type", GUILayout.MaxWidth(150));
            SerializedObject so = new SerializedObject(pb_settings);
            SerializedProperty settingGraphics = so.FindProperty("graphicsDeviceTypes");
            if (settingGraphics.hasChildren) {
                serializedObject.ApplyModifiedProperties();
            }
            EditorGUILayout.PropertyField(settingGraphics, includeChildren: true);

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(0);
            EditorGUILayout.LabelField("Mini API Level", GUILayout.MaxWidth(150));
            pb_settings.miniAPI = (AndroidSdkVersions)EditorGUILayout.EnumPopup(pb_settings.miniAPI, GUILayout.MaxWidth(150));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(0);
            EditorGUILayout.LabelField("Target API Level", GUILayout.MaxWidth(150));
            pb_settings.targetAPI = (AndroidSdkVersions)EditorGUILayout.IntField((int)pb_settings.targetAPI, GUILayout.MaxWidth(150));
            GUILayout.EndHorizontal();
            GUI.enabled = true;


            EditorGUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUI.enabled = false;
            if (pb_settings.buildPath != null) {
                pb_settings.buildPath = EditorGUILayout.TextField("", pb_settings.buildPath);
            }
            else {
                string path = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                pb_settings.buildPath = EditorGUILayout.TextField("", path);
            }
            GUI.enabled = true;
            if (GUILayout.Button("Build Path", GUILayout.Width(100))) {
                SelectBuildPath();
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(-10);
            pb_settings.packageName = EditorGUILayout.TextField("Package Name", pb_settings.packageName);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(-10);
            pb_settings.verstion = EditorGUILayout.TextField("Version", pb_settings.verstion);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(-10);
            pb_settings.bundleVersionCode = EditorGUILayout.IntField("Bundle Version Code", pb_settings.bundleVersionCode);
            if (pb_settings.bundleVersionCode < 1) {
                pb_settings.bundleVersionCode = 1;
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(-10);
            pb_settings.keystoreName = EditorGUILayout.TextField("Keystore Name", pb_settings.keystoreName);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(-10);
            pb_settings.keystorePassword = EditorGUILayout.TextField("Keystore Password", pb_settings.keystorePassword);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(-10);
            pb_settings.keystoreAliasName = EditorGUILayout.TextField("Alias Name", pb_settings.keystoreAliasName);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(-10);
            pb_settings.keystoreAliasPassword = EditorGUILayout.TextField("Alias Password", pb_settings.keystoreAliasPassword);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(-10);
            pb_settings.createSymbols = EditorGUILayout.Toggle("Create symbols.zip", pb_settings.createSymbols);
            GUILayout.EndHorizontal();

            Splitter(new Color(0.35f, 0.35f, 0.35f));

            GUILayout.Space(10);

            if (GUILayout.Button("Change Adnroid Setting")) {
                EditorUtility.SetDirty(pb_settings);
                ProjectBuild.SetAndroidData(pb_settings);
            }

            GUILayout.Space(10);

            if (GUILayout.Button("Build APK")) {
                EditorUtility.SetDirty(pb_settings);
                ProjectBuild.BuildForAndroid(pb_settings, true);
            }
            GUILayout.Space(10);

            if (GUILayout.Button("Build AAB")) {
                EditorUtility.SetDirty(pb_settings);
                ProjectBuild.BuildForAndroid(pb_settings, false);
            }

            EditorGUILayout.EndVertical();

            if (GUI.changed) {
                SaveChange();
            }
        }

        public void Splitter(Color rgb, float thickness = 1, int margin = 0) {
            GUIStyle splitter = new GUIStyle();
            splitter.normal.background = EditorGUIUtility.whiteTexture;
            splitter.stretchWidth = true;
            splitter.margin = new RectOffset(margin, margin, 7, 7);

            Rect position = GUILayoutUtility.GetRect(GUIContent.none, splitter, GUILayout.Height(thickness));

            if (Event.current.type == EventType.Repaint) {
                Color restoreColor = GUI.color;
                GUI.color = rgb;
                splitter.Draw(position, false, false, false, false);
                GUI.color = restoreColor;
            }
        }

        /// <summary>
        /// 选择打包路径 按钮点击事件
        /// </summary>
        private void SelectBuildPath() {
            string path = GetPathFromWindowsExplorer();
            if (path!=null && path!="") {
                pb_settings.buildPath = path;
            }
            SaveChange();
            GUIUtility.ExitGUI();
        }

        [DllImport("shell32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
        private static extern IntPtr SHBrowseForFolder([In, Out] OpenDialogDir ofn);

        [DllImport("shell32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
        private static extern bool SHGetPathFromIDList([In] IntPtr pidl, [In, Out] char[] fileName);

        /// <summary>
        /// 调用WindowsExploer 并返回所选文件夹路径
        /// </summary>
        /// <param name="dialogtitle">打开对话框的标题</param>
        /// <returns>所选文件夹路径</returns>
        public string GetPathFromWindowsExplorer(string dialogtitle = "请选择路径") {
            try {
                OpenDialogDir ofn2 = new OpenDialogDir();
                ofn2.pszDisplayName = new string(new char[2048]);
                ; // 存放目录路径缓冲区  
                ofn2.lpszTitle = dialogtitle; // 标题  
                ofn2.ulFlags = 0x00000040; // 新的样式,带编辑框  
                IntPtr pidlPtr = SHBrowseForFolder(ofn2);

                char[] charArray = new char[2048];

                for (int i = 0; i < 2048; i++) {
                    charArray[i] = '\0';
                }

                SHGetPathFromIDList(pidlPtr, charArray);
                string res = new string(charArray);
                res = res.Substring(0, res.IndexOf('\0'));
                return res;
            }
            catch (Exception e) {
                Debug.LogError(e);
            }

            return string.Empty;
        }

        private void SaveChange() {
            EditorUtility.SetDirty(pb_settings);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }


    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public class OpenDialogDir {
        public IntPtr hwndOwner = IntPtr.Zero;
        public IntPtr pidlRoot = IntPtr.Zero;
        public String pszDisplayName = null;
        public String lpszTitle = null;
        public UInt32 ulFlags = 0;
        public IntPtr lpfn = IntPtr.Zero;
        public IntPtr lParam = IntPtr.Zero;
        public int iImage = 0;
    }

}
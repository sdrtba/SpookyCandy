using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using YG.Utils;
using YG.Insides;
using UnityEditor.Compilation;

namespace YG.EditorScr
{
    public class InfoYGEditorWindow : EditorWindow
    {
        private InfoYG scr;
        private SerializedObject serializedObject;
        private string lastPlatform;
        private int versionUpdates;
        private bool isExampleFiles = true;

        private Texture2D iconPluginYG2, iconSettings, iconDebugging;
        private Vector2 scrollPosition;

        private List<SerializedProperty> moduleIterators = new List<SerializedProperty>();
        private Texture2D[] moduleTextures = new Texture2D[0];

        [MenuItem("YG2/" + Langs.settings)]
        public static void ShowWindow()
        {
            InfoYGEditorWindow window = (InfoYGEditorWindow)GetWindow(typeof(InfoYGEditorWindow));
            window.titleContent = new GUIContent("Settings YG2");
            window.minSize = new Vector2(400, 700);
            window.Show();
        }

        private void OnEnable()
        {
            scr = InfoYG.Inst();
            serializedObject = new SerializedObject(scr);

            isExampleFiles = Directory.Exists(InfoYG.PATCH_PC_EXAMPLE);
            ExampleScenes.LoadSceneList();

            if (scr != null && scr.basicSettings.platform != null)
                lastPlatform = scr.basicSettings.platform.nameDefining;

            Serialize();
            ServerInfo.onLoadServerInfo += OnLoadServerInfo;
        }

        private void OnDisable()
        {
            ServerInfo.onLoadServerInfo -= OnLoadServerInfo;
        }

        private void Serialize()
        {
            versionUpdates = VersionUpdatesLabel();

            string iconPluginPath = $"{InfoYG.PATCH_ASSETS_YG2}/Scripts/EditorScr/Editor/Icons/IconPluginYG2.png";
            iconPluginYG2 = AssetDatabase.LoadAssetAtPath<Texture2D>(iconPluginPath);

            string iconSettingsPath = $"{InfoYG.PATCH_ASSETS_YG2}/Scripts/Editor/Icons/Settings.png";
            iconSettings = AssetDatabase.LoadAssetAtPath<Texture2D>(iconSettingsPath);

            string iconDebuggingPath = $"{InfoYG.PATCH_ASSETS_YG2}/Scripts/Editor/Icons/Debugging.png";
            iconDebugging = AssetDatabase.LoadAssetAtPath<Texture2D>(iconDebuggingPath);

            string[] modules = Directory.GetDirectories(InfoYG.PATCH_PC_MODULES);
            for (int i = 0; i < modules.Length; i++)
                modules[i] = Path.GetFileName(modules[i]);

            SerializedObject serializedObject = new SerializedObject(scr);
            SerializedProperty iterator = serializedObject.GetIterator();
            bool enterChildren = true;

            while (iterator.NextVisible(enterChildren))
            {
                enterChildren = false;

                bool cont = false;
                for (int i = 0; i < moduleIterators.Count; i++)
                {
                    if (moduleIterators[i].name == iterator.name)
                        cont = true;
                }
                if (cont)
                    continue;

                for (int i = 0; i < modules.Length; i++)
                {
                    if (modules[i] == iterator.name)
                    {
                        SerializedProperty newProperty = serializedObject.FindProperty(iterator.name);
                        moduleIterators.Add(newProperty);
                        break;
                    }
                }
            }

            moduleTextures = new Texture2D[moduleIterators.Count];
            for (int i = 0; i < moduleIterators.Count; i++)
            {
                string modulName = moduleIterators[i].name;
                string texturePath = $"{InfoYG.PATCH_ASSETS_MODULES}/{modulName}/Scripts/Editor/Icons/{modulName}.png";
                Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath);
                moduleTextures[i] = texture;
            }
        }

        private void Reserialize()
        {
            OnDisable();
            moduleIterators = new List<SerializedProperty>();
            moduleTextures = new Texture2D[0];
            OnEnable();
        }

        private void OnGUI()
        {
            if (serializedObject.targetObject == null || serializedObject == null)
            {
                Reserialize();
            }

            serializedObject.Update();
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            if (scr == null)
            {
                GUILayout.Label("Error!", EditorStyles.boldLabel);
                return;
            }
            GUIStyle styleOrange = TextStyles.Orange();
            GUIStyle styleGray = TextStyles.Gray();

            GUILayout.BeginVertical(YGEditorStyles.box);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(Langs.switchLang, YGEditorStyles.button))
            {
#if RU_YG2
                DefineSymbols.RemoveDefine("RU_YG2");
#else
                DefineSymbols.AddDefine("RU_YG2");
#endif
                Repaint();
            }
            GUILayout.Label($"    PluginYG v{InfoYG.VERSION_YG2}", styleOrange);
            GUILayout.Label($"Unity v{Application.unityVersion}", styleOrange);
            GUILayout.EndHorizontal();
            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            DocumentationEditor.DocButton();
            DocumentationEditor.ChatButton();
            DocumentationEditor.VideoButton();
            GUILayout.EndHorizontal();

            GUIStyle buttonStyle = new GUIStyle(YGEditorStyles.button);
            string versionButtonText = Langs.versionControl + "  (";

            if (versionUpdates == 0)
            {
                versionButtonText += Langs.versionsActual.ToLower();
            }
            else if (versionUpdates == 1)
            {
                versionButtonText += Langs.versionsUpdate.ToLower();
                buttonStyle.normal.textColor = TextStyles.colorGreen;
            }
            else if (versionUpdates == 2)
            {
                versionButtonText += Langs.versionsCritical.ToLower();
                buttonStyle.normal.textColor = new Color(1.5f, 0.4f, 0.1f);
            }

            versionButtonText += ")";

            if (GUILayout.Button(versionButtonText, buttonStyle, GUILayout.Height(23)))
                VersionControlWindow.ShowWindow();

            GUILayout.EndVertical();
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();

            GUIStyle styleHeader = TextStyles.While();
            styleHeader.fontSize = 18;
            styleHeader.fontStyle = FontStyle.Bold;

            GUIStyle styleHeader2 = TextStyles.Gray();
            styleHeader2.fontSize = 12;
            styleHeader2.fontStyle = FontStyle.Bold;

            if (iconPluginYG2)
            {
                GUILayout.Space(20);
                Rect textureRect = GUILayoutUtility.GetRect(40, 40, GUILayout.ExpandWidth(false));
                GUI.DrawTexture(textureRect, iconPluginYG2);
                GUILayout.Space(10);
            }
            else
            {
                styleHeader.alignment = TextAnchor.MiddleCenter;
                styleHeader2.alignment = TextAnchor.MiddleCenter;
            }

            GUILayout.BeginVertical();
            EditorGUILayout.LabelField("PLUGIN YG2", styleHeader);

            EditorGUILayout.LabelField(Langs.fullNamePlugin.ToUpper(), styleHeader2);
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            GUILayout.BeginHorizontal(YGEditorStyles.boxLight);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("basicSettings"), true);
            if (iconSettings)
            {
                Rect textureRect = GUILayoutUtility.GetRect(18, 18, GUILayout.ExpandWidth(false));
                GUI.DrawTexture(textureRect, iconSettings);
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal(YGEditorStyles.boxLight);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("simulationInEditor"), true);
            if (iconDebugging)
            {
                Rect textureRect = GUILayoutUtility.GetRect(18, 18, GUILayout.ExpandWidth(false));
                GUI.DrawTexture(textureRect, iconDebugging);
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(10);

            if (moduleIterators.Count > 0)
            {
                EditorGUILayout.LabelField(Langs.modulesSettings, TextStyles.Header());

                for (int i = 0; i < moduleIterators.Count; i++)
                {
                    moduleIterators[i].serializedObject.Update();

                    if (moduleTextures[i] == null)
                    {
                        GUILayout.BeginVertical(YGEditorStyles.boxLight);
                        EditorGUILayout.PropertyField(moduleIterators[i], true);
                        GUILayout.EndVertical();
                    }
                    else
                    {
                        GUILayout.BeginHorizontal(YGEditorStyles.boxLight);

                        EditorGUILayout.PropertyField(moduleIterators[i], true);

                        Rect textureRect = GUILayoutUtility.GetRect(18, 18, GUILayout.ExpandWidth(false));
                        GUI.DrawTexture(textureRect, moduleTextures[i]);

                        GUILayout.EndHorizontal();
                    }

                    moduleIterators[i].serializedObject.ApplyModifiedProperties();
                }
            }

            string currentPlatform = scr.basicSettings.platform != null ? scr.basicSettings.platform.nameDefining : string.Empty;

            if (lastPlatform != currentPlatform)
            {
                string[] platfFolders = Directory.GetDirectories(InfoYG.PATCH_PC_PLATFORMS);

                for (int i = 0; i < platfFolders.Length; i++)
                {
                    platfFolders[i] = platfFolders[i].Replace("\\", "/");
                    string folder = platfFolders[i] + "/SDK";

                    if (!Directory.Exists(folder))
                        continue;

                    string[] files = Directory.GetFiles(folder);
                    IEnumerable<string> asmdefFiles = files.Where(file => Path.GetExtension(file).Equals(".asmdef", StringComparison.OrdinalIgnoreCase));
                    List<string> asmdefList = asmdefFiles.ToList();

                    for (int j = 0; j < asmdefList.Count; j++)
                    {
                        string jsonOrig = File.ReadAllText(asmdefList[j]);
                        AsmdefJson json = JsonUtility.FromJson<AsmdefJson>(jsonOrig);

                        string platformName = "";
                        int lastSlashIndex = platfFolders[i].LastIndexOf('/');
                        if (lastSlashIndex != -1)
                            platformName = platfFolders[i].Substring(lastSlashIndex + 1);

                        if (currentPlatform.Contains(platformName))
                        {
                            json.includePlatforms = new string[10]
                            {
                                "Android", "Editor", "iOS", "CloudRendering", "macOSStandalone", "tvOS", "WSA", "WebGL", "WindowsStandalone32", "WindowsStandalone64"
                            };
                        }
                        else
                        {
                            json.includePlatforms = new string[1] { "XboxOne" };
                        }

                        string jsonNew = JsonUtility.ToJson(json, true);
                        File.WriteAllText(asmdefList[j], jsonNew);
                    }
                }

                if (currentPlatform == string.Empty)
                {
                    DefineSymbols.RemoveDefine(lastPlatform);
                    InfoYG.SetDefaultPlatform();
                }
                else
                {
                    if (scr.basicSettings.autoApplySettings)
                        scr.basicSettings.platform.ApplyProjectSettings();

                    DefineSymbols.RemoveDefine(lastPlatform);
                    DefineSymbols.AddDefine(currentPlatform);

                    PlatformSettings.SelectPlatform();
                }
                AssetDatabase.Refresh();
            }
            lastPlatform = currentPlatform;

            GUILayout.Space(20);
#if Storage || LanguageYGLegasy
            if (!DefineSymbols.CheckDefine(DefineSymbols.NJSON_DEFINE))
            {
                if (FastButton.Stringy(Langs.importNJSON))
                    UnityPackagesManager.ImportPackage(DefineSymbols.NJSON_PACKAGE);

            }
#endif
#if Storage
            if (DefineSymbols.CheckDefine(DefineSymbols.NJSON_DEFINE))
            {
                if (!DefineSymbols.CheckDefine(DefineSymbols.NJSON_STORAGE_DEFINE))
                {
                    if (FastButton.Stringy(Langs.activateNJSONForSave))
                        DefineSymbols.AddDefine(DefineSymbols.NJSON_STORAGE_DEFINE);
                }
                else
                {
                    if (FastButton.Stringy(Langs.deactivateNJSONForSave))
                        DefineSymbols.RemoveDefine(DefineSymbols.NJSON_STORAGE_DEFINE);
                }
            }
#endif
            if (FastButton.Stringy(Langs.resetInfoSettings))
            {
                if (EditorUtility.DisplayDialog(Langs.resetInfoSettings, Langs.resetInfoSettings_dialog, "Ok", Langs.cancel))
                {
                    Undo.RecordObject(scr, "Set default settings PluginYG2");

                    InfoYG tempInfoYG = ScriptableObject.CreateInstance<InfoYG>();
                    string tempPath = "Assets/SettingsYG2.asset";
                    AssetDatabase.CreateAsset(tempInfoYG, tempPath);
                    AssetDatabase.SaveAssets();

                    EditorUtility.CopySerialized(tempInfoYG, InfoYG.instance);

                    AssetDatabase.DeleteAsset(tempPath);

                    InfoYG.SetDefaultPlatform();

                    EditorUtility.SetDirty(InfoYG.Inst());
                    AssetDatabase.Refresh();
                    Reserialize();
                }
            }

            if (isExampleFiles)
            {
                GUILayout.Space(20);
                GUILayout.BeginVertical(YGEditorStyles.box);

                GUILayout.Label(Langs.demoScenesInBuildSettings, styleOrange);
                GUILayout.BeginHorizontal();
                if (GUILayout.Button(Langs.addDemoScenes, YGEditorStyles.button))
                    ExampleScenes.AddScenesToBuildSettings();
                if (GUILayout.Button(Langs.delete, YGEditorStyles.button))
                    ExampleScenes.RemoveScenesFromBuildSettings();
                GUILayout.EndHorizontal();

                string scenesStr;
                if (ExampleScenes.sceneNames.Length > 0)
                {
                    scenesStr = Langs.demoScenes;
                    foreach (var scene in ExampleScenes.sceneNames)
                    {
                        scenesStr += $"{scene},  ";
                    }
                    scenesStr = scenesStr.TrimEnd(',', ' ');
                }
                else
                {
                    scenesStr = Langs.demoNotAdded;
                }
                GUILayout.Label(scenesStr, EditorStyles.helpBox);

                GUILayout.Space(7);
                if (GUILayout.Button(Langs.deleteAllDemoMaterialsFromProject, YGEditorStyles.button))
                {
                    if (EditorUtility.DisplayDialog(Langs.removeAllDemoMaterials, Langs.removeAllDemoDialog, Langs.removeAllDemoMaterials, Langs.cancel))
                    {
                        ExampleScenes.RemoveScenesFromBuildSettings();

                        List<string> directories = Directory.GetDirectories(InfoYG.PATCH_PC_MODULES).ToList();
                        for (int i = 0; i < directories.Count; i++)
                        {
                            directories[i] += "/Example";
                            if (Directory.Exists(directories[i]))
                                FileYG.DeleteDirectory(directories[i]);
                        }

                        FileYG.DeleteDirectory(InfoYG.PATCH_PC_EXAMPLE);
                        AssetDatabase.Refresh();
                    }
                }
                GUILayout.EndVertical();
            }

            EditorGUILayout.EndScrollView();

            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();

            Repaint();
        }

        private void OnLoadServerInfo()
        {
            versionUpdates = VersionUpdatesLabel();
        }

        private int VersionUpdatesLabel()
        {
            int versionUpdates = 0;
            List<string> modulesStr = new List<string>
            {
                $"PluginYG2 v{InfoYG.VERSION_YG2}"
            };

            string[] platfomFolders = Directory.GetDirectories(InfoYG.PATCH_PC_PLATFORMS);
            string[] platfomNames = new string[platfomFolders.Length];

            for (int i = 0; i < platfomFolders.Length; i++)
                platfomNames[i] = Path.GetFileName(platfomFolders[i]);

            for (int i = 0; i < platfomNames.Length; i++)
            {
                string platfomVersionPathc = $"{platfomFolders[i]}/Version.txt";

                if (File.Exists(platfomVersionPathc))
                {
                    string version = File.ReadAllText(platfomVersionPathc);
                    platfomNames[i] += " " + version;
                }
            }

            modulesStr.AddRange(platfomNames);

            if (File.Exists(InfoYG.FILE_MODULES_PC))
                modulesStr.AddRange(File.ReadAllLines(InfoYG.FILE_MODULES_PC).ToList());

            for (int i = 0; i < modulesStr.Count; i++)
            {
                if (modulesStr[i] == string.Empty)
                    continue;

                string name = modulesStr[i];
                string version = "0";

                int spaceIndex = modulesStr[i].IndexOf(" ");
                if (spaceIndex > -1)
                {
                    name = modulesStr[i].Remove(spaceIndex);
                    version = modulesStr[i].Remove(0, spaceIndex + 2);
                }

                for (int j = 0; j < ServerInfo.saveInfo.modules.Length; j++)
                {
                    if (name == ServerInfo.saveInfo.modules[j].name)
                    {
                        float.TryParse(version, NumberStyles.Float, CultureInfo.InvariantCulture, out float projectVersion);
                        float.TryParse(ServerInfo.saveInfo.modules[j].version, NumberStyles.Float, CultureInfo.InvariantCulture, out float lastVersion);

                        if (lastVersion > projectVersion)
                        {
                            versionUpdates = 1;
                            if (ServerInfo.saveInfo.modules[j].critical)
                                return 2;
                        }
                    }
                }
            }
            return versionUpdates;
        }

        [Serializable]
        class AsmdefJson
        {
            public string name;
            public string rootNamespace;
            public string[] references;
            public string[] includePlatforms;
            public string[] excludePlatforms;
            public bool allowUnsafeCode;
            public bool[] overrideReferences;
            public string[] precompiledReferences;
            public bool autoReferenced;
            public string[] defineConstraints;
            public string[] versionDefines;
            public bool noEngineReferences;
        }
    }
}
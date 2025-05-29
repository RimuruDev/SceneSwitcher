// **************************************************************** //
//
//   Copyright (c) RimuruDev. All rights reserved.
//   Contact me: 
//          - Gmail:    rimuru.dev@gmail.com
//          - LinkedIn: https://www.linkedin.com/in/rimuru/
//          - Gists:    https://gist.github.com/RimuruDev/af759ce6d9768a38f6838d8b7cc94fc8
//          - GitHub:   https://github.com/RimuruDev
//          - GitHub Organizations: https://github.com/Rimuru-Dev
//
// **************************************************************** //

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Serialization;

namespace RimuruDevUtils.SceneSwitcher
{
    public sealed class SceneSwitcher : EditorWindow, IHasCustomMenu
    {
        private const string SETTINGS_STORAGE_PATH = "Assets/Editor/SceneSwitcher/SceneSwitcherSettings.asset";
        private const string SCENE_NAME_PLACE = "SCENE_NAME";

        private const string EXIT_PLAY_MODE = "Exit Play Mode";
        private const string RETURN_TO_PREVIOUS_BUTTON = " <- |Return| <- ";
        private const string SETTINGS_BUTTON = "Open Settings";
        private const string ENABLE_CUSTOM_START_SCENE = "Enable Custom Start Scene";

        private string[] scenes;
        private string CurrentScene => EditorSceneManager.GetActiveScene().path;
        private string Previous;
        
        private SceneAsset customPlayModeStartScene;
        private string customPlayModeStartScenePath;

        private Settings CurrentSettings => settingsAsset.Settings;
        private SceneSwitcherSettingsScriptableObject settingsAsset;

        private static bool CustomStartSceneEnabledInProject => EditorSceneManager.playModeStartScene != null;

        [MenuItem("Tools/Scene Switcher")]
        private static void ShowWindow()
        {
            GetWindow(typeof(SceneSwitcher));
        }

        private void Awake()
        {
            if (!LoadSettings())
            {
                CreateNewSettingsAsset();
                LoadSettings();
            }
            
            CollectScenes();
            
            if (CurrentSettings.AutoEnableCustomPlayModeStartScene && !CustomStartSceneEnabledInProject)
            {
                SwitchCustomStartSceneEnabled();
            }
        }

        private void OnEnable()
        {
            EditorBuildSettings.sceneListChanged += CollectScenes;
            AssetChangeListener.AssetsWereChanged += CollectScenes;

        }

        private void OnDisable()
        {
            EditorBuildSettings.sceneListChanged -= CollectScenes;
            AssetChangeListener.AssetsWereChanged -= CollectScenes;
        }

        private void OnGUI()
        {
            if (EditorApplication.isPlaying)
            {
                if (GUILayout.Button(EXIT_PLAY_MODE, GUILayout.Height(position.height), GUILayout.Width(position.width)))
                {
                    EditorApplication.ExitPlaymode();
                }
                return;
            }

            if(CurrentSettings.ShowReturnToPreviousButton)
            {
                DrawReturnToPrevious();
                GUILayout.Space(CurrentSettings.SpaceAfterReturnButton);
            }
            
            DrawSceneButtons();
        }

        private void DrawReturnToPrevious()
        {
            if(Previous == "") return;
            
            GUILayout.Space(10);
            
            if(GUILayout.Button(RETURN_TO_PREVIOUS_BUTTON, GUILayout.Height(CurrentSettings.ReturnButtonHeight)))
            {
                SwitchTo(Previous);
            }
        }

        private void DrawSceneButtons()
        {
            if (scenes.Length == 0)
            {
                EditorGUILayout.HelpBox("   ZERO SCENES FOUND/SELECTED", MessageType.Info); 
            }
            
            for (int i = 0; i < scenes.Length; i++)
            {
                string scenePath = scenes[i];
                
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();

                string buttonText = Path.GetFileNameWithoutExtension(scenePath);

                //add play mode start scene indicator
                if (EditorSceneManager.playModeStartScene != null && scenePath == customPlayModeStartScenePath)
                {
                    buttonText = CurrentSettings.CustomStartSceneLabelFormatting.Replace(SCENE_NAME_PLACE, buttonText);
                }
                //add current scene indicator 
                if (scenePath == CurrentScene)
                {
                    buttonText = CurrentSettings.CurrentSceneButtonFormatting.Replace(SCENE_NAME_PLACE, buttonText);
                }

                if (GUILayout.Button(buttonText, GUILayout.Width(position.width),
                        GUILayout.Height(CurrentSettings.SceneButtonHeight)))
                {
                    SwitchTo(scenePath);
                }

                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                
                if(i < scenes.Length - 1)
                {
                    GUILayout.Space(CurrentSettings.SpaceBetweenSceneButtons);
                }
            }
        }

        private void SwitchCustomStartSceneEnabled()
        {
            if (!CustomStartSceneEnabledInProject)
            {
                EditorSceneManager.playModeStartScene = customPlayModeStartScene;
            }
            else 
            {
                EditorSceneManager.playModeStartScene = null;
            }
        }

        private void OpenSettings()
        {
            Selection.activeObject = settingsAsset;
        }

        private void CollectScenes()
        {
            customPlayModeStartScenePath = EditorBuildSettings.scenes[CurrentSettings.CustomPlayModeStartSceneBuildIndex].path;
            customPlayModeStartScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(customPlayModeStartScenePath);
            
            switch(CurrentSettings.WhichScenesCollect)
            {
                case Settings.Collect.OnlyFromBuild:
                    scenes = new string[EditorBuildSettings.scenes.Length];
                    for (int i = 0; i < scenes.Length; i++)
                    {
                        scenes[i] = EditorBuildSettings.scenes[i].path;
                    }
                    break;

                case Settings.Collect.CustomList:
                    //clear from nulls
                    CurrentSettings.CustomSceneList.RemoveAll((asset) => asset == null);
                    //clear from duplicates
                    CurrentSettings.CustomSceneList = CurrentSettings.CustomSceneList.Distinct().ToList();

                    scenes = new string[CurrentSettings.CustomSceneList.Count];
                    for (int i = 0; i < CurrentSettings.CustomSceneList.Count; i++)
                    {
                        var sceneAsset = CurrentSettings.CustomSceneList[i];
                        if(sceneAsset == null) continue;

                        scenes[i] = AssetDatabase.GetAssetPath(sceneAsset);
                    }
                    break;
                case Settings.Collect.All:
                    string[] guids = AssetDatabase.FindAssets("t:Scene");
                    scenes = new string[guids.Length];
                    for (int i = 0; i < guids.Length; i++)
                    {
                        scenes[i] = AssetDatabase.GUIDToAssetPath(guids[i]);
                    }
                    break;
                default: throw new ArgumentOutOfRangeException();
            }
        }

        private void SwitchTo(string path)
        {
            if(path == CurrentScene) return;
            
            if (CurrentSettings.SaveSceneSwitch)
            {
                EditorSceneManager.SaveOpenScenes();
            }

            Previous = CurrentScene;
            EditorSceneManager.OpenScene(path);
        }

        private bool LoadSettings()
        {
            settingsAsset = AssetDatabase.LoadAssetAtPath<SceneSwitcherSettingsScriptableObject>(SETTINGS_STORAGE_PATH);
            return settingsAsset != null;
        }

        private bool SaveSettingsAsset()
        {
            if (settingsAsset == null) return false;
            
            EditorUtility.SetDirty(settingsAsset);
            AssetDatabase.SaveAssetIfDirty(settingsAsset);
            return true;
        }

        private static void CreateNewSettingsAsset()
        {
            string[] slicedPath = Path.GetDirectoryName(SETTINGS_STORAGE_PATH)?.Split(Path.DirectorySeparatorChar);

            if(slicedPath != null && slicedPath.Length <= 1)
            {
                Debug.LogError($"[SCENE SWITCHER] CANNOT CREATE SETTINGS ASSET. INVALID PATH: {SETTINGS_STORAGE_PATH}");
            }

            string currentDirectory = slicedPath[0];

            for (int i = 1; i < slicedPath.Length; i++)
            {
                string folder = slicedPath[i];
                string nextDirectory = Path.Join(currentDirectory, folder);
                
                if (!AssetDatabase.IsValidFolder(nextDirectory))
                {
                    AssetDatabase.CreateFolder(currentDirectory, folder);
                }

                currentDirectory = nextDirectory;
            }

            AssetDatabase.Refresh();
            
            AssetDatabase.CreateAsset(CreateInstance<SceneSwitcherSettingsScriptableObject>(),SETTINGS_STORAGE_PATH);
            Debug.Log($"[SCENE SWITCHER] CREATED NEW SCENE SWITCHER SETTINGS ASSET AT {Path.GetDirectoryName(SETTINGS_STORAGE_PATH)}");
        }

        public void AddItemsToMenu(GenericMenu menu)
        {
            GUIContent openSettings = new GUIContent(SETTINGS_BUTTON);
            menu.AddItem(openSettings, false, OpenSettings);

            menu.AddSeparator("");
            
            GUIContent switchStartSceneEnabled = new GUIContent(ENABLE_CUSTOM_START_SCENE);
            menu.AddItem(switchStartSceneEnabled, CustomStartSceneEnabledInProject, SwitchCustomStartSceneEnabled);
            
            menu.AddSeparator("");
            
            //fast collect method switching
            GUIContent onlyFromBuild = new GUIContent("Only From Build");
            GUIContent customList = new GUIContent("Custom List");
            GUIContent all = new GUIContent("All");

            menu.AddItem(onlyFromBuild, CurrentSettings.WhichScenesCollect == Settings.Collect.OnlyFromBuild, SetCollectToOnlyFromBuild);
            menu.AddItem(customList, CurrentSettings.WhichScenesCollect == Settings.Collect.CustomList, SetCollectToCustomList);
            menu.AddItem(all, CurrentSettings.WhichScenesCollect == Settings.Collect.All, SetCollectToAll);
        }

        private void SetCollectToOnlyFromBuild()
        {
            CurrentSettings.WhichScenesCollect = Settings.Collect.OnlyFromBuild;
            SaveSettingsAsset();
        }

        private void SetCollectToCustomList()
        {
            CurrentSettings.WhichScenesCollect = Settings.Collect.CustomList;
            SaveSettingsAsset();
        }

        private void SetCollectToAll()
        {
            CurrentSettings.WhichScenesCollect = Settings.Collect.All;
            SaveSettingsAsset();
        }

        [Serializable]
        public class Settings
        {
            public Collect WhichScenesCollect = Collect.OnlyFromBuild; 
            public bool ShowReturnToPreviousButton = false;

            public bool AutoEnableCustomPlayModeStartScene = false;
            public int CustomPlayModeStartSceneBuildIndex = 0;
            public bool SaveSceneSwitch = true;

            public List<SceneAsset> CustomSceneList;

            [Range(15, 50)] public int ReturnButtonHeight = 20;
            [Range(15, 50)] public int SceneButtonHeight = 20;

            [Range(0, 20)] public int SpaceAfterReturnButton = 10;
            [Range(0, 20)] public int SpaceBetweenSceneButtons = 0;
            
            public string CurrentSceneButtonFormatting = "> SCENE_NAME <"; 
            public string CustomStartSceneLabelFormatting = "(PM) SCENE_NAME"; 
            
            public enum Collect 
            {
                OnlyFromBuild,
                CustomList,
                All
            }
        }
    }
}
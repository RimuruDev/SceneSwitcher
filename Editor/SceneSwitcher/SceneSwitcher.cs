// **************************************************************** //
//
//   Copyright (c) RimuruDev. All rights reserved.
//   Contact me:
//          - Gmail:    rimuru.dev@gmail.com
//          - LinkedIn: https://www.linkedin.com/in/rimuru/
//          - Gists:    https://gist.github.com/RimuruDev/af759ce6d9768a38f6838d8b7cc94fc8
//          - GitHub:   https://github.com/RimuruDev
//
// **************************************************************** //

#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;

namespace AbyssMoth.External.RimuruDevUtils.Editor.SceneSwitcher
{
    public sealed class SceneSwitcher : EditorWindow
    {
        private const string CtrlF2 = "%#F2";
        private const string FindAssets = "t:Scene";
        private const string logFormat = "<color=yellow>{0}</color>";

        private bool showAllScenes;
        private bool autoSaveEnabled = true;
        private bool settingsFoldout = true;
        private bool showDebugLog;
        private bool compactButtons;
        private Vector2 scrollPosition;

        [MenuItem("RimuruDev Tools/Scene Switcher " + CtrlF2)]
        private static void ShowWindow()
        {
            GetWindow<SceneSwitcher>();
        }

        private void OnGUI()
        {
            GUILayout.Label("Scene Switcher", EditorStyles.boldLabel);

            settingsFoldout = EditorGUILayout.Foldout(settingsFoldout, "Settings");
           
            if (settingsFoldout)
            {
                EditorGUI.indentLevel++;
                {
                    showAllScenes = EditorGUILayout.Toggle("Show Absolutely All Scenes", showAllScenes);
                    autoSaveEnabled = EditorGUILayout.Toggle("Enable Auto Save", autoSaveEnabled);
                    showDebugLog = EditorGUILayout.Toggle("Show Debug Log", showDebugLog);
                    compactButtons = EditorGUILayout.Toggle("Compact Buttons", compactButtons);
                }
                EditorGUI.indentLevel--;
            }

            scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, true, GUILayout.Width(350), GUILayout.Height(350));

            var scenePaths = showAllScenes
                ? GetAllScenePaths()
                : GetScenePathsByBuildSettings();
           
            var buttonWidth = compactButtons 
                ? 200
                : 300;

            foreach (var scenePath in scenePaths)
            {
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();

                if (GUILayout.Button(Path.GetFileNameWithoutExtension(scenePath), GUILayout.Width(buttonWidth)))
                {
                    if (autoSaveEnabled && EditorSceneManager.SaveOpenScenes())
                    {
                        if (showDebugLog)
                            Debug.LogFormat(logFormat, "Scenes saved!");
                    }

                    EditorSceneManager.OpenScene(scenePath);
                }

                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }

            GUILayout.EndScrollView();
        }

        private static string[] GetScenePathsByBuildSettings()
        {
            var scenes = EditorBuildSettings.scenes;
            var paths = new string[scenes.Length];

            for (var i = 0; i < scenes.Length; i++)
                paths[i] = scenes[i].path;

            return paths;
        }

        private static string[] GetAllScenePaths()
        {
            var guids = AssetDatabase.FindAssets(FindAssets);
            var scenePaths = new string[guids.Length];

            for (var i = 0; i < guids.Length; i++)
                scenePaths[i] = AssetDatabase.GUIDToAssetPath(guids[i]);

            return scenePaths;
        }
    }
}
#endif
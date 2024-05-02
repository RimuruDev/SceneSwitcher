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

#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;

namespace AbyssMoth.External.RimuruDevUtils.Editor.SceneSwitcher
{
    public sealed class SceneSwitcher : EditorWindow
    {
        private const string СtrlF2 = "%#F2";
        private const string FindAssets = "t:Scene";
        private const string logFormat = "<color=yellow>{0}</color>";

        private bool showAllScenes;
        private bool autoSaveEnabled = true;
        private bool settingsFoldout = true;
        private bool showDebugLog;
        private Vector2 scrollPosition;

        [MenuItem("RimuruDev Tools/Scene Switcher " + СtrlF2)]
        private static void ShowWindow()
        {
            GetWindow(typeof(SceneSwitcher));
        }

        private void OnGUI()
        {
            GUILayout.Label("Scene Switcher", EditorStyles.boldLabel);

            settingsFoldout = EditorGUILayout.Foldout(settingsFoldout, "Settings");
            if (settingsFoldout)
            {
                EditorGUI.indentLevel++;
                showAllScenes = EditorGUILayout.Toggle("Show Absolutely All Scenes", showAllScenes);
                autoSaveEnabled = EditorGUILayout.Toggle("Enable Auto Save", autoSaveEnabled);
                showDebugLog = EditorGUILayout.Toggle("Show Debug Log", showDebugLog);
                EditorGUI.indentLevel--;
            }

            scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(400), GUILayout.Height(600));
            var scenePaths = showAllScenes ? GetAllScenePaths() : GetScenePathsByBuildSettings();

            foreach (var scenePath in scenePaths)
            {
                if (GUILayout.Button(Path.GetFileNameWithoutExtension(scenePath)))
                {
                    if (autoSaveEnabled && EditorSceneManager.SaveOpenScenes())
                    {
                        if (showDebugLog)
                            Debug.LogFormat(logFormat, "Scenes saved!");
                    }

                    EditorSceneManager.OpenScene(scenePath);
                }
            }

            GUILayout.EndScrollView();
        }

        private static string[] GetScenePathsByBuildSettings()
        {
            var paths = new string[EditorBuildSettings.scenes.Length];

            for (var i = 0; i < EditorBuildSettings.scenes.Length; i++)
                paths[i] = EditorBuildSettings.scenes[i].path;

            return paths;
        }

        private static string[] GetAllScenePaths()
        {
            var guids = AssetDatabase.FindAssets(FindAssets);

            var scenePaths = new string[guids.Length];

            for (var i = 0; i < scenePaths.Length; i++)
            {
                var path = AssetDatabase.GUIDToAssetPath(guids[i]);
                scenePaths[i] = path;
            }

            return scenePaths;
        }
    }
}
#endif
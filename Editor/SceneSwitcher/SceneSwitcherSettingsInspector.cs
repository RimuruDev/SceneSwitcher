using System.Linq;
using UnityEditor;
using UnityEngine;

namespace RimuruDevUtils.SceneSwitcher
{
    [CustomEditor(typeof(SceneSwitcherSettingsScriptableObject))]
    public class SceneSwitcherSettingsInspector : UnityEditor.Editor
    {
    private SerializedProperty settingsProp;

        private SerializedProperty whichScenesCollectProp;
        private SerializedProperty showReturnButtonProp;
        private SerializedProperty customPlayModeStartSceneBuildIndexAutoEnableProp;
        private SerializedProperty customPlayModeStartSceneBuildIndexProp;
        private SerializedProperty saveSceneSwitchProp;
        private SerializedProperty customSceneListProp;

        private SerializedProperty returnButtonHeightProp;
        private SerializedProperty sceneButtonHeightProp;

        private SerializedProperty spaceAfterReturnButtonProp;
        private SerializedProperty spaceBetweenSceneButtonsProp;

        private SerializedProperty currentSceneButtonFormattingProp;
        private SerializedProperty customPlayModeStartSceneLabelFormattingProp;

        private bool behaviourFoldout = true;
        private bool styleFoldout = true;

        private void OnEnable()
        {
            // Get the Settings property
            settingsProp = serializedObject.FindProperty("Settings");
    
            // Find all serialized fields inside Settings
            whichScenesCollectProp = settingsProp.FindPropertyRelative("WhichScenesCollect");
            showReturnButtonProp = settingsProp.FindPropertyRelative("ShowReturnToPreviousButton");
            customPlayModeStartSceneBuildIndexAutoEnableProp = settingsProp.FindPropertyRelative("AutoEnableCustomPlayModeStartScene");
            customPlayModeStartSceneBuildIndexProp = settingsProp.FindPropertyRelative("CustomPlayModeStartSceneBuildIndex");
            saveSceneSwitchProp = settingsProp.FindPropertyRelative("SaveSceneSwitch");
            customSceneListProp = settingsProp.FindPropertyRelative("CustomSceneList");
    
            returnButtonHeightProp = settingsProp.FindPropertyRelative("ReturnButtonHeight");
            sceneButtonHeightProp = settingsProp.FindPropertyRelative("SceneButtonHeight");
    
            spaceAfterReturnButtonProp = settingsProp.FindPropertyRelative("SpaceAfterReturnButton");
            spaceBetweenSceneButtonsProp = settingsProp.FindPropertyRelative("SpaceBetweenSceneButtons");
    
            currentSceneButtonFormattingProp = settingsProp.FindPropertyRelative("CurrentSceneButtonFormatting");
            customPlayModeStartSceneLabelFormattingProp = settingsProp.FindPropertyRelative("CustomStartSceneLabelFormatting");
        }
    
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
    
            SceneSwitcherSettingsScriptableObject settingsObject = target as SceneSwitcherSettingsScriptableObject;
            
            EditorGUILayout.LabelField("Scene Switcher Settings", EditorStyles.boldLabel);

            behaviourFoldout = EditorGUILayout.Foldout(behaviourFoldout, "Behaviour", EditorStyles.foldout);
            if(behaviourFoldout)
            {
                EditorGUILayout.PropertyField(whichScenesCollectProp);

                if (settingsObject.Settings.WhichScenesCollect == SceneSwitcher.Settings.Collect.CustomList)
                {
                    //draw list
                    EditorGUILayout.PropertyField(customSceneListProp, true);

                    //draw some handy buttons
                    if (GUILayout.Button("Add All Scenes From Project"))
                    {
                        string[] guids = AssetDatabase.FindAssets("t:Scene");
                        foreach (string guid in guids)
                        {
                            settingsObject.Settings.CustomSceneList.Add(AssetDatabase.LoadAssetAtPath<SceneAsset>(AssetDatabase.GUIDToAssetPath(guid)));
                        }
                        EditorUtility.SetDirty(settingsObject);
                    }
                    if (GUILayout.Button("Add Scenes From Build"))
                    {
                        foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
                        {
                            settingsObject.Settings.CustomSceneList.Add(AssetDatabase.LoadAssetAtPath<SceneAsset>(scene.path));
                        }
                        EditorUtility.SetDirty(settingsObject);
                    }
                    if (GUILayout.Button("Clear List"))
                    {
                        settingsObject.Settings.CustomSceneList.Clear();
                        EditorUtility.SetDirty(settingsObject);
                    }

                    //check if contains nulls
                    if (settingsObject.Settings.CustomSceneList.Any((asset) => asset == null))
                    {
                        EditorGUILayout.HelpBox("LIST CONTAINS NULL ELEMENTS. THEY WILL BE REMOVED", MessageType.Warning);
                        EditorGUILayout.Separator();
                    }
                    //check if contains duplicates
                    bool hasDuplicates = settingsObject.Settings.CustomSceneList.GroupBy(x => x)
                        .Any(g => g.Count() > 1);

                    if (hasDuplicates)
                    {
                        EditorGUILayout.HelpBox("LIST CONTAINS DUPLICATES. THEY WILL BE REMOVED", MessageType.Warning);
                        EditorGUILayout.Separator();
                    }
                }
                EditorGUILayout.PropertyField(showReturnButtonProp);

                EditorGUILayout.PropertyField(customPlayModeStartSceneBuildIndexProp);
                EditorGUILayout.PropertyField(customPlayModeStartSceneBuildIndexAutoEnableProp);

                EditorGUILayout.PropertyField(saveSceneSwitchProp);

            }
            
            styleFoldout = EditorGUILayout.Foldout(styleFoldout, "Button Style");
            if(styleFoldout)
            {
                EditorGUILayout.IntSlider(returnButtonHeightProp, 15, 50, new GUIContent("Return Button Height"));
                EditorGUILayout.IntSlider(spaceAfterReturnButtonProp, 0, 20, new GUIContent("Space After Return To Previous Button"));
                
                EditorGUILayout.Separator();
                
                EditorGUILayout.IntSlider(sceneButtonHeightProp, 15, 50, new GUIContent("Scene Button Height"));
                EditorGUILayout.IntSlider(spaceBetweenSceneButtonsProp, 0, 20, new GUIContent("Space Between Scene Buttons"));

                EditorGUILayout.Separator();

                EditorGUILayout.PropertyField(currentSceneButtonFormattingProp);
                EditorGUILayout.PropertyField(customPlayModeStartSceneLabelFormattingProp);
            }
            
            serializedObject.ApplyModifiedProperties();
        }
    }
}
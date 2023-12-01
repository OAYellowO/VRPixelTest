using UnityEditor;
using UnityEngine;

namespace Unity.XR.OpenXR.Features.PICOSupport
{
    [CustomEditor(typeof(PICOFeature))]
    internal class PICOFeatureEditor : Editor
    {
        void OnEnable()
        {

        }

        public override void OnInspectorGUI()
        {
            PICOFeature picoFeature = (PICOFeature)target;
            PICOProjectSetting projectConfig = PICOProjectSetting.GetProjectConfig();
            EditorGUIUtility.labelWidth = 180;
            //eye tracking
            GUIStyle firstLevelStyle = new GUIStyle(GUI.skin.label);
            firstLevelStyle.alignment = TextAnchor.UpperLeft;
            firstLevelStyle.fontStyle = FontStyle.Bold;
            firstLevelStyle.fontSize = 12;
            firstLevelStyle.wordWrap = true;
            var guiContent = new GUIContent();
            guiContent.text = "Eye Tracking";
            guiContent.tooltip = "Before calling EyeTracking API, enable this option first, only for Neo3 Pro Eye , PICO 4 Pro device.";
            projectConfig.isEyeTracking = EditorGUILayout.Toggle(guiContent, projectConfig.isEyeTracking);
            if (projectConfig.isEyeTracking)
            {
                projectConfig.isEyeTrackingCalibration = EditorGUILayout.Toggle(new GUIContent("Eye Tracking Calibration"), projectConfig.isEyeTrackingCalibration);
                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.LabelField("Note:  Eye Tracking is supported only on Neo 3 Pro Eye , PICO 4 Pro", firstLevelStyle);
                EditorGUILayout.EndVertical();
            }
            projectConfig.isHandTracking = EditorGUILayout.Toggle("Hand Tracking", projectConfig.isHandTracking);

            projectConfig.isCameraSubsystem = EditorGUILayout.Toggle("Camera Subsystem", projectConfig.isCameraSubsystem);
            picoFeature.isCameraSubsystem = projectConfig.isCameraSubsystem;
            var displayFrequencyContent = new GUIContent();
            displayFrequencyContent.text = "Display Refresh Rates";
            projectConfig.displayFrequency = (SystemDisplayFrequency)EditorGUILayout.EnumPopup(displayFrequencyContent, projectConfig.displayFrequency);

            // content protect
            projectConfig.useContentProtect = EditorGUILayout.Toggle("Use Content Protect", projectConfig.useContentProtect);
            
            GUILayout.BeginHorizontal();
            guiContent.text = "System Splash Screen";
            guiContent.tooltip = "";
            EditorGUILayout.LabelField(guiContent, GUILayout.Width(165));
            projectConfig.systemSplashScreen = (Texture2D)EditorGUILayout.ObjectField(projectConfig.systemSplashScreen, typeof(Texture2D), true);
            GUILayout.EndHorizontal();

            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Note:  Set the system splash screen picture in PNG format.", firstLevelStyle);
            EditorGUILayout.EndVertical();


            serializedObject.Update();
            if (GUI.changed)
            {
                EditorUtility.SetDirty(projectConfig);
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}

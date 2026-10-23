using System;
using System.IO;
using ProblematicExercise.Runtime;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace ProblematicExercise.Editor
{
    public static class FirstDayTools
    {
        public const string ScenePath = "Assets/ProblematicExercise/Scenes/FirstDay.unity";

        [MenuItem("Problematic Exercise/Prepare First Day")]
        public static void Prepare()
        {
            FirstVisitTools.Prepare();
            if (!File.Exists(ScenePath))
            {
                if (!Application.isBatchMode && !EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
                var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
                var camera = new GameObject("Camera", typeof(Camera)).GetComponent<Camera>();
                camera.clearFlags = CameraClearFlags.SolidColor;
                camera.backgroundColor = new Color32(16, 29, 27, 255);
                camera.orthographic = true;
                var view = new GameObject("FirstDay", typeof(FirstVisitView)).GetComponent<FirstVisitView>();
                var serialized = new SerializedObject(view);
                serialized.FindProperty("dailyMode").boolValue = true;
                serialized.ApplyModifiedPropertiesWithoutUndo();
                EditorSceneManager.SaveScene(scene, ScenePath);
                AssetDatabase.Refresh();
            }
            Debug.Log("FIRST_DAY_PREPARED " + ScenePath);
        }

        [MenuItem("Problematic Exercise/Open First Day")]
        public static void Open()
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
            Prepare();
            EditorSceneManager.OpenScene(ScenePath);
        }

        public static void BuildWindows()
        {
            Prepare();
            string output = Environment.GetEnvironmentVariable("PE_FIRST_DAY_BUILD");
            if (string.IsNullOrWhiteSpace(output)) throw new InvalidOperationException("PE_FIRST_DAY_BUILD must name a private output directory.");
            Directory.CreateDirectory(output);
            var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
            {
                scenes = new[] { ScenePath },
                locationPathName = Path.Combine(output, "ProblematicExercise.exe"),
                target = BuildTarget.StandaloneWindows64,
                options = BuildOptions.Development
            });
            File.WriteAllText(Path.Combine(output, "build-result.txt"), report.summary.result + "\nErrors=" + report.summary.totalErrors);
            if (report.summary.result != BuildResult.Succeeded) throw new InvalidOperationException("First day build failed: " + report.summary.result);
        }
    }
}

using System;
using System.IO;
using ProblematicExercise.Runtime;
using TMPro;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ProblematicExercise.Editor
{
    public static class FirstVisitTools
    {
        public const string ScenePath = "Assets/ProblematicExercise/Scenes/FirstVisit.unity";
        private static double preparationDeadline;

        // TMP's package import completes on subsequent Editor updates. This entry
        // point runs without -quit and exits only after the imported font is loadable.
        public static void PrepareBatch()
        {
            preparationDeadline = EditorApplication.timeSinceStartup + 90;
            SessionState.SetBool("PE.FirstVisit.Preparing", true);
            SessionState.SetFloat("PE.FirstVisit.Deadline", (float)preparationDeadline);
            EditorApplication.update += CompleteBatchPreparation;
            Prepare();
        }

        [InitializeOnLoadMethod]
        private static void ResumeBatchPreparation()
        {
            if (!Application.isBatchMode || !SessionState.GetBool("PE.FirstVisit.Preparing", false)) return;
            preparationDeadline = SessionState.GetFloat("PE.FirstVisit.Deadline", 0);
            EditorApplication.update -= CompleteBatchPreparation;
            EditorApplication.update += CompleteBatchPreparation;
        }

        private static void CompleteBatchPreparation()
        {
            var importedFont = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/TextMesh Pro/Resources/Fonts & Materials/LiberationSans SDF.asset");
            if (importedFont != null && !EditorApplication.isCompiling && !EditorApplication.isUpdating)
            {
                EditorApplication.update -= CompleteBatchPreparation;
                SessionState.SetBool("PE.FirstVisit.Preparing", false);
                Debug.Log("FIRST_VISIT_FONT_READY " + AssetDatabase.GetAssetPath(importedFont));
                EditorApplication.Exit(0);
            }
            else if (EditorApplication.timeSinceStartup > preparationDeadline)
            {
                EditorApplication.update -= CompleteBatchPreparation;
                SessionState.SetBool("PE.FirstVisit.Preparing", false);
                Debug.LogError("Timed out waiting for TMP essentials import.");
                EditorApplication.Exit(1);
            }
        }

        [MenuItem("Problematic Exercise/Prepare First Visit")]
        public static void Prepare()
        {
            if (!File.Exists("Assets/TextMesh Pro/Resources/TMP Settings.asset"))
            {
                TMP_PackageResourceImporter.ImportResources(true, false, false);
                AssetDatabase.Refresh();
            }
            if (!File.Exists(ScenePath))
            {
                if (!Application.isBatchMode && !EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
                Directory.CreateDirectory(Path.GetDirectoryName(ScenePath));
                var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
                var camera = new GameObject("Camera", typeof(Camera)).GetComponent<Camera>();
                camera.clearFlags = CameraClearFlags.SolidColor;
                camera.backgroundColor = new Color32(16, 29, 27, 255);
                camera.orthographic = true;
                new GameObject("FirstVisit", typeof(FirstVisitView));
                EditorSceneManager.SaveScene(scene, ScenePath);
                AssetDatabase.Refresh();
            }
            Debug.Log("FIRST_VISIT_PREPARED " + ScenePath);
        }

        [MenuItem("Problematic Exercise/Open First Visit")]
        public static void Open()
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
            Prepare();
            EditorSceneManager.OpenScene(ScenePath);
        }

        // Only called explicitly by the validation command; never changes global BuildSettings.
        public static void BuildWindows()
        {
            Prepare();
            string output = Environment.GetEnvironmentVariable("PE_FIRST_VISIT_BUILD");
            if (string.IsNullOrWhiteSpace(output)) throw new InvalidOperationException("PE_FIRST_VISIT_BUILD must name a private build directory.");
            Directory.CreateDirectory(output);
            var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
            {
                scenes = new[] { ScenePath },
                locationPathName = Path.Combine(output, "ProblematicExercise.exe"),
                target = BuildTarget.StandaloneWindows64,
                options = BuildOptions.Development
            });
            File.WriteAllText(Path.Combine(output, "build-result.txt"), report.summary.result + "\nErrors=" + report.summary.totalErrors);
            if (report.summary.result != BuildResult.Succeeded) throw new InvalidOperationException("First visit build failed: " + report.summary.result);
        }
    }
}

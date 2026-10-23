using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace ProblematicExercise.Editor
{
    public static class CampaignTools
    {
        public const string ScenePath = "Assets/ProblematicExercise/Scenes/Campaign.unity";

        [MenuItem("Problematic Exercise/Open Campaign")]
        public static void Open()
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
            Prepare();
            EditorSceneManager.OpenScene(ScenePath);
        }

        public static void Prepare()
        {
            FirstDayTools.Prepare();
            if (File.Exists(ScenePath)) return;
            if (!AssetDatabase.CopyAsset(FirstDayTools.ScenePath, ScenePath)) throw new InvalidOperationException("Cannot create campaign scene.");
            var scene = EditorSceneManager.OpenScene(ScenePath);
            foreach (var root in scene.GetRootGameObjects())
            {
                var view = root.GetComponent<Runtime.FirstVisitView>();
                if (view == null) continue;
                var serialized = new SerializedObject(view);
                serialized.FindProperty("campaignMode").boolValue = true;
                serialized.ApplyModifiedPropertiesWithoutUndo();
                EditorSceneManager.SaveScene(scene);
            }
            Debug.Log("CAMPAIGN_PREPARED " + ScenePath);
        }

        public static void BuildWindows()
        {
            Prepare();
            string output = Environment.GetEnvironmentVariable("PE_CAMPAIGN_BUILD");
            if (string.IsNullOrWhiteSpace(output)) throw new InvalidOperationException("PE_CAMPAIGN_BUILD must name a private output directory.");
            Directory.CreateDirectory(output);
            var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
            {
                scenes = new[] { ScenePath }, locationPathName = Path.Combine(output, "ProblematicExercise.exe"),
                target = BuildTarget.StandaloneWindows64, options = BuildOptions.Development
            });
            File.WriteAllText(Path.Combine(output, "build-result.txt"), report.summary.result + "\nErrors=" + report.summary.totalErrors);
            if (report.summary.result != BuildResult.Succeeded) throw new InvalidOperationException("Campaign build failed: " + report.summary.result);
        }
    }
}

using System;
using System.Collections;
using System.IO;
using NUnit.Framework;
using ProblematicExercise.Domain;
using ProblematicExercise.Runtime;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.Rendering;
using UnityEngine.TestTools;

namespace ProblematicExercise.Tests
{
    public class FirstVisitInteractionTests
    {
        private Mouse mouse;
        private FirstVisitView view;
        private GameObject root;
        private InputSettings.BackgroundBehavior oldBackgroundBehavior;
        private InputSettings.EditorInputBehaviorInPlayMode oldEditorBehavior;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            // Only the test's in-memory input routing changes, restored in teardown.
            // Batch Editors have no focused Game View and otherwise discard UI input.
            oldBackgroundBehavior = InputSystem.settings.backgroundBehavior;
            oldEditorBehavior = InputSystem.settings.editorInputBehaviorInPlayMode;
            InputSystem.settings.backgroundBehavior = InputSettings.BackgroundBehavior.IgnoreFocus;
            InputSystem.settings.editorInputBehaviorInPlayMode = InputSettings.EditorInputBehaviorInPlayMode.AllDeviceInputAlwaysGoesToGameView;
            mouse = InputSystem.AddDevice<Mouse>();
            mouse.MakeCurrent();
            root = new GameObject("FirstVisitTest", typeof(FirstVisitView));
            view = root.GetComponent<FirstVisitView>();
            yield return null;
            yield return null;
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            UnityEngine.Object.Destroy(root);
            if (mouse != null && mouse.added) InputSystem.RemoveDevice(mouse);
            InputSystem.settings.backgroundBehavior = oldBackgroundBehavior;
            InputSystem.settings.editorInputBehaviorInPlayMode = oldEditorBehavior;
            yield return null;
        }

        private GameObject Control(string name)
        {
            var controls = root.GetComponentsInChildren<RectTransform>(true);
            foreach (var control in controls)
                if (control.name == name) return control.gameObject;
            throw new InvalidOperationException("Missing control " + name);
        }

        private Vector2 Center(string name) => RectTransformUtility.WorldToScreenPoint(null,
            ((RectTransform)Control(name).transform).TransformPoint(((RectTransform)Control(name).transform).rect.center));

        private IEnumerator Pointer(Vector2 position, bool down)
        {
            InputSystem.QueueStateEvent(mouse, new MouseState { position = position, buttons = down ? (ushort)1 : (ushort)0 });
            yield return null;
            yield return null;
        }

        private IEnumerator Click(string name)
        {
            Vector2 point = Center(name);
            yield return Pointer(point, false);
            yield return Pointer(point, true);
            yield return Pointer(point, false);
        }

        private IEnumerator ToHand()
        {
            yield return Capture("start");
            yield return Click("StartButton");
            Assert.AreEqual(VisitPhase.ChoosingClient, view.Model.Phase);
            yield return Click("ClientButton0");
            Assert.AreEqual(VisitPhase.Entering, view.Model.Phase);
            yield return new WaitForSecondsRealtime(1f);
            Assert.AreEqual(VisitPhase.ChoosingExercise, view.Model.Phase);
            Assert.AreEqual(3, view.Model.Hand.Count);
        }

        [UnityTest]
        public IEnumerator RealUiCompletesVisitWithoutDuplicateScore()
        {
            yield return ToHand();
            yield return Capture("hand");
            int cost = VisitRules.BaseCost(view.Model.Hand[0].Rarity);
            yield return Click("ExerciseCard0");
            Assert.IsNotNull(view.Model.Selected);
            Assert.AreEqual(100, view.Model.Stamina);
            yield return Click("ConfirmButton");
            Assert.AreEqual(VisitPhase.Activity, view.Model.Phase);
            Assert.AreEqual(100 - cost, view.Model.Stamina);
            Assert.AreEqual(3, view.Model.Deck.DiscardCount);
            yield return new WaitForSecondsRealtime(.2f);
            yield return Capture("activity");
            yield return Click("FinishActivityButton");
            Assert.AreEqual(VisitPhase.Result, view.Model.Phase);
            Assert.AreEqual(1, view.Model.ResultCount);
            int points = view.Model.TotalPoints;
            Assert.Greater(points, 0);
            yield return Capture("result");
            yield return Click("ContinueButton");
            yield return new WaitForSecondsRealtime(1f);
            Assert.AreEqual(VisitPhase.Complete, view.Model.Phase);
            Assert.AreEqual(points, view.Model.TotalPoints);
            Assert.AreEqual(1, view.Model.ResultCount);
            yield return Capture("complete");
            yield return Click("RestartButton");
            Assert.AreEqual(VisitPhase.Start, view.Model.Phase);
            Assert.AreEqual(0, view.Model.TotalPoints);
        }

        [UnityTest]
        public IEnumerator RealDragBelowReturnsAndAboveConfirms()
        {
            yield return ToHand();
            Vector2 home = Center("ExerciseCard0");
            yield return Pointer(home, true);
            yield return Pointer(home + Vector2.up * 12, true);
            yield return Pointer(home + Vector2.up * 12, false);
            Assert.AreEqual(VisitPhase.ChoosingExercise, view.Model.Phase);
            Assert.AreEqual(100, view.Model.Stamina);
            Assert.Less(Vector2.Distance(home, Center("ExerciseCard0")), 1f);
            yield return Pointer(home, true);
            yield return Pointer(home + Vector2.up * 12, true);
            yield return Pointer(new Vector2(home.x, Screen.height * .6f), true);
            yield return Pointer(new Vector2(home.x, Screen.height * .6f), false);
            Assert.AreEqual(VisitPhase.Activity, view.Model.Phase);
            Assert.AreEqual(0, view.Model.Hand.Count);
            Assert.AreEqual(3, view.Model.Deck.DiscardCount);
        }

        private IEnumerator Capture(string label)
        {
            string output = Environment.GetEnvironmentVariable("PE_FIRST_VISIT_EVIDENCE");
            if (string.IsNullOrEmpty(output)) yield break;
            Directory.CreateDirectory(output);
            // Batch Editor has no presented Game View/backbuffer. Render the actual
            // Canvas offscreen, then restore Overlay before any pointer interaction.
            var canvas = root.GetComponentInChildren<Canvas>();
            var cameraObject = new GameObject("EvidenceCamera", typeof(Camera));
            var camera = cameraObject.GetComponent<Camera>();
            camera.enabled = false;
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = Color.black;
            camera.transform.position = new Vector3(0, 0, -10);
            var texture = new RenderTexture(Screen.width, Screen.height, 24);
            texture.Create();
            camera.targetTexture = texture;
            var previousTarget = RenderTexture.active;
            var oldMode = canvas.renderMode;
            var oldCamera = canvas.worldCamera;
            float oldDistance = canvas.planeDistance;
            Texture2D capture = null;
            try
            {
                canvas.renderMode = RenderMode.ScreenSpaceCamera;
                canvas.worldCamera = camera;
                canvas.planeDistance = 1f;
                Canvas.ForceUpdateCanvases();
                RenderPipeline.SubmitRenderRequest(camera, new RenderPipeline.StandardRequest { destination = texture });
                RenderTexture.active = texture;
                capture = new Texture2D(texture.width, texture.height, TextureFormat.RGB24, false);
                capture.ReadPixels(new Rect(0, 0, texture.width, texture.height), 0, 0);
                capture.Apply();
                File.WriteAllBytes(Path.Combine(output, label + ".png"), capture.EncodeToPNG());
            }
            finally
            {
                RenderTexture.active = previousTarget;
                canvas.renderMode = oldMode;
                canvas.worldCamera = oldCamera;
                canvas.planeDistance = oldDistance;
                camera.targetTexture = null;
                texture.Release();
                UnityEngine.Object.Destroy(texture);
                UnityEngine.Object.Destroy(capture);
                UnityEngine.Object.Destroy(cameraObject);
                Canvas.ForceUpdateCanvases();
            }
            yield return null;
            yield return null;
        }
    }
}

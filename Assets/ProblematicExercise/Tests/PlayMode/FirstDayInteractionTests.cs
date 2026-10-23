using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using ProblematicExercise.Domain;
using ProblematicExercise.Runtime;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.Rendering;
using UnityEngine.TestTools;

namespace ProblematicExercise.Tests
{
    public class FirstDayInteractionTests
    {
        private Mouse mouse;
        private FirstVisitView view;
        private GameObject root;
        private InputSettings.BackgroundBehavior oldBackground;
        private InputSettings.EditorInputBehaviorInPlayMode oldEditorBehavior;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            oldBackground = InputSystem.settings.backgroundBehavior;
            oldEditorBehavior = InputSystem.settings.editorInputBehaviorInPlayMode;
            InputSystem.settings.backgroundBehavior = InputSettings.BackgroundBehavior.IgnoreFocus;
            InputSystem.settings.editorInputBehaviorInPlayMode = InputSettings.EditorInputBehaviorInPlayMode.AllDeviceInputAlwaysGoesToGameView;
            mouse = InputSystem.AddDevice<Mouse>(); mouse.MakeCurrent();
            root = new GameObject("FirstDayTest");
            root.SetActive(false);
            view = root.AddComponent<FirstVisitView>();
            view.ConfigureAsDay();
            root.SetActive(true);
            yield return null; yield return null;
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            UnityEngine.Object.Destroy(root);
            if (mouse != null && mouse.added) InputSystem.RemoveDevice(mouse);
            InputSystem.settings.backgroundBehavior = oldBackground;
            InputSystem.settings.editorInputBehaviorInPlayMode = oldEditorBehavior;
            yield return null;
        }

        private GameObject Control(string name)
        {
            foreach (var item in root.GetComponentsInChildren<RectTransform>(true))
                if (item.name == name) return item.gameObject;
            throw new InvalidOperationException("Missing control " + name);
        }

        private IEnumerator Pointer(Vector2 position, bool down)
        {
            InputSystem.QueueStateEvent(mouse, new MouseState { position = position, buttons = down ? (ushort)1 : (ushort)0 });
            yield return null; yield return null;
        }

        private IEnumerator Click(string name)
        {
            var rect = (RectTransform)Control(name).transform;
            Vector2 point = RectTransformUtility.WorldToScreenPoint(null, rect.TransformPoint(rect.rect.center));
            yield return Pointer(point, false); yield return Pointer(point, true); yield return Pointer(point, false);
        }

        [UnityTest]
        public IEnumerator MenuCreditsThreeVisitsDailyResultAndCleanRestart()
        {
            Assert.IsTrue(view.IsMainMenu);
            int objectsBefore = root.GetComponentsInChildren<Transform>(true).Length;
            yield return Capture("menu");
            yield return Click("CreditsButton");
            Assert.IsTrue(Control("CreditsPanel").activeSelf);
            yield return Click("CreditsBackButton");
            Assert.IsTrue(view.IsMainMenu);
            yield return Click("NewSessionButton");
            var day = view.Day;
            var deck = day.Deck;
            var ids = new HashSet<string>();
            int expectedStamina = 100, expectedPoints = 0;
            for (int visitIndex = 0; visitIndex < 3; visitIndex++)
            {
                Assert.AreEqual(VisitPhase.ChoosingClient, view.Model.Phase);
                Assert.AreSame(deck, view.Model.Deck);
                Assert.AreSame(day, view.Day);
                if (visitIndex == 0) yield return Capture("roster");
                yield return Click("ClientButton0");
                Assert.IsTrue(ids.Add(view.Model.CurrentClient.Id));
                yield return new WaitForSecondsRealtime(1.7f);
                Assert.AreEqual(3, view.Model.Hand.Count);
                int slot = 0;
                for (int i = 1; i < view.Model.Hand.Count; i++)
                    if (view.Model.Hand[i].Rarity < view.Model.Hand[slot].Rarity) slot = i;
                int cost = VisitRules.BaseCost(view.Model.Hand[slot].Rarity);
                if (visitIndex == 0) yield return Capture("hand");
                yield return Click("ExerciseCard" + slot);
                Assert.AreEqual(expectedStamina, day.Resources.Stamina);
                yield return Click("ConfirmButton");
                expectedStamina = Math.Max(0, expectedStamina - cost);
                Assert.AreEqual(expectedStamina, day.Resources.Stamina);
                Assert.AreEqual(0, deck.DrawnCount);
                Assert.AreEqual(VisitPhase.Activity, view.Model.Phase);
                yield return new WaitForSecondsRealtime(.15f);
                if (visitIndex == 0) yield return Capture("activity");
                yield return Click("FinishActivityButton");
                Assert.AreEqual(VisitPhase.Result, view.Model.Phase);
                expectedPoints += view.Model.LastResult.Points;
                Assert.AreEqual(expectedPoints, day.Resources.TotalPoints);
                yield return new WaitForSecondsRealtime(.5f);
                if (visitIndex == 0) yield return Capture("visit-result");
                yield return Click("ContinueButton");
                yield return new WaitForSecondsRealtime(1f);
                Assert.AreEqual(visitIndex + 1, day.CompletedVisits);
                Assert.AreEqual(expectedStamina, day.Resources.Stamina);
                Assert.AreEqual(expectedPoints, day.Resources.TotalPoints);
            }
            Assert.AreEqual(DayPhase.Result, day.Phase);
            Assert.IsTrue(Control("DayResultPanel").activeSelf);
            Assert.AreEqual(EndDayReason.OutOfClients, day.Result.Reason);
            Assert.AreEqual(3, day.Result.Visits.Count);
            StringAssert.Contains("3 de 3", Control("DayStatus").GetComponent<TMP_Text>().text);
            Assert.AreEqual(1, deck.RecycleCount);
            string summary = Control("DailySummary").GetComponent<TMP_Text>().text;
            foreach (var record in day.Result.Visits) StringAssert.Contains(record.ClientName, summary);
            StringAssert.Contains(expectedPoints.ToString(), summary);
            yield return Capture("day-result");
            yield return Click("DayMenuButton");
            Assert.IsTrue(view.IsMainMenu);
            Assert.IsNull(view.Day);
            Assert.AreEqual(objectsBefore, root.GetComponentsInChildren<Transform>(true).Length);
            Assert.AreEqual(1, root.GetComponentsInChildren<Canvas>(true).Length);
            Assert.AreEqual(1, root.GetComponentsInChildren<EventSystem>(true).Length);
            yield return Click("NewSessionButton");
            Assert.AreNotSame(day, view.Day);
            Assert.AreEqual(100, view.Day.Resources.Stamina);
            Assert.AreEqual(0, view.Day.Resources.TotalPoints);
            Assert.AreEqual(0, view.Day.Resources.History.Count);
            Assert.AreEqual(3, view.Day.Roster.Available.Count);
        }

        [UnityTest]
        public IEnumerator FastDragConfirmsWithoutIntermediateMotionFrame()
        {
            yield return Click("NewSessionButton");
            yield return Click("ClientButton0");
            yield return new WaitForSecondsRealtime(.95f);
            Assert.AreEqual(VisitPhase.Entering, view.Model.Phase);
            StringAssert.Contains("Vamos encontrar", Control("Message").GetComponent<TMP_Text>().text);
            Assert.IsFalse(Control("HandPanel").activeSelf);
            yield return new WaitForSecondsRealtime(.75f);
            var rect = (RectTransform)Control("ExerciseCard0").transform;
            Vector2 start = RectTransformUtility.WorldToScreenPoint(null, rect.TransformPoint(rect.rect.center));
            Vector2 finish = new Vector2(start.x, Screen.height * .65f);
            yield return Pointer(start, true);
            // One large OS-like move: BeginDrag and Drag see the same moved sample.
            yield return Pointer(finish, true);
            yield return Pointer(finish, false);
            Assert.AreEqual(VisitPhase.Activity, view.Model.Phase);
            Assert.AreEqual(0, view.Model.Hand.Count);
            Assert.Less(view.Day.Resources.Stamina, 100);
        }

        [UnityTest]
        public IEnumerator ExhaustingStaminaKeepsActivityReactionAndResultBeforeDailyScreen()
        {
            yield return Click("NewSessionButton");
            for (int visitIndex = 0; visitIndex < 3; visitIndex++)
            {
                yield return Click("ClientButton0");
                yield return new WaitForSecondsRealtime(1.7f);
                int slot = 0;
                for (int i = 1; i < view.Model.Hand.Count; i++)
                    if (view.Model.Hand[i].Rarity > view.Model.Hand[slot].Rarity) slot = i;
                yield return Click("ExerciseCard" + slot);
                yield return Click("ConfirmButton");
                Assert.AreEqual(VisitPhase.Activity, view.Model.Phase);
                Assert.IsFalse(Control("DayResultPanel").activeSelf);
                yield return Click("FinishActivityButton");
                Assert.AreEqual(VisitPhase.Result, view.Model.Phase);
                Assert.IsFalse(Control("ContinueButton").GetComponent<UnityEngine.UI.Button>().interactable);
                Assert.AreEqual(visitIndex + 1, view.Day.Resources.History.Count);
                yield return new WaitForSecondsRealtime(.5f);
                Assert.IsTrue(Control("ContinueButton").GetComponent<UnityEngine.UI.Button>().interactable);
                yield return Click("ContinueButton");
                yield return new WaitForSecondsRealtime(1f);
            }
            Assert.AreEqual(0, view.Day.Resources.Stamina);
            Assert.AreEqual(DayPhase.Result, view.Day.Phase);
            Assert.AreEqual(EndDayReason.OutOfStamina, view.Day.Result.Reason);
            yield return Capture("day-stamina-end");
        }

        private IEnumerator Capture(string label)
        {
            string output = Environment.GetEnvironmentVariable("PE_FIRST_DAY_EVIDENCE");
            if (string.IsNullOrEmpty(output)) yield break;
            Directory.CreateDirectory(output);
            var canvas = root.GetComponentInChildren<Canvas>();
            var cameraObject = new GameObject("EvidenceCamera", typeof(Camera));
            var camera = cameraObject.GetComponent<Camera>(); camera.enabled = false;
            camera.clearFlags = CameraClearFlags.SolidColor; camera.backgroundColor = Color.black;
            camera.transform.position = new Vector3(0, 0, -10);
            // Explicit offscreen presentation size; actual pointer tests use Screen.width/height.
            var target = new RenderTexture(1280, 720, 24); target.Create(); camera.targetTexture = target;
            var oldTarget = RenderTexture.active;
            var oldMode = canvas.renderMode; var oldCamera = canvas.worldCamera; float oldDistance = canvas.planeDistance;
            Texture2D capture = null;
            try
            {
                canvas.renderMode = RenderMode.ScreenSpaceCamera; canvas.worldCamera = camera; canvas.planeDistance = 1f;
                Canvas.ForceUpdateCanvases();
                // Let masks update for the temporary camera's viewport before capture.
                yield return null;
                Canvas.ForceUpdateCanvases();
                RenderPipeline.SubmitRenderRequest(camera, new RenderPipeline.StandardRequest { destination = target });
                RenderTexture.active = target;
                capture = new Texture2D(target.width, target.height, TextureFormat.RGB24, false);
                capture.ReadPixels(new Rect(0, 0, target.width, target.height), 0, 0); capture.Apply();
                File.WriteAllBytes(Path.Combine(output, label + ".png"), capture.EncodeToPNG());
            }
            finally
            {
                RenderTexture.active = oldTarget; canvas.renderMode = oldMode; canvas.worldCamera = oldCamera; canvas.planeDistance = oldDistance;
                camera.targetTexture = null; target.Release();
                UnityEngine.Object.Destroy(target); UnityEngine.Object.Destroy(capture); UnityEngine.Object.Destroy(cameraObject);
                Canvas.ForceUpdateCanvases();
            }
            yield return null; yield return null;
        }
    }
}

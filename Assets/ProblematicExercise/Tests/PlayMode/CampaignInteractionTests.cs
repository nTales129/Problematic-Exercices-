using System;
using System.Collections;
using NUnit.Framework;
using ProblematicExercise.Domain;
using ProblematicExercise.Runtime;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.TestTools;

namespace ProblematicExercise.Tests
{
    public class CampaignInteractionTests
    {
        private Mouse mouse;
        private GameObject root;
        private FirstVisitView view;
        private InputSettings.BackgroundBehavior background;
        private InputSettings.EditorInputBehaviorInPlayMode editorInput;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            background = InputSystem.settings.backgroundBehavior;
            editorInput = InputSystem.settings.editorInputBehaviorInPlayMode;
            InputSystem.settings.backgroundBehavior = InputSettings.BackgroundBehavior.IgnoreFocus;
            InputSystem.settings.editorInputBehaviorInPlayMode = InputSettings.EditorInputBehaviorInPlayMode.AllDeviceInputAlwaysGoesToGameView;
            mouse = InputSystem.AddDevice<Mouse>(); mouse.MakeCurrent();
            root = new GameObject("CampaignTest"); root.SetActive(false);
            view = root.AddComponent<FirstVisitView>(); view.ConfigureAsCampaign(); root.SetActive(true);
            yield return null; yield return null;
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            UnityEngine.Object.Destroy(root);
            if (mouse != null && mouse.added) InputSystem.RemoveDevice(mouse);
            InputSystem.settings.backgroundBehavior = background;
            InputSystem.settings.editorInputBehaviorInPlayMode = editorInput;
            yield return null;
        }

        private GameObject Control(string name)
        {
            foreach (var item in root.GetComponentsInChildren<RectTransform>(true)) if (item.name == name) return item.gameObject;
            throw new InvalidOperationException("Missing control " + name);
        }

        private IEnumerator Pointer(Vector2 point, bool down)
        {
            InputSystem.QueueStateEvent(mouse, new MouseState { position = point, buttons = down ? (ushort)1 : (ushort)0 });
            yield return null; yield return null;
        }

        private IEnumerator Click(string name)
        {
            var rect = (RectTransform)Control(name).transform;
            var point = RectTransformUtility.WorldToScreenPoint(null, rect.TransformPoint(rect.rect.center));
            yield return Pointer(point, false); yield return Pointer(point, true); yield return Pointer(point, false);
        }

        [UnityTest]
        public IEnumerator FiveDaysThroughRenderedControlsToFinalAndFreshSession()
        {
            int objects = root.GetComponentsInChildren<Transform>(true).Length;
            yield return Click("CreditsButton");
            yield return Click("CreditsBackButton");
            yield return Click("NewSessionButton");
            var campaign = view.Campaign;
            int points = 0, visits = 0;
            for (int number = 1; number <= 5; number++)
            {
                var day = view.Day;
                Assert.AreEqual(number, day.DayNumber);
                Assert.AreEqual(100, day.Resources.Stamina);
                Assert.AreEqual(points, campaign.TotalPoints);
                int stamina = 100;
                while (day.Phase != DayPhase.Result)
                {
                    yield return Click("ClientButton0");
                    yield return new WaitForSecondsRealtime(1.55f);
                    Assert.AreEqual(VisitPhase.ChoosingExercise, view.Model.Phase);
                    int slot = 0;
                    for (int i = 1; i < view.Model.Hand.Count; i++) if (view.Model.Hand[i].Rarity < view.Model.Hand[slot].Rarity) slot = i;
                    int cost = VisitRules.BaseCost(view.Model.Hand[slot].Rarity);
                    yield return Click("ExerciseCard" + slot);
                    Assert.AreEqual(stamina, day.Resources.Stamina);
                    yield return Click("ConfirmButton");
                    stamina = Math.Max(0, stamina - cost);
                    yield return Click("FinishActivityButton");
                    points += view.Model.LastResult.Points; visits++;
                    Assert.AreEqual(points, campaign.TotalPoints);
                    yield return new WaitForSecondsRealtime(.5f);
                    yield return Click("ContinueButton");
                    yield return new WaitForSecondsRealtime(.95f);
                    Assert.AreEqual(stamina, day.Resources.Stamina);
                }
                Assert.AreEqual(number, campaign.Days.Count);
                Assert.AreEqual(visits, campaign.History.Count);
                Assert.IsTrue(Control("DayResultPanel").activeSelf);
                Assert.IsFalse(view.IsMainMenu);
                if (number < 5)
                {
                    Assert.IsFalse(Control("DayMenuButton").activeSelf);
                    StringAssert.Contains("Melhorias ainda não implementadas", Control("DemoEnded").GetComponent<TMP_Text>().text);
                    yield return Click("NextDayButton");
                    Assert.AreNotSame(day.Deck, view.Day.Deck);
                }
            }
            Assert.AreEqual(CampaignPhase.Completed, campaign.Phase);
            string final = Control("DailySummary").GetComponent<TMP_Text>().text;
            StringAssert.Contains("Dias concluídos: 5 / 5", final);
            StringAssert.Contains("Atendimentos realizados: " + visits, final);
            StringAssert.Contains("Pontuação total: " + points, final);
            StringAssert.Contains("Objetivo da campanha: 500", final);
            Assert.IsFalse(Control("NextDayButton").activeSelf);
            yield return Click("DayMenuButton");
            Assert.IsTrue(view.IsMainMenu);
            yield return Click("NewSessionButton");
            Assert.AreNotSame(campaign, view.Campaign);
            Assert.AreEqual(0, view.Campaign.TotalPoints);
            Assert.AreEqual(0, view.Campaign.History.Count);
            Assert.AreEqual(1, view.Day.DayNumber);
            Assert.AreEqual(objects, root.GetComponentsInChildren<Transform>(true).Length);
            Assert.AreEqual(1, root.GetComponentsInChildren<EventSystem>(true).Length);
        }
    }
}

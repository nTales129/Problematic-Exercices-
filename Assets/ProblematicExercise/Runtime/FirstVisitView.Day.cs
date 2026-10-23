using System;
using System.Collections;
using System.Text;
using ProblematicExercise.Domain;
using TMPro;
using UnityEngine;

namespace ProblematicExercise.Runtime
{
    public sealed partial class FirstVisitView
    {
        private GameObject menuPanel, creditsPanel, dayResultPanel;
        private TMP_Text dayStatus, dayHistory, dailySummary;
        private UnityEngine.UI.Image progressFill, clientShirt, clientMouth;
        private RectTransform trainer, leftArm, rightArm;
        public GameplayDay Day { get; private set; }
        [SerializeField] private bool campaignMode;
        public CampaignSession Campaign { get; private set; }
        private TMP_Text dayResultTitle, continuationNote;
        private UnityEngine.UI.Button nextDayButton, dayMenuButton;
        public bool IsMainMenu => dailyMode && menuPanel != null && menuPanel.activeSelf;

        // Scene composition/test setup only; player navigation goes through visible controls.
        public void ConfigureAsDay()
        {
            if (canvas != null) throw new InvalidOperationException("Configure day mode before enabling the object.");
            dailyMode = true;
        }

        public void ConfigureAsCampaign()
        {
            ConfigureAsDay();
            campaignMode = true;
        }

        private RectTransform BuildPerson(string name, Transform parent, Vector2 min, Vector2 max, Color shirt, bool isTrainer)
        {
            var person = Box(name, parent, min, max, Color.clear).rectTransform;
            Color skin = new Color32(214, 173, 139, 255);
            Box("LeftShoe", person, new Vector2(.17f, 0), new Vector2(.44f, .045f), ink);
            Box("RightShoe", person, new Vector2(.56f, 0), new Vector2(.83f, .045f), ink);
            Box("LeftLeg", person, new Vector2(.24f, .045f), new Vector2(.44f, .35f), new Color32(20, 36, 35, 255));
            Box("RightLeg", person, new Vector2(.56f, .045f), new Vector2(.76f, .35f), new Color32(20, 36, 35, 255));
            var torso = Box("Shirt", person, new Vector2(.2f, .32f), new Vector2(.8f, .71f), shirt);
            var armA = Box("LeftArm", person, new Vector2(.05f, .34f), new Vector2(.19f, .7f), skin).rectTransform;
            var armB = Box("RightArm", person, new Vector2(.81f, .34f), new Vector2(.95f, .7f), skin).rectTransform;
            Box("Neck", person, new Vector2(.4f, .69f), new Vector2(.6f, .78f), skin);
            Box("Head", person, new Vector2(.28f, .76f), new Vector2(.72f, .98f), skin);
            Box("Hair", person, new Vector2(.25f, .94f), new Vector2(.75f, 1), new Color32(54, 45, 39, 255));
            Box("EyeLeft", person, new Vector2(.35f, .87f), new Vector2(.39f, .895f), panelColor);
            Box("EyeRight", person, new Vector2(.61f, .87f), new Vector2(.65f, .895f), panelColor);
            var mouth = Box("Mouth", person, new Vector2(.4f, .8f), new Vector2(.6f, .815f), panelColor);
            if (isTrainer)
            {
                Box("Headband", person, new Vector2(.25f, .92f), new Vector2(.75f, .945f), accent);
                Box("TrainingBadge", person, new Vector2(.57f, .56f), new Vector2(.7f, .64f), panelColor);
                Box("Ponytail", person, new Vector2(.72f, .77f), new Vector2(.87f, .97f), new Color32(54, 45, 39, 255));
            }
            else
            {
                clientShirt = torso; clientMouth = mouth;
                leftArm = armA; rightArm = armB;
            }
            return person;
        }

        private void BuildDayPanels()
        {
            dayStatus = Label("DayStatus", canvas, "", 16, new Vector2(.035f, .005f), new Vector2(.45f, .05f), muted);
            dayHistory = Label("DayHistory", canvas, "", 16, new Vector2(.62f, .005f), new Vector2(.965f, .05f), accent);
            dayHistory.alignment = TextAlignmentOptions.MidlineRight;
            menuPanel = Box("MainMenu", canvas, new Vector2(.04f, .15f), new Vector2(.55f, .85f), new Color32(16, 29, 27, 248)).gameObject;
            Label("MenuEyebrow", menuPanel.transform, "ATELIÊ DE MOVIMENTO", 18, new Vector2(.06f, .85f), new Vector2(.94f, .96f), accent);
            Label("GameTitle", menuPanel.transform, "PROBLEMATIC\nEXERCISE", 52, new Vector2(.06f, .52f), new Vector2(.94f, .86f), ink);
            Label("MenuDescription", menuPanel.transform, campaignMode ? "Seis pessoas. Cinco dias para encontrar o ritmo." : "Três pessoas. Um dia para encontrar o ritmo.", 19, new Vector2(.06f, .4f), new Vector2(.94f, .53f), muted);
            Button("NewSessionButton", menuPanel.transform, "Nova sessão", new Vector2(.06f, .27f), new Vector2(.83f, .38f), StartDay);
            Button("CreditsButton", menuPanel.transform, "Créditos", new Vector2(.06f, .14f), new Vector2(.43f, .25f), () =>
            {
                menuPanel.SetActive(false); creditsPanel.SetActive(true);
            });
            Button("QuitButton", menuPanel.transform, "Sair", new Vector2(.46f, .14f), new Vector2(.83f, .25f), () => Application.Quit());
            creditsPanel = Box("CreditsPanel", canvas, new Vector2(.15f, .17f), new Vector2(.85f, .83f), panelColor).gameObject;
            Label("CreditsTitle", creditsPanel.transform, "Créditos", 34, new Vector2(.06f, .8f), new Vector2(.94f, .95f), ink);
            Label("CreditsText", creditsPanel.transform,
                "Problematic Exercise\nUm protótipo autoral de academia.\n\nDesenvolvimento: projeto Problematic Exercise\nAssistência técnica: Codex\nMotor: Unity\nTipografia: Liberation Sans — SIL OFL 1.1\nPersonagens, textos e formas próprios.",
                21, new Vector2(.06f, .22f), new Vector2(.94f, .79f), muted);
            Button("CreditsBackButton", creditsPanel.transform, "Voltar", new Vector2(.06f, .07f), new Vector2(.4f, .18f), ShowMainMenu);
            dayResultPanel = Box("DayResultPanel", canvas, new Vector2(.14f, .12f), new Vector2(.86f, .85f), panelColor).gameObject;
            dayResultTitle = Label("DayResultTitle", dayResultPanel.transform, "Dia 1 concluído", 36, new Vector2(.06f, .84f), new Vector2(.94f, .96f), ink);
            dailySummary = Label("DailySummary", dayResultPanel.transform, "", 22, new Vector2(.06f, .25f), new Vector2(.94f, .84f), ink);
            if (campaignMode)
            {
                dailySummary.enableAutoSizing = true;
                dailySummary.fontSizeMin = 16;
                dailySummary.fontSizeMax = 22;
            }
            Box("ProgressTrack", dayResultPanel.transform, new Vector2(.06f, .2f), new Vector2(.94f, .22f), new Color32(60, 77, 66, 255));
            progressFill = Box("ProgressFill", dayResultPanel.transform, new Vector2(.06f, .2f), new Vector2(.06f, .22f), accent);
            dayMenuButton = Button("DayMenuButton", dayResultPanel.transform, "Voltar ao menu", new Vector2(.06f, .055f), new Vector2(.5f, .16f), ShowMainMenu);
            nextDayButton = Button("NextDayButton", dayResultPanel.transform, "Próximo dia", new Vector2(.06f, .055f), new Vector2(.5f, .16f), NextCampaignDay);
            continuationNote = Label("DemoEnded", dayResultPanel.transform, "Demonstração encerrada no dia 1.", 16, new Vector2(.53f, .055f), new Vector2(.94f, .16f), muted);
        }

        private void HideDayPanels()
        {
            menuPanel.SetActive(false); creditsPanel.SetActive(false); dayResultPanel.SetActive(false);
            heading.gameObject.SetActive(true); message.gameObject.SetActive(true);
            staminaLabel.gameObject.SetActive(true); pointsLabel.gameObject.SetActive(true);
            dayStatus.gameObject.SetActive(true); dayHistory.gameObject.SetActive(true);
        }

        private void HideVisitPanels()
        {
            startPanel.SetActive(false); clientsPanel.SetActive(false); handPanel.SetActive(false);
            resultPanel.SetActive(false); completePanel.SetActive(false); finishButton.gameObject.SetActive(false);
            actor.gameObject.SetActive(false); clientLabel.text = ""; activityLabel.text = "";
            heading.gameObject.SetActive(false); message.gameObject.SetActive(false);
        }

        private void ShowMainMenu()
        {
            StopAllCoroutines();
            Day = null; Model = null; Campaign = null;
            HideVisitPanels();
            creditsPanel.SetActive(false); dayResultPanel.SetActive(false); menuPanel.SetActive(true);
            staminaLabel.gameObject.SetActive(false); pointsLabel.gameObject.SetActive(false);
            dayStatus.gameObject.SetActive(false); dayHistory.gameObject.SetActive(false);
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
        }

        private void StartDay()
        {
            if (!IsMainMenu) return;
            if (campaignMode)
            {
                Campaign = VisitSample.CreateCampaign(sampleSeed);
                Day = Campaign.CurrentDay;
            }
            else
            {
                Day = VisitSample.CreateDay(sampleSeed);
                Day.Begin();
            }
            Model = Day.CurrentVisit;
            Refresh();
        }

        private void RefreshDayHud()
        {
            dayStatus.text = "DIA " + Day.DayNumber + (campaignMode ? " / 5" : "") + "   /   " + Day.CompletedVisits + " de " + Day.Quota + " atendimentos";
            if (Campaign != null) pointsLabel.text = "PONTOS   " + Campaign.TotalPoints;
            var notes = new StringBuilder();
            foreach (var record in Day.Resources.History)
            {
                if (notes.Length > 0) notes.Append("   ·   ");
                notes.Append(record.Grade);
            }
            dayHistory.text = notes.ToString();
            continueButton.interactable = true;
        }

        private void ShowDayResult()
        {
            HideDayPanels(); HideVisitPanels();
            RefreshDayHud();
            dayResultPanel.SetActive(true);
            var result = Day.Result;
            dayResultTitle.text = "Dia " + result.DayNumber + " concluído";
            var text = new StringBuilder();
            text.Append("Clientes atendidos: ").Append(result.Visits.Count).Append(" / ").Append(Day.Quota).Append("\n\n");
            foreach (var visit in result.Visits)
                text.Append(visit.ClientName).Append("   —   ").Append(visit.Grade).Append("   ·   ").Append(visit.Points).Append(" pontos\n");
            text.Append("\nPontos do dia: ").Append(result.DayPoints).Append("   |   Acumulado: ").Append(result.AccumulatedPoints);
            text.Append("\nProgresso: ").Append(result.Progress).Append(" / ").Append(result.ProgressTarget);
            text.Append("\n").Append(result.Reason == EndDayReason.OutOfStamina ? "Encerramento: stamina esgotada." : "Encerramento: clientes do dia atendidos.");
            dailySummary.text = text.ToString();
            bool final = Campaign != null && Campaign.Phase == CampaignPhase.Completed;
            nextDayButton.gameObject.SetActive(Campaign != null && !final);
            dayMenuButton.gameObject.SetActive(Campaign == null || final);
            continuationNote.text = Campaign == null ? "Demonstração encerrada no dia 1."
                : final ? "Sessão concluída. Sem save permanente." : "Continuação direta. Melhorias ainda não implementadas.";
            if (final)
            {
                var summary = Campaign.Result;
                dayResultTitle.text = "Campanha concluída";
                dailySummary.text = "Dias concluídos: " + summary.DaysCompleted + " / 5\nAtendimentos realizados: " + summary.VisitsCompleted
                    + "\n\nPontuação total: " + summary.TotalPoints + "\nProgresso alcançado: " + summary.Progress
                    + "\nObjetivo da campanha: " + summary.Target + " pontos\n\nResultado final: "
                    + (summary.GoalReached ? "objetivo alcançado." : "objetivo não alcançado.")
                    + "\nDia 5: " + result.Visits.Count + " atendimentos, " + result.DayPoints + " pontos."
                    + (result.Reason == EndDayReason.OutOfStamina ? " Stamina esgotada." : " Clientes atendidos.");
            }
            var rect = progressFill.rectTransform;
            rect.anchorMax = new Vector2(.06f + .88f * result.Progress / result.ProgressTarget, .22f);
            rect.offsetMin = rect.offsetMax = Vector2.zero;
        }

        private void NextCampaignDay()
        {
            if (Campaign == null || !Campaign.ContinueToNextDay()) return;
            StopAllCoroutines();
            Day = Campaign.CurrentDay;
            Model = Day.CurrentVisit;
            Refresh();
        }

        private void UpdatePersonIdentity()
        {
            leftArm.localRotation = rightArm.localRotation = Quaternion.identity;
            clientMouth.color = panelColor;
            if (Model.CurrentClient == null) return;
            clientShirt.color = Model.CurrentClient.Id.EndsWith("caio", StringComparison.Ordinal) ? new Color32(104, 156, 183, 255)
                : Model.CurrentClient.Id.EndsWith("nara", StringComparison.Ordinal) ? new Color32(173, 130, 178, 255) : new Color32(208, 132, 83, 255);
            if (Model.Phase == VisitPhase.Result) clientMouth.color = accent;
        }

        private void AnimateExercise(float pulse)
        {
            bool rowing = Model.Played.Id.EndsWith("row", StringComparison.Ordinal);
            leftArm.localRotation = Quaternion.Euler(0, 0, (rowing ? 65 : 25) + pulse * 25);
            rightArm.localRotation = Quaternion.Euler(0, 0, -(rowing ? 65 : 25) - pulse * 25);
            trainer.localRotation = Quaternion.Euler(0, 0, pulse * 1.5f);
        }

        private IEnumerator React()
        {
            continueButton.interactable = false;
            float elapsed = 0;
            while (elapsed < .4f)
            {
                elapsed += Time.unscaledDeltaTime;
                actor.anchoredPosition = actorRest + Vector2.up * Mathf.Sin(Mathf.Clamp01(elapsed / .4f) * Mathf.PI) * 18;
                yield return null;
            }
            actor.anchoredPosition = actorRest;
            continueButton.interactable = true;
        }
    }
}

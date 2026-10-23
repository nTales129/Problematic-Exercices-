using System;
using System.Collections;
using ProblematicExercise.Domain;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;

namespace ProblematicExercise.Runtime
{
    public sealed partial class FirstVisitView : MonoBehaviour
    {
        [SerializeField] private int sampleSeed = 17;
        [SerializeField] private TMP_FontAsset font;
        [SerializeField] private bool dailyMode;
        private readonly Color ink = new Color32(234, 239, 228, 255);
        private readonly Color muted = new Color32(177, 190, 178, 255);
        private readonly Color accent = new Color32(194, 222, 135, 255);
        private readonly Color panelColor = new Color32(31, 48, 44, 255);
        private RectTransform canvas;
        private GameObject startPanel, clientsPanel, handPanel, resultPanel, completePanel;
        private TMP_Text heading, message, staminaLabel, pointsLabel, preview, resultLabel, clientLabel, activityLabel;
        private UnityEngine.UI.Button confirmButton, finishButton, continueButton;
        private readonly UnityEngine.UI.Button[] clientButtons = new UnityEngine.UI.Button[3];
        private readonly UnityEngine.UI.Image[] cards = new UnityEngine.UI.Image[3];
        private readonly TMP_Text[] cardLabels = new TMP_Text[3];
        private readonly ExerciseCardView[] cardInput = new ExerciseCardView[3];
        private RectTransform actor;
        private Vector2 actorRest;
        public FirstVisit Model { get; private set; }

        private void Awake()
        {
            if (!dailyMode) Model = VisitSample.Create(sampleSeed);
            if (font == null) font = Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");
            if (font == null) throw new InvalidOperationException("Prepare FirstVisit using the Problematic Exercise menu before playing.");
            BuildView();
            if (dailyMode) ShowMainMenu(); else Refresh();
        }

        private void BuildView()
        {
            var canvasObject = new GameObject("FirstVisitCanvas", typeof(RectTransform), typeof(Canvas),
                typeof(UnityEngine.UI.CanvasScaler), typeof(UnityEngine.UI.GraphicRaycaster));
            canvasObject.transform.SetParent(transform, false);
            canvas = (RectTransform)canvasObject.transform;
            canvasObject.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasObject.GetComponent<UnityEngine.UI.CanvasScaler>();
            scaler.uiScaleMode = UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1280, 720);
            scaler.matchWidthOrHeight = .5f;
            var eventObject = new GameObject("FirstVisitEventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
            eventObject.transform.SetParent(transform, false);
            eventObject.GetComponent<InputSystemUIInputModule>().AssignDefaultActions();

            Box("Background", canvas, Vector2.zero, Vector2.one, new Color32(16, 29, 27, 255));
            BuildRoom();
            Label("Brand", canvas, "PE / ATELIÊ DE MOVIMENTO", 18, new Vector2(.035f, .925f), new Vector2(.6f, .98f), accent);
            heading = Label("Heading", canvas, "Primeiro atendimento", 26, new Vector2(.035f, .87f), new Vector2(.72f, .925f), ink);
            staminaLabel = Label("Stamina", canvas, "", 18, new Vector2(.76f, .925f), new Vector2(.965f, .98f), ink);
            pointsLabel = Label("Points", canvas, "", 18, new Vector2(.76f, .865f), new Vector2(.965f, .925f), accent);
            message = Label("Message", canvas, "", 18, new Vector2(.06f, .3f), new Vector2(.75f, .37f), ink);

            startPanel = Box("StartPanel", canvas, new Vector2(.28f, .23f), new Vector2(.72f, .69f), panelColor).gameObject;
            Label("Welcome", startPanel.transform, "Cada pessoa, um ritmo.", 32, new Vector2(.07f, .61f), new Vector2(.93f, .9f), ink);
            Label("WelcomeText", startPanel.transform, "Receba um cliente, escolha um exercício e acompanhe seu resultado.", 21,
                new Vector2(.07f, .3f), new Vector2(.93f, .64f), muted);
            Button("StartButton", startPanel.transform, "Iniciar atendimento", new Vector2(.07f, .08f), new Vector2(.93f, .26f), () =>
            {
                if (Model.Start()) Refresh();
            });

            clientsPanel = Box("ClientsPanel", canvas, new Vector2(.16f, .06f), new Vector2(.84f, .24f), new Color32(31, 48, 44, 0)).gameObject;
            for (int i = 0; i < 3; i++)
            {
                int slot = i;
                clientButtons[i] = Button("ClientButton" + i, clientsPanel.transform, "", new Vector2(.025f + i * .325f, .18f),
                    new Vector2(.325f + i * .325f, .82f), () => ChooseClient(slot));
            }

            handPanel = Box("HandPanel", canvas, new Vector2(0, 0), new Vector2(1, .31f), new Color32(19, 34, 30, 255)).gameObject;
            Label("HandHint", handPanel.transform, "Escolha uma carta • Arraste para cima para confirmar", 16,
                new Vector2(.035f, .86f), new Vector2(.72f, 1), muted);
            for (int i = 0; i < 3; i++)
            {
                cards[i] = Box("ExerciseCard" + i, handPanel.transform, new Vector2(.035f + i * .235f, .07f),
                    new Vector2(.25f + i * .235f, .83f), panelColor, true);
                Box("CardAccent", cards[i].transform, new Vector2(0, .94f), Vector2.one, accent);
                cardLabels[i] = Label("CardLabel", cards[i].transform, "", 21, new Vector2(.07f, .04f), new Vector2(.93f, .96f), ink);
                cardInput[i] = cards[i].gameObject.AddComponent<ExerciseCardView>();
            }
            preview = Label("Preview", handPanel.transform, "", 17, new Vector2(.76f, .4f), new Vector2(.965f, .93f), muted);
            confirmButton = Button("ConfirmButton", handPanel.transform, "Confirmar", new Vector2(.76f, .1f), new Vector2(.965f, .35f), () =>
            {
                if (Model.Selected != null) Confirm(Model.Selected.Id);
            });

            finishButton = Button("FinishActivityButton", canvas, "Concluir exercício", new Vector2(.37f, .055f), new Vector2(.63f, .135f), () =>
            {
                if (Model.FinishActivity())
                {
                    Refresh();
                    if (dailyMode) StartCoroutine(React());
                }
            });
            resultPanel = Box("ResultPanel", canvas, new Vector2(.73f, .33f), new Vector2(.965f, .84f), panelColor).gameObject;
            Label("ResultTitle", resultPanel.transform, "ATENDIMENTO AVALIADO", 18, new Vector2(.07f, .82f), new Vector2(.93f, .95f), accent);
            resultLabel = Label("ResultLabel", resultPanel.transform, "", 25, new Vector2(.07f, .3f), new Vector2(.93f, .8f), ink);
            continueButton = Button("ContinueButton", resultPanel.transform, "Continuar", new Vector2(.07f, .08f), new Vector2(.93f, .23f), () =>
            {
                if (Model.Continue()) { Refresh(); StartCoroutine(Exit()); }
            });

            completePanel = Box("CompletePanel", canvas, new Vector2(.27f, .22f), new Vector2(.73f, .7f), panelColor).gameObject;
            Label("Completed", completePanel.transform, "Atendimento concluído", 30, new Vector2(.07f, .62f), new Vector2(.93f, .9f), ink);
            Label("CompletedText", completePanel.transform, "Obrigado por acompanhar este treino.", 21, new Vector2(.07f, .34f), new Vector2(.93f, .6f), muted);
            Button("RestartButton", completePanel.transform, "Voltar ao início", new Vector2(.07f, .08f), new Vector2(.93f, .27f), () =>
            {
                Model = VisitSample.Create(sampleSeed);
                actor.anchoredPosition = actorRest;
                Refresh();
            });
            if (dailyMode) BuildDayPanels();
        }

        private void BuildRoom()
        {
            var stage = Box("TrainingRoom", canvas, new Vector2(.025f, .29f), new Vector2(.975f, .865f), new Color32(42, 62, 57, 255));
            stage.gameObject.AddComponent<UnityEngine.UI.RectMask2D>();
            Box("Floor", stage.transform, Vector2.zero, new Vector2(1, .23f), new Color32(76, 83, 63, 255));
            for (int i = 0; i < 5; i++)
            {
                Box("Window" + i, stage.transform, new Vector2(.045f + .125f * i, .51f), new Vector2(.145f + .125f * i, .93f), new Color32(109, 145, 136, 255));
                Box("WindowFrame" + i, stage.transform, new Vector2(.091f + .125f * i, .51f), new Vector2(.098f + .125f * i, .93f), panelColor);
            }
            Box("WallStripe", stage.transform, new Vector2(0, .3f), new Vector2(1, .32f), new Color32(174, 160, 111, 255));
            Box("Mat", stage.transform, new Vector2(.16f, .145f), new Vector2(.72f, .185f), new Color32(164, 184, 119, 255));
            Box("RackLeft", stage.transform, new Vector2(.83f, .23f), new Vector2(.84f, .67f), muted);
            Box("RackRight", stage.transform, new Vector2(.94f, .23f), new Vector2(.95f, .67f), muted);
            for (int i = 0; i < 3; i++)
                Box("RackBar" + i, stage.transform, new Vector2(.81f, .28f + .13f * i), new Vector2(.97f, .3f + .13f * i), new Color32(180, 162, 122, 255));
            actor = BuildPerson("ClientActor", stage.transform, new Vector2(.26f, .18f), new Vector2(.38f, .79f), new Color32(208, 132, 83, 255), false);
            actorRest = actor.anchoredPosition;
            trainer = BuildPerson("TrainerActor", stage.transform, new Vector2(.59f, .18f), new Vector2(.71f, .79f), accent, true);
            Label("TrainerName", stage.transform, "TREINADORA", 16, new Vector2(.565f, .8f), new Vector2(.77f, .89f), ink);
            clientLabel = Label("ClientName", stage.transform, "", 18, new Vector2(.25f, .8f), new Vector2(.47f, .89f), ink);
            activityLabel = Label("ActivityName", canvas, "", 20, new Vector2(.36f, .245f), new Vector2(.8f, .29f), accent);
        }

        private void ChooseClient(int index)
        {
            if (index >= Model.Offered.Count) return;
            string id = Model.Offered[index].Id;
            if (!(dailyMode ? Day.ChooseClient(id) : Model.ChooseClient(id))) return;
            Refresh();
            StartCoroutine(Enter());
        }

        private IEnumerator Enter()
        {
            float elapsed = 0;
            while (elapsed < .8f)
            {
                elapsed += Time.unscaledDeltaTime;
                actor.anchoredPosition = actorRest + Vector2.left * 520 * (1 - Mathf.Clamp01(elapsed / .8f));
                yield return null;
            }
            if (dailyMode)
            {
                // Standard visit order: await arrival, then entry dialogue, then hand.
                // Timed placeholder dialogue is original presentation, not recovered duration.
                message.text = "“Vamos encontrar meu ritmo de hoje.”";
                yield return new WaitForSecondsRealtime(.6f);
            }
            if (Model.FinishEntry()) Refresh();
        }

        private IEnumerator Exit()
        {
            float elapsed = 0;
            while (elapsed < .8f)
            {
                elapsed += Time.unscaledDeltaTime;
                actor.anchoredPosition = actorRest + Vector2.right * 420 * Mathf.Clamp01(elapsed / .8f);
                yield return null;
            }
            if (dailyMode)
            {
                if (!Day.FinishExit()) yield break;
                Model = Day.CurrentVisit;
                if (Day.Phase == DayPhase.Result) ShowDayResult(); else Refresh();
            }
            else if (Model.FinishExit()) Refresh();
        }

        private void Select(string id)
        {
            if (Model.Select(id)) RefreshPreview();
        }

        private void Confirm(string id)
        {
            if (Model.Confirm(id)) Refresh();
        }

        private void RefreshPreview()
        {
            confirmButton.interactable = Model.Selected != null;
            preview.text = Model.Selected == null ? "Selecione para ver o exercício." :
                Model.Selected.Name + "\nStamina: " + VisitRules.BaseCost(Model.Selected.Rarity);
            for (int i = 0; i < Model.Hand.Count && i < cards.Length; i++)
                cards[i].color = Model.Hand[i] == Model.Selected ? new Color32(71, 92, 59, 255) : panelColor;
        }

        private void Refresh()
        {
            if (dailyMode) HideDayPanels();
            var phase = Model.Phase;
            startPanel.SetActive(phase == VisitPhase.Start);
            clientsPanel.SetActive(phase == VisitPhase.ChoosingClient);
            handPanel.SetActive(phase == VisitPhase.ChoosingExercise);
            finishButton.gameObject.SetActive(phase == VisitPhase.Activity);
            resultPanel.SetActive(phase == VisitPhase.Result);
            completePanel.SetActive(phase == VisitPhase.Complete);
            actor.gameObject.SetActive(phase >= VisitPhase.Entering && phase <= VisitPhase.Exiting);
            actor.anchoredPosition = actorRest;
            actor.localRotation = Quaternion.identity;
            UpdatePersonIdentity();
            staminaLabel.text = "STAMINA   " + Model.Stamina + " / " + Model.MaximumStamina;
            pointsLabel.text = "PONTOS   " + Model.TotalPoints;
            clientLabel.text = actor.gameObject.activeSelf ? Model.CurrentClient.Name : "";
            activityLabel.text = phase == VisitPhase.Activity ? Model.Played.Name : "";
            switch (phase)
            {
                case VisitPhase.Start: heading.text = "Primeiro atendimento"; message.text = "Um espaço para experimentar o movimento."; break;
                case VisitPhase.ChoosingClient: heading.text = "Quem vamos receber?"; message.text = "Escolha um cliente para o atendimento."; break;
                case VisitPhase.Entering: heading.text = "Boas-vindas, " + Model.CurrentClient.Name; message.text = dailyMode ? "Cliente chegando..." : "“Vamos encontrar meu ritmo de hoje.”"; break;
                case VisitPhase.ChoosingExercise: heading.text = "Um treino para " + Model.CurrentClient.Name; message.text = "Escolher permite explorar. Confirmar começa o exercício."; break;
                case VisitPhase.Activity: heading.text = "Em movimento"; message.text = "“Respire e acompanhe o seu ritmo.” Conclua quando terminar."; break;
                case VisitPhase.Result: heading.text = "Resultado do atendimento"; message.text = "O exercício terminou. Confira a avaliação antes de continuar."; break;
                case VisitPhase.Exiting: heading.text = "Até a próxima, " + Model.CurrentClient.Name; message.text = "“Obrigado pelo treino de hoje!”"; break;
                case VisitPhase.Complete: heading.text = "Um treino, um novo passo"; message.text = "Primeiro atendimento encerrado."; break;
            }
            if (phase == VisitPhase.ChoosingClient)
                for (int i = 0; i < clientButtons.Length; i++)
                {
                    clientButtons[i].gameObject.SetActive(i < Model.Offered.Count);
                    if (i < Model.Offered.Count) clientButtons[i].GetComponentInChildren<TMP_Text>().text = Model.Offered[i].Name;
                }
            if (phase == VisitPhase.ChoosingExercise)
            {
                for (int i = 0; i < cards.Length; i++)
                {
                    cards[i].gameObject.SetActive(i < Model.Hand.Count);
                    if (i >= Model.Hand.Count) continue;
                    var exercise = Model.Hand[i];
                    cardLabels[i].text = "<size=14>EXERCÍCIO / " + (exercise.Rarity + 1) + "</size>\n<b>" + exercise.Name + "</b>\n<size=16>Stamina " + VisitRules.BaseCost(exercise.Rarity) + "</size>";
                    cardInput[i].Initialize(canvas, exercise.Id, Select, Confirm, () => Model.Phase == VisitPhase.ChoosingExercise);
                }
                RefreshPreview();
                if (Model.Hand.Count == 0) message.text = "Nenhum exercício disponível nesta mão.";
            }
            if (phase == VisitPhase.Result)
                resultLabel.text = Model.CurrentClient.Name + "\n<size=70>" + Model.ResultGrade + "</size>\n+" + Model.LastResult.Points + " pontos";
            if (dailyMode) RefreshDayHud();
            // Reset selection to a visible control after each screen transition.
            GameObject focus = phase == VisitPhase.Activity ? finishButton.gameObject : phase == VisitPhase.Result ? continueButton.gameObject : null;
            EventSystem.current.SetSelectedGameObject(focus);
        }

        private void Update()
        {
            if (Model == null || Model.Phase != VisitPhase.Activity) return;
            float pulse = Mathf.Sin(Time.unscaledTime * (1.8f + Model.Played.Rarity * .6f));
            actor.anchoredPosition = actorRest + new Vector2(pulse * 12, Mathf.Abs(pulse) * 18);
            actor.localRotation = Quaternion.Euler(0, 0, pulse * (3 + Model.Played.Rarity * 2));
            AnimateExercise(pulse);
        }

        private void OnDisable() => StopAllCoroutines();

        private UnityEngine.UI.Image Box(string name, Transform parent, Vector2 min, Vector2 max, Color color, bool raycast = false)
        {
            var obj = new GameObject(name, typeof(RectTransform), typeof(UnityEngine.UI.Image));
            obj.transform.SetParent(parent, false);
            var rect = (RectTransform)obj.transform;
            rect.anchorMin = min; rect.anchorMax = max;
            rect.offsetMin = rect.offsetMax = Vector2.zero;
            var image = obj.GetComponent<UnityEngine.UI.Image>();
            image.color = color; image.raycastTarget = raycast;
            return image;
        }

        private TMP_Text Label(string name, Transform parent, string text, float size, Vector2 min, Vector2 max, Color color)
        {
            var obj = new GameObject(name, typeof(RectTransform), typeof(TextMeshProUGUI));
            obj.transform.SetParent(parent, false);
            var rect = (RectTransform)obj.transform;
            rect.anchorMin = min; rect.anchorMax = max; rect.offsetMin = rect.offsetMax = Vector2.zero;
            var label = obj.GetComponent<TextMeshProUGUI>();
            label.font = font; label.text = text; label.fontSize = size; label.color = color;
            label.raycastTarget = false; label.alignment = TextAlignmentOptions.MidlineLeft;
            label.textWrappingMode = TextWrappingModes.Normal;
            return label;
        }

        private UnityEngine.UI.Button Button(string name, Transform parent, string text, Vector2 min, Vector2 max, UnityEngine.Events.UnityAction action)
        {
            var image = Box(name, parent, min, max, accent, true);
            var button = image.gameObject.AddComponent<UnityEngine.UI.Button>();
            button.targetGraphic = image;
            var label = Label("Label", image.transform, text, 21, new Vector2(.06f, .04f), new Vector2(.94f, .96f), new Color32(21, 36, 29, 255));
            label.alignment = TextAlignmentOptions.Center;
            button.onClick.AddListener(action);
            return button;
        }
    }
}

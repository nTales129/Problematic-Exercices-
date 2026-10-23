using System;
using System.Collections.Generic;

namespace ProblematicExercise.Domain
{
    public enum VisitPhase { Start, ChoosingClient, Entering, ChoosingExercise, Activity, Result, Exiting, Complete }

    public sealed class FirstVisit
    {
        private readonly List<Client> available;
        private readonly List<Client> offered = new List<Client>();
        private readonly List<Exercise> hand = new List<Exercise>();
        private readonly Random random;
        private readonly VisitResources resources;
        public ExerciseDeck Deck { get; }
        public VisitPhase Phase { get; private set; }
        public IReadOnlyList<Client> Offered { get; }
        public IReadOnlyList<Exercise> Hand { get; }
        public int AvailableCount => available.Count;
        public Client CurrentClient { get; private set; }
        public Exercise Selected { get; private set; }
        public Exercise Played { get; private set; }
        public int Stamina => resources.Stamina;
        public int MaximumStamina => resources.MaximumStamina;
        public int TotalPoints => resources.TotalPoints;
        public int ResultCount { get; private set; }
        public Grade ResultGrade { get; private set; }
        public Preference ResultPreference { get; private set; }
        public VisitRecord LastResult { get; private set; }

        public FirstVisit(IEnumerable<Client> clients, IEnumerable<Exercise> exercises, Random random, int initialStamina)
            : this(clients, new ExerciseDeck(exercises, random), random, new VisitResources(initialStamina)) { }

        internal FirstVisit(IEnumerable<Client> clients, ExerciseDeck deck, Random random, VisitResources resources)
        {
            this.random = random ?? throw new ArgumentNullException(nameof(random));
            this.resources = resources ?? throw new ArgumentNullException(nameof(resources));
            available = new List<Client>(clients);
            if (available.Count < 1) throw new ArgumentException("A visit requires an available client.");
            Deck = deck ?? throw new ArgumentNullException(nameof(deck));
            Offered = offered.AsReadOnly();
            Hand = hand.AsReadOnly();
        }

        public bool Start()
        {
            if (Phase != VisitPhase.Start) return false;
            var sample = new List<Client>(available);
            ExerciseDeck.Shuffle(sample, random);
            int count = Math.Min(random.Next(2, 4), sample.Count);
            offered.AddRange(sample.GetRange(0, count));
            Phase = VisitPhase.ChoosingClient;
            return true;
        }

        public bool ChooseClient(string id)
        {
            if (Phase != VisitPhase.ChoosingClient) return false;
            Client client = offered.Find(candidate => candidate.Id == id);
            if (client == null) return false;
            CurrentClient = client;
            available.Remove(client);
            offered.Clear();
            Phase = VisitPhase.Entering;
            return true;
        }

        public bool FinishEntry()
        {
            if (Phase != VisitPhase.Entering) return false;
            hand.AddRange(Deck.Draw());
            Phase = VisitPhase.ChoosingExercise;
            return true;
        }

        public bool Select(string id)
        {
            if (Phase != VisitPhase.ChoosingExercise) return false;
            var card = hand.Find(candidate => candidate.Id == id);
            if (card == null) return false;
            Selected = card;
            return true;
        }

        public bool Confirm(string id)
        {
            if (Phase != VisitPhase.ChoosingExercise) return false;
            Exercise card = hand.Find(candidate => candidate.Id == id);
            if (card == null) return false;
            Played = Selected = card;
            hand.Clear();
            Deck.DiscardDrawn();
            resources.Consume(card);
            Phase = VisitPhase.Activity;
            return true;
        }

        public bool FinishActivity()
        {
            if (Phase != VisitPhase.Activity) return false;
            ResultPreference = CurrentClient.Evaluate(Played);
            ResultGrade = VisitRules.EvaluateGrade(Played.Rarity, ResultPreference);
            LastResult = resources.Record(CurrentClient, Played, ResultGrade, ResultPreference);
            ResultCount++;
            Phase = VisitPhase.Result;
            return true;
        }

        public bool Continue()
        {
            if (Phase != VisitPhase.Result) return false;
            Phase = VisitPhase.Exiting;
            return true;
        }

        public bool FinishExit()
        {
            if (Phase != VisitPhase.Exiting) return false;
            Phase = VisitPhase.Complete;
            return true;
        }
    }
}

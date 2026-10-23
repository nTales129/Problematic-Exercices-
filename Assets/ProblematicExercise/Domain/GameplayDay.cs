using System;
using System.Collections.Generic;

namespace ProblematicExercise.Domain
{
    public sealed class StageProfile
    {
        public IReadOnlyList<int> ClientsPerDay { get; } = Array.AsReadOnly(new[] { 3, 4, 5, 6, 6 });
        public int ProgressTarget => 500;
        public int PointsPerPresent => 35;
        public int ExtraFreePresents => 0;
        public int PleaseThreshold => 10;
        public bool RandomClientsEveryDay => false;
    }

    public sealed class RosterEntry
    {
        public Client Client { get; }
        public int Level { get; }
        public RosterEntry(Client client, int level)
        {
            Client = client ?? throw new ArgumentNullException(nameof(client));
            Level = level;
        }
    }

    public sealed class ClientRoster
    {
        private readonly List<Client> available = new List<Client>();
        public IReadOnlyList<Client> Available { get; }
        public int InitialCount { get; }

        public ClientRoster(IEnumerable<RosterEntry> entries, int quota, bool randomEveryDay, Random random)
        {
            if (quota <= 0) throw new ArgumentOutOfRangeException(nameof(quota));
            if (random == null) throw new ArgumentNullException(nameof(random));
            Available = available.AsReadOnly();
            var all = new List<RosterEntry>(entries);
            var ids = new HashSet<string>();
            foreach (var entry in all)
                if (!ids.Add(entry.Client.Id)) throw new ArgumentException("Duplicate client ID in independent roster.");
            // Confirmed predicate; composing shuffle/take/complement follows the documented strong inference.
            var priority = all.FindAll(entry => randomEveryDay || entry.Level > 0);
            ExerciseDeck.Shuffle(priority, random);
            foreach (var entry in priority)
                if (available.Count < quota) available.Add(entry.Client);
            if (available.Count < quota)
            {
                var remaining = all.FindAll(entry => !available.Contains(entry.Client));
                ExerciseDeck.Shuffle(remaining, random);
                foreach (var entry in remaining)
                    if (available.Count < quota) available.Add(entry.Client);
            }
            ExerciseDeck.Shuffle(available, random);
            InitialCount = available.Count;
        }

        internal bool Remove(string id)
        {
            int index = available.FindIndex(client => client.Id == id);
            if (index < 0) return false;
            available.RemoveAt(index);
            return true;
        }
    }

    public enum DayPhase { Preparing, Visiting, Result }
    public enum EndDayReason { OutOfClients, OutOfStamina }

    public sealed class DayResult
    {
        public int DayNumber { get; }
        public EndDayReason Reason { get; }
        public IReadOnlyList<VisitRecord> Visits { get; }
        public int DayPoints { get; }
        public int AccumulatedPoints { get; }
        public int Progress { get; }
        public int ProgressTarget { get; }
        public int PresentPointsGained { get; }
        public int PresentPointBalance { get; }

        internal DayResult(VisitResources resources, EndDayReason reason, StageProfile profile,
            int previousPoints, int previousProgress, int previousPresentPoints, int dayNumber)
        {
            DayNumber = dayNumber;
            Reason = reason;
            Visits = new List<VisitRecord>(resources.History).AsReadOnly();
            DayPoints = resources.TotalPoints;
            AccumulatedPoints = checked(previousPoints + DayPoints);
            ProgressTarget = profile.ProgressTarget;
            Progress = (int)Math.Max(0, Math.Min(ProgressTarget, (long)previousProgress + DayPoints));
            PresentPointsGained = checked(DayPoints + profile.PointsPerPresent * profile.ExtraFreePresents);
            PresentPointBalance = checked(previousPresentPoints + PresentPointsGained);
        }
    }

    public sealed class GameplayDay
    {
        private readonly Random random;
        private readonly int previousPoints, previousProgress, previousPresentPoints;
        public StageProfile Profile { get; }
        public DayPhase Phase { get; private set; }
        public ClientRoster Roster { get; }
        public ExerciseDeck Deck { get; }
        public VisitResources Resources { get; }
        public FirstVisit CurrentVisit { get; private set; }
        public DayResult Result { get; private set; }
        public int CompletedVisits { get; private set; }
        public int DayNumber { get; }
        public int Quota => Profile.ClientsPerDay[DayNumber - 1];
        public event Action<DayResult> Completed;

        public GameplayDay(IEnumerable<RosterEntry> clients, IEnumerable<Exercise> exercises, Random random,
            int initialStamina = 100, float costModifier = 1f, int previousPoints = 0,
            int previousProgress = 0, int previousPresentPoints = 0, int dayNumber = 1)
        {
            this.random = random ?? throw new ArgumentNullException(nameof(random));
            this.previousPoints = previousPoints; this.previousProgress = previousProgress;
            this.previousPresentPoints = previousPresentPoints;
            Profile = new StageProfile();
            if (dayNumber < 1 || dayNumber > Profile.ClientsPerDay.Count) throw new ArgumentOutOfRangeException(nameof(dayNumber));
            DayNumber = dayNumber;
            Roster = new ClientRoster(clients, Quota, Profile.RandomClientsEveryDay, random);
            if (Roster.InitialCount == 0) throw new ArgumentException("No clients available for day.");
            Deck = new ExerciseDeck(exercises, random);
            Resources = new VisitResources(initialStamina, costModifier);
        }

        public bool Begin()
        {
            if (Phase != DayPhase.Preparing) return false;
            Phase = DayPhase.Visiting;
            NextVisit();
            return true;
        }

        private void NextVisit()
        {
            CurrentVisit = new FirstVisit(Roster.Available, Deck, random, Resources);
            CurrentVisit.Start();
        }

        public bool ChooseClient(string id)
        {
            if (Phase != DayPhase.Visiting || !CurrentVisit.ChooseClient(id)) return false;
            Roster.Remove(id);
            return true;
        }

        public bool FinishExit()
        {
            if (Phase != DayPhase.Visiting || !CurrentVisit.FinishExit()) return false;
            CompletedVisits++;
            // The completed visit and result are retained before either end condition.
            // KO has the locally confirmed precedence over simultaneous roster exhaustion.
            if (Resources.Stamina == 0 || Roster.Available.Count == 0)
            {
                var reason = Resources.Stamina == 0 ? EndDayReason.OutOfStamina : EndDayReason.OutOfClients;
                Result = new DayResult(Resources, reason, Profile, previousPoints, previousProgress, previousPresentPoints, DayNumber);
                Phase = DayPhase.Result;
                Completed?.Invoke(Result);
            }
            else NextVisit();
            return true;
        }
    }
}

using System;
using System.Collections.Generic;

namespace ProblematicExercise.Domain
{
    public enum CampaignPhase { Playing, Completed }

    public sealed class CampaignClientState
    {
        public RosterEntry Entry { get; }
        public int CompletedVisits { get; internal set; }
        public CampaignClientState(RosterEntry entry) { Entry = entry; }
    }

    public sealed class CampaignResult
    {
        public int DaysCompleted { get; }
        public int VisitsCompleted { get; }
        public int TotalPoints { get; }
        public int Progress { get; }
        public int Target { get; }
        public bool GoalReached => Progress >= Target;
        internal CampaignResult(int days, int visits, int points, int progress, int target)
        {
            DaysCompleted = days; VisitsCompleted = visits; TotalPoints = points;
            Progress = progress; Target = target;
        }
    }

    // Own lifecycle policy: new daily deck and stamina, persistent session ledgers.
    // No recovered LevelUp formula is claimed; initial priority levels stay fixed.
    public sealed class CampaignSession
    {
        private readonly List<RosterEntry> roster;
        private readonly List<Exercise> exercises;
        private readonly Random random;
        private readonly int dailyStamina;
        private readonly List<DayResult> days = new List<DayResult>();
        private readonly List<VisitRecord> history = new List<VisitRecord>();
        private readonly List<CampaignClientState> clients = new List<CampaignClientState>();
        private int accumulatedPoints;
        public StageProfile Profile { get; } = new StageProfile();
        public GameplayDay CurrentDay { get; private set; }
        public CampaignPhase Phase { get; private set; }
        public IReadOnlyList<DayResult> Days { get; }
        public IReadOnlyList<VisitRecord> History { get; }
        public IReadOnlyList<CampaignClientState> Clients { get; }
        public int TotalPoints => accumulatedPoints + (CurrentDay.Phase == DayPhase.Result ? 0 : CurrentDay.Resources.TotalPoints);
        public int Progress { get; private set; }
        public int PresentPointBalance { get; private set; }
        public CampaignResult Result { get; private set; }

        public CampaignSession(IEnumerable<RosterEntry> roster, IEnumerable<Exercise> exercises, Random random, int dailyStamina = 100)
        {
            this.roster = new List<RosterEntry>(roster);
            this.exercises = new List<Exercise>(exercises);
            this.random = random ?? throw new ArgumentNullException(nameof(random));
            if (dailyStamina <= 0) throw new ArgumentOutOfRangeException(nameof(dailyStamina));
            this.dailyStamina = dailyStamina;
            Days = days.AsReadOnly(); History = history.AsReadOnly(); Clients = clients.AsReadOnly();
            foreach (var entry in this.roster) clients.Add(new CampaignClientState(entry));
            PrepareDay();
        }

        private void PrepareDay()
        {
            CurrentDay = new GameplayDay(roster, exercises, random, dailyStamina, 1f,
                accumulatedPoints, Progress, PresentPointBalance, days.Count + 1);
            CurrentDay.Completed += RecordDay;
            CurrentDay.Begin();
        }

        private void RecordDay(DayResult result)
        {
            if (result.DayNumber != days.Count + 1) return;
            days.Add(result);
            accumulatedPoints = result.AccumulatedPoints;
            Progress = result.Progress; PresentPointBalance = result.PresentPointBalance;
            foreach (var visit in result.Visits)
            {
                history.Add(visit);
                clients.Find(client => client.Entry.Client.Id == visit.ClientId).CompletedVisits++;
            }
            if (days.Count == Profile.ClientsPerDay.Count)
            {
                Phase = CampaignPhase.Completed;
                Result = new CampaignResult(days.Count, history.Count, accumulatedPoints, Progress, Profile.ProgressTarget);
            }
        }

        public bool ContinueToNextDay()
        {
            if (Phase == CampaignPhase.Completed || CurrentDay.Phase != DayPhase.Result) return false;
            PrepareDay();
            return true;
        }
    }
}

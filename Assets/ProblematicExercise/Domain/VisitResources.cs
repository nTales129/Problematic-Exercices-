using System;
using System.Collections.Generic;

namespace ProblematicExercise.Domain
{
    public sealed class VisitRecord
    {
        public string ClientId { get; }
        public string ClientName { get; }
        public string ExerciseId { get; }
        public string ExerciseName { get; }
        public Grade Grade { get; }
        public Preference Preference { get; }
        public int Points { get; }
        public int StaminaAfter { get; }

        internal VisitRecord(Client client, Exercise exercise, Grade grade, Preference preference, int stamina)
        {
            ClientId = client.Id; ClientName = client.Name;
            ExerciseId = exercise.Id; ExerciseName = exercise.Name;
            Grade = grade; Preference = preference;
            Points = VisitRules.Points(grade);
            StaminaAfter = stamina;
        }
    }

    // Shared by all visits in a day. A legacy one-visit fixture owns its own instance.
    public sealed class VisitResources
    {
        private readonly List<VisitRecord> history = new List<VisitRecord>();
        public IReadOnlyList<VisitRecord> History { get; }
        public int Stamina { get; private set; }
        public int MaximumStamina { get; }
        public float CostModifier { get; }
        public int TotalPoints { get; private set; }

        public VisitResources(int initialStamina, float costModifier = 1f)
        {
            if (initialStamina <= 0) throw new ArgumentOutOfRangeException(nameof(initialStamina));
            if (float.IsNaN(costModifier) || float.IsInfinity(costModifier)) throw new ArgumentOutOfRangeException(nameof(costModifier));
            Stamina = MaximumStamina = initialStamina;
            CostModifier = costModifier;
            History = history.AsReadOnly();
        }

        internal void Consume(Exercise exercise) => Stamina = VisitRules.ChangeStamina(Stamina, MaximumStamina,
            VisitRules.StaminaDelta(exercise.Rarity, CostModifier));

        internal VisitRecord Record(Client client, Exercise exercise, Grade grade, Preference preference)
        {
            var result = new VisitRecord(client, exercise, grade, preference, Stamina);
            history.Add(result);
            TotalPoints = unchecked(TotalPoints + result.Points);
            return result;
        }
    }
}

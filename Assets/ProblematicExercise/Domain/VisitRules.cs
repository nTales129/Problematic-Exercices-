using System;
using System.Collections.Generic;

namespace ProblematicExercise.Domain
{
    public enum Preference { Likes, Dislikes, Ambivalent, Favorite }
    public enum Grade { D, C, B, A, S, W }

    public sealed class Exercise
    {
        public string Id { get; }
        public string Name { get; }
        public int Rarity { get; }
        public IReadOnlyList<string> Tags { get; }

        public Exercise(string id, string name, int rarity, params string[] tags)
        {
            if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException("Exercise ID required.", nameof(id));
            VisitRules.BaseCost(rarity);
            Id = id;
            Name = name;
            Rarity = rarity;
            Tags = Array.AsReadOnly((string[])tags.Clone());
        }
    }

    public sealed class Client
    {
        private readonly Dictionary<string, Preference> skillPreferences;
        private readonly Dictionary<string, Preference> tagPreferences;
        public string Id { get; }
        public string Name { get; }

        public Client(string id, string name, IDictionary<string, Preference> tags,
            IDictionary<string, Preference> skills = null)
        {
            if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException("Client ID required.", nameof(id));
            Id = id;
            Name = name;
            tagPreferences = new Dictionary<string, Preference>(tags);
            skillPreferences = skills == null ? new Dictionary<string, Preference>() : new Dictionary<string, Preference>(skills);
        }

        public Preference Evaluate(Exercise exercise)
        {
            int tally = 0;
            if (skillPreferences.TryGetValue(exercise.Id, out var preference))
                tally = unchecked(tally + VisitRules.Contribution(preference));
            foreach (var tag in exercise.Tags)
                if (tagPreferences.TryGetValue(tag, out preference))
                    tally = unchecked(tally + VisitRules.Contribution(preference));
            return VisitRules.Classify(tally);
        }
    }

    public static class VisitRules
    {
        public static int Contribution(Preference preference)
        {
            switch (preference)
            {
                // Confirmed convergence. Do not rebalance Dislikes to -1.
                case Preference.Likes:
                case Preference.Dislikes: return 1;
                case Preference.Ambivalent: return 0;
                case Preference.Favorite: return 25;
                default: throw new ArgumentOutOfRangeException(nameof(preference));
            }
        }

        public static Preference Classify(int tally) => tally > 20 ? Preference.Favorite
            : tally > 0 ? Preference.Likes : tally == 0 ? Preference.Ambivalent : Preference.Dislikes;

        public static Grade EvaluateGrade(int rarity, Preference preference)
        {
            if (preference == Preference.Favorite) return Grade.W;
            if (rarity < 0 || rarity > 3) throw new ArgumentOutOfRangeException(nameof(rarity));
            int adjustment;
            switch (preference)
            {
                case Preference.Likes: adjustment = 1; break;
                case Preference.Dislikes: adjustment = -1; break;
                case Preference.Ambivalent: adjustment = 0; break;
                default: throw new ArgumentOutOfRangeException(nameof(preference));
            }
            return (Grade)(rarity + 1 + adjustment);
        }

        public static int Points(Grade grade)
        {
            switch (grade)
            {
                case Grade.D: return 10;
                case Grade.C: return 14;
                case Grade.B: return 18;
                case Grade.A: return 22;
                case Grade.S: return 26;
                case Grade.W: return 34;
                default: throw new ArgumentOutOfRangeException(nameof(grade));
            }
        }

        public static int BaseCost(int rarity)
        {
            switch (rarity)
            {
                case 0: return 10;
                case 1: return 25;
                case 2: return 40;
                case 3: return 55;
                default: throw new ArgumentOutOfRangeException(nameof(rarity));
            }
        }

        public static float ReduceCostModifier(float modifier, float percent) => modifier - percent / 100f;

        public static int StaminaDelta(int rarity, float modifier)
        {
            float product = -BaseCost(rarity) * modifier;
            return checked((int)Math.Floor((double)product));
        }

        public static int ChangeStamina(int current, int maximum, int delta, bool infinite = false, bool revive = false)
        {
            if (maximum < 0 || current < 0 || current > maximum) throw new ArgumentOutOfRangeException(nameof(current));
            if (infinite || (current == 0 && !revive)) return current;
            long value = (long)current + delta;
            return (int)Math.Max(0, Math.Min(maximum, value));
        }
    }
}

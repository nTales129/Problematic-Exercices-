using System;
using System.Collections.Generic;

namespace ProblematicExercise.Domain
{
    public sealed class ExerciseDeck
    {
        private readonly List<Exercise> draw;
        private readonly List<Exercise> discard = new List<Exercise>();
        private readonly List<Exercise> drawn = new List<Exercise>();
        private readonly Random random;
        public int HandSize { get; private set; } = 3;
        public int DrawCount => draw.Count;
        public int DiscardCount => discard.Count;
        public int DrawnCount => drawn.Count;
        public int RecycleCount { get; private set; }

        public ExerciseDeck(IEnumerable<Exercise> exercises, Random random)
        {
            this.random = random ?? throw new ArgumentNullException(nameof(random));
            draw = new List<Exercise>(exercises);
            Shuffle(draw, random);
        }

        // No required-tag override is configured in this bounded slice.
        public IReadOnlyList<Exercise> Draw(int amount = -1)
        {
            if (amount == -1) amount = Math.Max(0, Math.Min(5, HandSize));
            if (amount < 0) throw new ArgumentOutOfRangeException(nameof(amount));
            var result = new List<Exercise>();
            for (int i = 0; i < amount; i++)
            {
                if (draw.Count == 0)
                {
                    if (discard.Count > 0) RecycleCount++;
                    draw.AddRange(discard);
                    discard.Clear();
                    Shuffle(draw, random);
                }
                if (draw.Count == 0) break;
                var card = draw[0];
                draw.RemoveAt(0);
                result.Add(card);
                drawn.Add(card);
            }
            return result.AsReadOnly();
        }

        public void IncreaseHandSize(int amount) => HandSize += amount;

        public void DiscardDrawn()
        {
            discard.AddRange(drawn);
            drawn.Clear();
        }

        // Independent technical adaptation: the original shuffle implementation is unknown.
        public static void Shuffle<T>(IList<T> items, Random random)
        {
            for (int i = items.Count - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                T value = items[i];
                items[i] = items[j];
                items[j] = value;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using ProblematicExercise.Domain;

namespace ProblematicExercise.Runtime
{
    public static class VisitSample
    {
        // Original, adult gym clients and content. These are explicitly documented fixtures.
        public static FirstVisit Create(int seed) => new FirstVisit(CreateClients(), CreateExercises(), new Random(seed), 100);

        public static GameplayDay CreateDay(int seed)
        {
            var entries = new List<RosterEntry>();
            foreach (var client in CreateClients()) entries.Add(new RosterEntry(client, 1));
            return new GameplayDay(entries, CreateExercises(), new Random(seed));
        }

        public static Client[] CreateClients() => new[]
        {
            new Client("pe.client.lia", "Lia", new Dictionary<string, Preference>
                { ["mobility"] = Preference.Likes, ["strength"] = Preference.Dislikes }),
            new Client("pe.client.caio", "Caio", new Dictionary<string, Preference>
                { ["strength"] = Preference.Likes, ["cardio"] = Preference.Dislikes }),
            new Client("pe.client.nara", "Nara", new Dictionary<string, Preference>
                { ["cardio"] = Preference.Favorite, ["mobility"] = Preference.Ambivalent })
        };

        public static Exercise[] CreateExercises() => new[]
        {
            new Exercise("pe.exercise.mobility", "Mobilidade", 0, "mobility"),
            new Exercise("pe.exercise.posture", "Postura", 0, "control"),
            new Exercise("pe.exercise.row", "Remada", 1, "strength"),
            new Exercise("pe.exercise.balance", "Equilíbrio", 1, "mobility", "control"),
            new Exercise("pe.exercise.circuit", "Circuito", 2, "cardio"),
            new Exercise("pe.exercise.advanced", "Série avançada", 3, "strength", "cardio")
        };
    }
}

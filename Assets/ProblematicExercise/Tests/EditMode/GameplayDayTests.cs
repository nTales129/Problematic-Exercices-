using System;
using System.Collections.Generic;
using NUnit.Framework;
using ProblematicExercise.Domain;
using ProblematicExercise.Runtime;

namespace ProblematicExercise.Tests
{
    public class GameplayDayTests
    {
        private static void CompleteVisit(GameplayDay day, bool expensive = false)
        {
            var visit = day.CurrentVisit;
            Assert.IsTrue(day.ChooseClient(visit.Offered[0].Id));
            Assert.IsTrue(visit.FinishEntry());
            Exercise chosen = visit.Hand[0];
            foreach (var card in visit.Hand)
                if (expensive ? card.Rarity > chosen.Rarity : card.Rarity < chosen.Rarity) chosen = card;
            Assert.IsTrue(visit.Confirm(chosen.Id));
            Assert.IsTrue(visit.FinishActivity());
            Assert.IsFalse(visit.FinishActivity());
            Assert.IsTrue(visit.Continue());
            Assert.IsTrue(day.FinishExit());
        }

        [Test]
        public void ThreeVisitsKeepDeckStaminaScoreAndUniqueClients()
        {
            var day = VisitSample.CreateDay(17);
            day.Begin();
            var deck = day.Deck;
            var ledger = day.Resources;
            var ids = new HashSet<string>();
            int expectedStamina = 100, points = 0;
            for (int i = 0; i < 3; i++)
            {
                Assert.AreSame(deck, day.CurrentVisit.Deck);
                Assert.AreSame(ledger, day.Resources);
                Assert.AreEqual(3 - i, day.Roster.Available.Count);
                if (i == 2) Assert.AreEqual(1, day.CurrentVisit.Offered.Count);
                CompleteVisit(day);
                var record = day.Resources.History[i];
                Assert.IsTrue(ids.Add(record.ClientId));
                int rarity = Array.Find(VisitSample.CreateExercises(), card => card.Id == record.ExerciseId).Rarity;
                expectedStamina = Math.Max(0, expectedStamina - VisitRules.BaseCost(rarity));
                points += record.Points;
                Assert.AreEqual(expectedStamina, day.Resources.Stamina);
                Assert.AreEqual(points, day.Resources.TotalPoints);
                Assert.AreEqual(i + 1, day.CompletedVisits);
                Assert.AreEqual(6, deck.DrawCount + deck.DiscardCount + deck.DrawnCount);
                Assert.AreEqual(0, deck.DrawnCount);
            }
            Assert.AreEqual(DayPhase.Result, day.Phase);
            Assert.AreEqual(EndDayReason.OutOfClients, day.Result.Reason);
            Assert.AreEqual(3, day.Result.Visits.Count);
            Assert.AreEqual(1, deck.RecycleCount);
            Assert.AreEqual(points, day.Result.DayPoints);
            Assert.AreEqual(points, day.Result.AccumulatedPoints);
            Assert.AreEqual(points, day.Result.Progress);
            Assert.AreEqual(points, day.Result.PresentPointsGained);
            Assert.IsFalse(day.FinishExit());
            Assert.AreEqual(points, day.Result.DayPoints);
        }

        [Test]
        public void KnockoutWaitsForActivityResultAndExitBeforeEndingDay()
        {
            var entries = new List<RosterEntry>();
            foreach (var client in VisitSample.CreateClients()) entries.Add(new RosterEntry(client, 1));
            var day = new GameplayDay(entries, new[] { new Exercise("costly", "Test", 3) }, new Random(3), initialStamina: 5);
            day.Begin();
            var visit = day.CurrentVisit;
            day.ChooseClient(visit.Offered[0].Id); visit.FinishEntry();
            Assert.AreEqual(1, visit.Hand.Count); // Short hand is preserved, not padded.
            visit.Confirm("costly");
            Assert.AreEqual(0, day.Resources.Stamina);
            Assert.AreEqual(DayPhase.Visiting, day.Phase);
            Assert.IsNull(day.Result);
            Assert.IsFalse(day.FinishExit());
            visit.FinishActivity();
            Assert.AreEqual(1, day.Resources.History.Count);
            Assert.IsNull(day.Result);
            visit.Continue();
            day.FinishExit();
            Assert.AreEqual(EndDayReason.OutOfStamina, day.Result.Reason);
            Assert.AreEqual(1, day.CompletedVisits);
            Assert.AreEqual(2, day.Roster.Available.Count);
        }

        [Test]
        public void StaminaWinsOverSimultaneousLastClientExhaustion()
        {
            var day = VisitSample.CreateDay(17);
            day.Begin();
            for (int i = 0; i < 3; i++) CompleteVisit(day, expensive: true);
            Assert.AreEqual(0, day.Resources.Stamina);
            Assert.AreEqual(0, day.Roster.Available.Count);
            Assert.AreEqual(EndDayReason.OutOfStamina, day.Result.Reason);
            Assert.AreEqual(3, day.Result.Visits.Count);
        }

        [Test]
        public void RosterPrioritizesPositiveLevelsThenComplementsWithoutRepeats()
        {
            var clients = VisitSample.CreateClients();
            var extra = new Client("extra", "Adult test client", new Dictionary<string, Preference>());
            var entries = new[] { new RosterEntry(clients[0], 0), new RosterEntry(clients[1], 2),
                new RosterEntry(clients[2], 3), new RosterEntry(extra, 0) };
            var priority = new ClientRoster(entries, 2, false, new Random(2));
            CollectionAssert.AreEquivalent(new[] { clients[1], clients[2] }, priority.Available);
            var complement = new ClientRoster(entries, 3, false, new Random(2));
            Assert.AreEqual(3, complement.Available.Count);
            CollectionAssert.Contains(complement.Available, clients[1]);
            CollectionAssert.Contains(complement.Available, clients[2]);
            Assert.AreEqual(3, new HashSet<Client>(complement.Available).Count);
        }

        [Test]
        public void ShortRosterIsNotDuplicatedToFillQuota()
        {
            var client = VisitSample.CreateClients()[0];
            var day = new GameplayDay(new[] { new RosterEntry(client, 1) }, VisitSample.CreateExercises(), new Random(1));
            day.Begin(); CompleteVisit(day);
            Assert.AreEqual(EndDayReason.OutOfClients, day.Result.Reason);
            Assert.AreEqual(1, day.CompletedVisits);
        }

        [Test]
        public void ProgressClampsAndPresentPointsRemainPoints()
        {
            var entries = new List<RosterEntry>();
            foreach (var client in VisitSample.CreateClients()) entries.Add(new RosterEntry(client, 1));
            var day = new GameplayDay(entries, VisitSample.CreateExercises(), new Random(17), previousPoints: 80,
                previousProgress: 495, previousPresentPoints: 9);
            day.Begin();
            for (int i = 0; i < 3; i++) CompleteVisit(day);
            Assert.AreEqual(500, day.Result.Progress);
            Assert.AreEqual(80 + day.Result.DayPoints, day.Result.AccumulatedPoints);
            Assert.AreEqual(9 + day.Result.DayPoints, day.Result.PresentPointBalance);
            CollectionAssert.AreEqual(new[] { 3, 4, 5, 6, 6 }, day.Profile.ClientsPerDay);
            Assert.IsFalse(day.Begin()); // No automatic day 2, no result reapplication.
        }

        [Test]
        public void NewSessionDoesNotReusePreviousResourcesOrHistory()
        {
            var first = VisitSample.CreateDay(17); first.Begin(); CompleteVisit(first);
            var next = VisitSample.CreateDay(17); next.Begin();
            Assert.AreNotSame(first.Deck, next.Deck);
            Assert.AreNotSame(first.Resources, next.Resources);
            Assert.AreEqual(100, next.Resources.Stamina);
            Assert.AreEqual(0, next.Resources.TotalPoints);
            Assert.AreEqual(0, next.Resources.History.Count);
            Assert.AreEqual(3, next.Roster.Available.Count);
        }

        [Test]
        public void InvalidOrRepeatedClientCommandsDoNotConsumeRoster()
        {
            var day = VisitSample.CreateDay(17); day.Begin();
            Assert.IsFalse(day.ChooseClient("missing"));
            Assert.AreEqual(3, day.Roster.Available.Count);
            string id = day.CurrentVisit.Offered[0].Id;
            Assert.IsTrue(day.ChooseClient(id));
            Assert.IsFalse(day.ChooseClient(id));
            Assert.AreEqual(2, day.Roster.Available.Count);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using ProblematicExercise.Domain;
using ProblematicExercise.Runtime;

namespace ProblematicExercise.Tests
{
    public class CampaignTests
    {
        private static void Visit(GameplayDay day, bool costly = false)
        {
            var visit = day.CurrentVisit;
            Assert.IsTrue(day.ChooseClient(visit.Offered[0].Id));
            Assert.IsTrue(visit.FinishEntry());
            var card = costly ? visit.Hand.OrderByDescending(c => c.Rarity).First() : visit.Hand.OrderBy(c => c.Rarity).First();
            Assert.IsTrue(visit.Confirm(card.Id));
            Assert.IsTrue(visit.FinishActivity());
            Assert.IsFalse(visit.FinishActivity());
            Assert.IsTrue(visit.Continue());
            Assert.IsTrue(day.FinishExit());
        }

        private static CampaignSession Synthetic(int stamina = 100, bool favorite = false)
        {
            var roster = Enumerable.Range(0, 6).Select(i => new RosterEntry(new Client("adult" + i, "Adult " + i,
                new Dictionary<string, Preference> { ["test"] = favorite ? Preference.Favorite : Preference.Ambivalent }), i < 3 ? 1 : 0));
            return new CampaignSession(roster, new[] { new Exercise("cheap", "Own test", 0, "test") }, new Random(17), stamina);
        }

        [Test]
        public void FiveDaysHaveExactQuotasUniqueDailyHistoryAndSessionTotals()
        {
            var campaign = Synthetic();
            int total = 0, visits = 0;
            foreach (int quota in new[] { 3, 4, 5, 6, 6 })
            {
                var day = campaign.CurrentDay;
                Assert.AreEqual(campaign.Days.Count + 1, day.DayNumber);
                Assert.AreEqual(quota, day.Quota);
                while (day.Phase != DayPhase.Result) Visit(day);
                Assert.AreEqual(quota, day.CompletedVisits);
                Assert.AreEqual(quota, day.Result.Visits.Select(v => v.ClientId).Distinct().Count());
                visits += quota; total += quota * 14;
                Assert.AreEqual(visits, campaign.History.Count);
                Assert.AreEqual(total, campaign.TotalPoints);
                Assert.AreEqual(total, campaign.Progress);
                Assert.AreEqual(total, campaign.PresentPointBalance);
                Assert.AreEqual(visits, campaign.Clients.Sum(c => c.CompletedVisits));
                if (day.DayNumber < 5) Assert.IsTrue(campaign.ContinueToNextDay());
            }
            Assert.AreEqual(CampaignPhase.Completed, campaign.Phase);
            Assert.AreEqual(24, campaign.Result.VisitsCompleted);
            Assert.AreEqual(5, campaign.Result.DaysCompleted);
            Assert.AreEqual(336, campaign.Result.TotalPoints);
            Assert.IsFalse(campaign.Result.GoalReached);
            Assert.IsFalse(campaign.ContinueToNextDay());
        }

        [Test]
        public void DailyDeckAndStaminaResetWhileSessionStatePersists()
        {
            var campaign = VisitSample.CreateCampaign(17);
            var day = campaign.CurrentDay;
            var deck = day.Deck;
            Visit(day);
            Assert.AreSame(deck, day.CurrentVisit.Deck);
            Assert.Less(day.Resources.Stamina, 100);
            Assert.Greater(campaign.TotalPoints, 0);
            while (day.Phase != DayPhase.Result) Visit(day);
            int points = campaign.TotalPoints;
            Assert.IsTrue(campaign.ContinueToNextDay());
            Assert.AreNotSame(deck, campaign.CurrentDay.Deck);
            Assert.AreEqual(100, campaign.CurrentDay.Resources.Stamina);
            Assert.AreEqual(0, campaign.CurrentDay.Resources.TotalPoints);
            Assert.AreEqual(6, campaign.CurrentDay.Deck.DrawCount);
            Assert.AreEqual(0, campaign.CurrentDay.Deck.DiscardCount);
            Assert.AreEqual(points, campaign.TotalPoints);
            Assert.AreEqual(3, campaign.History.Count);
        }

        [Test]
        public void KnockoutFinishesVisitAndEndsOnlyCurrentDay()
        {
            var campaign = Synthetic(stamina: 5);
            for (int number = 1; number <= 5; number++)
            {
                var day = campaign.CurrentDay;
                var visit = day.CurrentVisit;
                day.ChooseClient(visit.Offered[0].Id); visit.FinishEntry(); visit.Confirm(visit.Hand[0].Id);
                Assert.AreEqual(0, day.Resources.Stamina);
                Assert.AreEqual(DayPhase.Visiting, day.Phase);
                Assert.IsFalse(campaign.ContinueToNextDay());
                visit.FinishActivity(); visit.Continue(); day.FinishExit();
                Assert.AreEqual(EndDayReason.OutOfStamina, day.Result.Reason);
                Assert.AreEqual(1, day.Result.Visits.Count);
                if (number < 5) Assert.IsTrue(campaign.ContinueToNextDay());
            }
            Assert.AreEqual(5, campaign.Result.VisitsCompleted);
        }

        [Test]
        public void ResultsAreAppliedOnceAndOldDaysCannotAdvanceOrCreditAgain()
        {
            var campaign = Synthetic();
            var old = campaign.CurrentDay;
            while (old.Phase != DayPhase.Result) Visit(old);
            var result = old.Result;
            Assert.IsFalse(old.FinishExit()); Assert.IsFalse(old.CurrentVisit.FinishActivity());
            Assert.IsFalse(old.CurrentVisit.Continue());
            Assert.AreSame(result, old.Result);
            Assert.AreEqual(42, campaign.TotalPoints);
            Assert.IsTrue(campaign.ContinueToNextDay());
            Assert.IsFalse(campaign.ContinueToNextDay());
            Assert.IsFalse(old.FinishExit());
            Assert.AreEqual(1, campaign.Days.Count); Assert.AreEqual(42, campaign.TotalPoints);
        }

        [Test]
        public void ProgressSaturatesButTotalAndPresentPointsKeepAccumulating()
        {
            var campaign = Synthetic(favorite: true);
            while (true)
            {
                while (campaign.CurrentDay.Phase != DayPhase.Result) Visit(campaign.CurrentDay);
                if (!campaign.ContinueToNextDay()) break;
            }
            Assert.AreEqual(24 * 34, campaign.TotalPoints);
            Assert.AreEqual(500, campaign.Progress);
            Assert.AreEqual(816, campaign.PresentPointBalance);
            Assert.IsTrue(campaign.Result.GoalReached);
        }

        [Test]
        public void NewSessionAfterCompletionHasNoSharedMutableState()
        {
            var old = Synthetic(5);
            do { Visit(old.CurrentDay); } while (old.ContinueToNextDay());
            var fresh = Synthetic();
            Assert.AreEqual(CampaignPhase.Completed, old.Phase);
            Assert.AreEqual(0, fresh.TotalPoints); Assert.AreEqual(0, fresh.Progress);
            Assert.AreEqual(0, fresh.History.Count); Assert.AreEqual(0, fresh.Days.Count);
            Assert.IsNull(fresh.Result); Assert.AreEqual(100, fresh.CurrentDay.Resources.Stamina);
            Assert.IsTrue(fresh.Clients.All(c => c.CompletedVisits == 0));
        }

        [Test]
        public void ProductionCatalogKeepsFirstDayAndComplementsPriorityAtLaterQuotas()
        {
            var campaign = VisitSample.CreateCampaign(17);
            var legacy = VisitSample.CreateDay(17); legacy.Begin();
            CollectionAssert.AreEqual(legacy.Roster.Available.Select(c => c.Id), campaign.CurrentDay.Roster.Available.Select(c => c.Id));
            CollectionAssert.AreEqual(legacy.CurrentVisit.Offered.Select(c => c.Id), campaign.CurrentDay.CurrentVisit.Offered.Select(c => c.Id));
            Assert.AreEqual(6, campaign.Clients.Select(c => c.Entry.Client.Id).Distinct().Count());
            var priority = campaign.Clients.Where(c => c.Entry.Level > 0).Select(c => c.Entry.Client.Id).ToArray();
            for (int number = 1; number <= 5; number++)
            {
                var day = campaign.CurrentDay;
                CollectionAssert.IsSubsetOf(priority, day.Roster.Available.Select(c => c.Id).ToArray());
                while (day.Phase != DayPhase.Result) Visit(day);
                if (number < 5) Assert.IsTrue(campaign.ContinueToNextDay());
            }
            Assert.IsTrue(campaign.Clients.All(c => c.Entry.Level == (priority.Contains(c.Entry.Client.Id) ? 1 : 0)));
        }

        [TestCase(0)]
        [TestCase(6)]
        public void InvalidDayCannotWrapOrRepeat(int number)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new GameplayDay(new[] { new RosterEntry(new Client("x", "Adult", new Dictionary<string, Preference>()), 1) },
                new[] { new Exercise("x", "Test", 0) }, new Random(1), dayNumber: number));
        }
    }
}

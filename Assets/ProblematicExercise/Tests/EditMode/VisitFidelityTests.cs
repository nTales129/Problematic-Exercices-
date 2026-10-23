using System;
using System.Collections.Generic;
using NUnit.Framework;
using ProblematicExercise.Domain;
using ProblematicExercise.Runtime;

namespace ProblematicExercise.Tests
{
    public class VisitFidelityTests
    {
        [TestCase(Preference.Likes, 1, Preference.Likes)]
        [TestCase(Preference.Dislikes, 1, Preference.Likes)]
        [TestCase(Preference.Ambivalent, 0, Preference.Ambivalent)]
        [TestCase(Preference.Favorite, 25, Preference.Favorite)]
        public void IndividualPreferenceContributesAsRecovered(Preference input, int tally, Preference aggregate)
        {
            Assert.AreEqual(tally, VisitRules.Contribution(input));
            var client = new Client("test", "Adult test client", new Dictionary<string, Preference> { ["tag"] = input });
            Assert.AreEqual(aggregate, client.Evaluate(new Exercise("skill", "Test exercise", 0, "tag")));
        }

        [TestCase(-1, Preference.Dislikes)]
        [TestCase(0, Preference.Ambivalent)]
        [TestCase(1, Preference.Likes)]
        [TestCase(20, Preference.Likes)]
        [TestCase(21, Preference.Favorite)]
        public void AggregationBoundaries(int tally, Preference expected) => Assert.AreEqual(expected, VisitRules.Classify(tally));

        [Test]
        public void SkillPreferenceDoesNotReplaceTagMatches()
        {
            var client = new Client("test", "Adult", new Dictionary<string, Preference> { ["tag"] = Preference.Favorite },
                new Dictionary<string, Preference> { ["skill"] = Preference.Dislikes });
            Assert.AreEqual(Preference.Favorite, client.Evaluate(new Exercise("skill", "Exercise", 0, "tag")));
        }

        [TestCase(0, Preference.Dislikes, Grade.D, 10)]
        [TestCase(0, Preference.Ambivalent, Grade.C, 14)]
        [TestCase(0, Preference.Likes, Grade.B, 18)]
        [TestCase(1, Preference.Likes, Grade.A, 22)]
        [TestCase(2, Preference.Likes, Grade.S, 26)]
        [TestCase(3, Preference.Likes, Grade.W, 34)]
        [TestCase(0, Preference.Favorite, Grade.W, 34)]
        [TestCase(99, Preference.Favorite, Grade.W, 34)]
        public void GradeAndPointsExamples(int rarity, Preference preference, Grade expected, int points)
        {
            var grade = VisitRules.EvaluateGrade(rarity, preference);
            Assert.AreEqual(expected, grade);
            Assert.AreEqual(points, VisitRules.Points(grade));
        }

        [TestCase(0, 10)] [TestCase(1, 25)] [TestCase(2, 40)] [TestCase(3, 55)]
        public void ExactStaminaCosts(int rarity, int cost) => Assert.AreEqual(-cost, VisitRules.StaminaDelta(rarity, 1f));

        [Test]
        public void ReducedCostUsesNegativeFloorAndSaturation()
        {
            float modifier = VisitRules.ReduceCostModifier(1f, 30f);
            Assert.AreEqual(-18, VisitRules.StaminaDelta(1, modifier));
            Assert.AreEqual(0, VisitRules.ChangeStamina(5, 100, -55));
            Assert.AreEqual(100, VisitRules.ChangeStamina(95, 100, 20));
            Assert.AreEqual(0, VisitRules.ChangeStamina(0, 100, 20));
            Assert.AreEqual(20, VisitRules.ChangeStamina(0, 100, 20, revive: true));
            Assert.AreEqual(5, VisitRules.ChangeStamina(5, 100, -55, infinite: true));
        }

        [Test]
        public void NormalDrawClampsButExplicitDrawDoesNotAndEmptyReturnsShort()
        {
            var cards = new List<Exercise>();
            for (int i = 0; i < 7; i++) cards.Add(new Exercise("test" + i, "Test", 0));
            var deck = new ExerciseDeck(cards, new Random(4));
            deck.IncreaseHandSize(100);
            Assert.AreEqual(5, deck.Draw().Count);
            deck.DiscardDrawn();
            Assert.AreEqual(7, deck.Draw(7).Count);
            Assert.AreEqual(0, deck.Draw().Count); // All cards are drawn, not yet discarded.
            deck.DiscardDrawn();
            Assert.AreEqual(7, deck.DiscardCount);
            Assert.AreEqual(7, deck.Draw(10).Count); // No emergency card, no duplicate recycled drawn card.
        }

        [Test]
        public void VisitRemovesClientDiscardsWholeHandAndCreditsExactlyOnce()
        {
            var visit = VisitSample.Create(17);
            Assert.IsTrue(visit.Start());
            Assert.That(visit.Offered.Count, Is.InRange(2, 3));
            Assert.IsFalse(visit.ChooseClient("absent"));
            Assert.IsTrue(visit.ChooseClient(visit.Offered[0].Id));
            Assert.AreEqual(2, visit.AvailableCount);
            Assert.IsFalse(visit.FinishActivity());
            Assert.IsTrue(visit.FinishEntry());
            Assert.AreEqual(3, visit.Hand.Count);
            var card = visit.Hand[0];
            Assert.IsTrue(visit.Select(card.Id));
            Assert.AreEqual(100, visit.Stamina);
            Assert.AreEqual(0, visit.TotalPoints);
            Assert.IsFalse(visit.Confirm("absent"));
            Assert.IsTrue(visit.Confirm(card.Id));
            Assert.AreEqual(0, visit.Hand.Count);
            Assert.AreEqual(3, visit.Deck.DiscardCount);
            Assert.AreEqual(0, visit.Deck.DrawnCount);
            Assert.AreEqual(100 - VisitRules.BaseCost(card.Rarity), visit.Stamina);
            Assert.IsFalse(visit.Confirm(card.Id));
            Assert.IsTrue(visit.FinishActivity());
            Assert.IsFalse(visit.FinishActivity());
            int points = visit.TotalPoints;
            Assert.AreEqual(VisitRules.Points(visit.ResultGrade), points);
            Assert.AreEqual(1, visit.ResultCount);
            Assert.IsTrue(visit.Continue());
            Assert.IsFalse(visit.Continue());
            Assert.IsTrue(visit.FinishExit());
            Assert.AreEqual(VisitPhase.Complete, visit.Phase);
            Assert.AreEqual(points, visit.TotalPoints);
            Assert.IsFalse(visit.Start());
        }

        [Test]
        public void ZeroStaminaAfterConfirmationStillFinishesVisit()
        {
            var client = new Client("test", "Adult", new Dictionary<string, Preference>());
            var visit = new FirstVisit(new[] { client, new Client("other", "Other adult", new Dictionary<string, Preference>()) },
                new[] { new Exercise("costly", "Exercise", 3) }, new Random(1), 5);
            visit.Start(); visit.ChooseClient(visit.Offered[0].Id); visit.FinishEntry();
            Assert.IsTrue(visit.Confirm("costly"));
            Assert.AreEqual(0, visit.Stamina);
            Assert.AreEqual(VisitPhase.Activity, visit.Phase);
            Assert.IsTrue(visit.FinishActivity());
            Assert.AreEqual(26, visit.TotalPoints);
            Assert.IsTrue(visit.Continue());
            Assert.IsTrue(visit.FinishExit());
        }
    }
}

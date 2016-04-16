using System;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using dab.SGS.Core;
using dab.SGS.Core.PlayingCards;
using dab.SGS.Core.Actions;
using System.Collections.Generic;
using dab.SGS.Core.PlayingCards.Basics;
using dab.SGS.Core.PlayingCards.Scrolls;
using dab.SGS.Core.PlayingCards.Equipments;

namespace dab.SGS.Core.Unit.Gameplay
{
    [TestClass]
    public class ValidCards
    {
        private class PlayingCardPlayableTestor
        {
            public PlayingCard Card { get; set; }
            public bool Expected { get; set; }
            public PlayingCardPlayableTestor(PlayingCard card, bool expected)
            {
                this.Card = card;
                this.Expected = expected;
            }
        }

        [TestMethod]
        public void TestValidCardsDuringPlayFullHealth()
        {
            var cards = new List<PlayingCardPlayableTestor>()
            {
                new PlayingCardPlayableTestor(new WineBasicPlayingCard(PlayingCardColor.Red, PlayingCardSuite.Club, "", null), true),
                new PlayingCardPlayableTestor(new PeachBasicPlayingCard(PlayingCardColor.Red, PlayingCardSuite.Club, "", null), false),

                new PlayingCardPlayableTestor(new DodgeBasicPlayingCard(PlayingCardColor.Red, PlayingCardSuite.Club, "", null), false),

                new PlayingCardPlayableTestor(new AttackBasicPlayingCard(PlayingCardColor.Red, PlayingCardSuite.Club, "", null, Elemental.None), true),
                new PlayingCardPlayableTestor(new AttackBasicPlayingCard(PlayingCardColor.Red, PlayingCardSuite.Club, "", null, Elemental.Lightning), true),
                new PlayingCardPlayableTestor(new AttackBasicPlayingCard(PlayingCardColor.Red, PlayingCardSuite.Club, "", null, Elemental.Fire), true),

                new PlayingCardPlayableTestor(new DuelScrollPlayingCard(PlayingCardColor.Red, PlayingCardSuite.Club, "", null), true),
                new PlayingCardPlayableTestor(new LightningDelayedScrollPlayingCard(PlayingCardColor.Red, PlayingCardSuite.Club, "", null), true),
                new PlayingCardPlayableTestor(new WardScrollPlayingCard(PlayingCardColor.Red, PlayingCardSuite.Club, "", null), false),
                new PlayingCardPlayableTestor(new StarvationDelayedScrollPlayingCard(PlayingCardColor.Red, PlayingCardSuite.Club, "", null), true),
                new PlayingCardPlayableTestor(new ContentmentDelayedScrollPlayingCard(PlayingCardColor.Red, PlayingCardSuite.Club, "", null), true),

                new PlayingCardPlayableTestor(new WeaponEquipmentPlayingCard(2, PlayingCardColor.Red, PlayingCardSuite.Club, "", null, null, null ,null), true),
                new PlayingCardPlayableTestor(new ShieldEquipmentPlayingCard(PlayingCardColor.Red, PlayingCardSuite.Club, "", null, null, null, null, null), true),
                new PlayingCardPlayableTestor(new HorseEquipmentPlayingCard(1, PlayingCardColor.Red, PlayingCardSuite.Club, "", null), true),
            };

            var ctx = new GameContext(null);
            var p = new Player("dab", 3, Roles.Minister);

            ctx.CurrentTurnStage = TurnStages.Play;

            foreach (var pcpt in cards)
            {
                pcpt.Card.Context = ctx;
                pcpt.Card.Owner = p;
                Assert.AreEqual(pcpt.Expected, pcpt.Card.IsPlayable(), String.Format("{0} failed the IsPlayable test here", pcpt.Card.ToString()));
            }


        }


        [TestMethod]
        public void TestValidCardsDuringPlayOneHealthAlreadyAttacked()
        {
            var cards = new List<PlayingCardPlayableTestor>()
            {
                new PlayingCardPlayableTestor(new WineBasicPlayingCard(PlayingCardColor.Red, PlayingCardSuite.Club, "", null), true),
                new PlayingCardPlayableTestor(new PeachBasicPlayingCard(PlayingCardColor.Red, PlayingCardSuite.Club, "", null), true),

                new PlayingCardPlayableTestor(new DodgeBasicPlayingCard(PlayingCardColor.Red, PlayingCardSuite.Club, "", null), false),

                new PlayingCardPlayableTestor(new AttackBasicPlayingCard(PlayingCardColor.Red, PlayingCardSuite.Club, "", null, Elemental.None), false),
                new PlayingCardPlayableTestor(new AttackBasicPlayingCard(PlayingCardColor.Red, PlayingCardSuite.Club, "", null, Elemental.Lightning), false),
                new PlayingCardPlayableTestor(new AttackBasicPlayingCard(PlayingCardColor.Red, PlayingCardSuite.Club, "", null, Elemental.Fire), false),

                new PlayingCardPlayableTestor(new DuelScrollPlayingCard(PlayingCardColor.Red, PlayingCardSuite.Club, "", null), true),
                new PlayingCardPlayableTestor(new LightningDelayedScrollPlayingCard(PlayingCardColor.Red, PlayingCardSuite.Club, "", null), true),
                new PlayingCardPlayableTestor(new WardScrollPlayingCard(PlayingCardColor.Red, PlayingCardSuite.Club, "", null), false),
                new PlayingCardPlayableTestor(new StarvationDelayedScrollPlayingCard(PlayingCardColor.Red, PlayingCardSuite.Club, "", null), true),
                new PlayingCardPlayableTestor(new ContentmentDelayedScrollPlayingCard(PlayingCardColor.Red, PlayingCardSuite.Club, "", null), true),

                new PlayingCardPlayableTestor(new WeaponEquipmentPlayingCard(2, PlayingCardColor.Red, PlayingCardSuite.Club, "", null, null, null ,null), true),
                new PlayingCardPlayableTestor(new ShieldEquipmentPlayingCard(PlayingCardColor.Red, PlayingCardSuite.Club, "", null, null, null, null, null), true),
                new PlayingCardPlayableTestor(new HorseEquipmentPlayingCard(1, PlayingCardColor.Red, PlayingCardSuite.Club, "", null), true),
            };

            var ctx = new GameContext(null);
            var p = new Player("dab", 3, Roles.Minister);

            p.CurrentHealth = 1;
            p.AttacksLeft = 0;

            ctx.CurrentTurnStage = TurnStages.Play;

            foreach (var pcpt in cards)
            {
                pcpt.Card.Context = ctx;
                pcpt.Card.Owner = p;
                Assert.AreEqual(pcpt.Expected, pcpt.Card.IsPlayable(), String.Format("{0} failed the IsPlayable test here", pcpt.Card.ToString()));
            }


        }

        [TestMethod]
        public void TestValidCardsDuringPlayerDead()
        {
            var cards = new List<PlayingCardPlayableTestor>()
            {
                new PlayingCardPlayableTestor(new WineBasicPlayingCard(PlayingCardColor.Red, PlayingCardSuite.Club, "", null), true),
                new PlayingCardPlayableTestor(new PeachBasicPlayingCard(PlayingCardColor.Red, PlayingCardSuite.Club, "", null), true),

                new PlayingCardPlayableTestor(new DodgeBasicPlayingCard(PlayingCardColor.Red, PlayingCardSuite.Club, "", null), false),

                new PlayingCardPlayableTestor(new AttackBasicPlayingCard(PlayingCardColor.Red, PlayingCardSuite.Club, "", null, Elemental.None), false),
                new PlayingCardPlayableTestor(new AttackBasicPlayingCard(PlayingCardColor.Red, PlayingCardSuite.Club, "", null, Elemental.Lightning), false),
                new PlayingCardPlayableTestor(new AttackBasicPlayingCard(PlayingCardColor.Red, PlayingCardSuite.Club, "", null, Elemental.Fire), false),

                new PlayingCardPlayableTestor(new DuelScrollPlayingCard(PlayingCardColor.Red, PlayingCardSuite.Club, "", null), false),
                new PlayingCardPlayableTestor(new LightningDelayedScrollPlayingCard(PlayingCardColor.Red, PlayingCardSuite.Club, "", null), false),
                new PlayingCardPlayableTestor(new WardScrollPlayingCard(PlayingCardColor.Red, PlayingCardSuite.Club, "", null), false),
                new PlayingCardPlayableTestor(new StarvationDelayedScrollPlayingCard(PlayingCardColor.Red, PlayingCardSuite.Club, "", null), false),
                new PlayingCardPlayableTestor(new ContentmentDelayedScrollPlayingCard(PlayingCardColor.Red, PlayingCardSuite.Club, "", null), false),

                new PlayingCardPlayableTestor(new WeaponEquipmentPlayingCard(2, PlayingCardColor.Red, PlayingCardSuite.Club, "", null, null, null ,null), false),
                new PlayingCardPlayableTestor(new ShieldEquipmentPlayingCard(PlayingCardColor.Red, PlayingCardSuite.Club, "", null, null, null, null, null), false),
                new PlayingCardPlayableTestor(new HorseEquipmentPlayingCard(1, PlayingCardColor.Red, PlayingCardSuite.Club, "", null), false),

            };

            var ctx = new GameContext(null);
            var p = new Player("dab", 3, Roles.Minister);

            p.CurrentHealth = 0;
            p.AttacksLeft = 1;

            ctx.CurrentTurnStage = TurnStages.PlayerDied;
            ctx.CurrentPlayStage.Source = new TargetPlayer(p);
            ctx.CurrentPlayStage.Cards = new SelectedCardsSender(new List<PlayingCard>() { cards[3].Card }, cards[3].Card);


            foreach (var pcpt in cards)
            {
                pcpt.Card.Context = ctx;
                pcpt.Card.Owner = p;
                Assert.AreEqual(pcpt.Expected, pcpt.Card.IsPlayable(), "Assert failed for " + pcpt.Card.ToString() + " " + ctx.CurrentTurnStage.ToString());

            }


        }

        [TestMethod]
        public void TestValidCardsDuringDuelScroll()
        {
            var cards = new List<PlayingCardPlayableTestor>()
            {
                new PlayingCardPlayableTestor(new WineBasicPlayingCard(PlayingCardColor.Red, PlayingCardSuite.Club, "", null), false),
                new PlayingCardPlayableTestor(new PeachBasicPlayingCard(PlayingCardColor.Red, PlayingCardSuite.Club, "", null), false),

                new PlayingCardPlayableTestor(new DodgeBasicPlayingCard(PlayingCardColor.Red, PlayingCardSuite.Club, "", null), false),

                new PlayingCardPlayableTestor(new AttackBasicPlayingCard(PlayingCardColor.Red, PlayingCardSuite.Club, "", null, Elemental.None), true),
                new PlayingCardPlayableTestor(new AttackBasicPlayingCard(PlayingCardColor.Red, PlayingCardSuite.Club, "", null, Elemental.Lightning), true),
                new PlayingCardPlayableTestor(new AttackBasicPlayingCard(PlayingCardColor.Red, PlayingCardSuite.Club, "", null, Elemental.Fire), true),

                new PlayingCardPlayableTestor(new DuelScrollPlayingCard(PlayingCardColor.Red, PlayingCardSuite.Club, "", null), false),
                new PlayingCardPlayableTestor(new LightningDelayedScrollPlayingCard(PlayingCardColor.Red, PlayingCardSuite.Club, "", null), false),
                new PlayingCardPlayableTestor(new WardScrollPlayingCard(PlayingCardColor.Red, PlayingCardSuite.Club, "", null), false),
                new PlayingCardPlayableTestor(new StarvationDelayedScrollPlayingCard(PlayingCardColor.Red, PlayingCardSuite.Club, "", null), false),
                new PlayingCardPlayableTestor(new ContentmentDelayedScrollPlayingCard(PlayingCardColor.Red, PlayingCardSuite.Club, "", null), false),

                new PlayingCardPlayableTestor(new WeaponEquipmentPlayingCard(2, PlayingCardColor.Red, PlayingCardSuite.Club, "", null, null, null ,null), false),
                new PlayingCardPlayableTestor(new ShieldEquipmentPlayingCard(PlayingCardColor.Red, PlayingCardSuite.Club, "", null, null, null, null, null), false),
                new PlayingCardPlayableTestor(new HorseEquipmentPlayingCard(1, PlayingCardColor.Red, PlayingCardSuite.Club, "", null), false),
            };

            var ctx = new GameContext(null);
            var p = new Player("dab", 3, Roles.Minister);

            ctx.CurrentTurnStage = TurnStages.PlayScrollPlaceResponse;
            var duel = new DuelScrollPlayingCard(PlayingCardColor.Black, PlayingCardSuite.Club, "Play duel bro");

            Assert.IsTrue(duel.IsPlayedAsDuel());

            ctx.CurrentPlayStage.Cards = new SelectedCardsSender(new List<PlayingCard>() { duel }, duel);

            foreach (var pcpt in cards)
            {
                pcpt.Card.Context = ctx;
                pcpt.Card.Owner = p;
                Assert.AreEqual(pcpt.Expected, pcpt.Card.IsPlayable(), String.Format("{0} (Type: {1}) failed the IsPlayable test here", pcpt.Card.ToString(), pcpt.Card.GetType().ToString()));
            }


        }
        [TestMethod]
        public void TestValidCardsDuringDuelScrollWardable()
        {
            var cards = new List<PlayingCardPlayableTestor>()
            {
                new PlayingCardPlayableTestor(new WineBasicPlayingCard(PlayingCardColor.Red, PlayingCardSuite.Club, "", null), false),
                new PlayingCardPlayableTestor(new PeachBasicPlayingCard(PlayingCardColor.Red, PlayingCardSuite.Club, "", null), false),

                new PlayingCardPlayableTestor(new DodgeBasicPlayingCard(PlayingCardColor.Red, PlayingCardSuite.Club, "", null), false),

                new PlayingCardPlayableTestor(new AttackBasicPlayingCard(PlayingCardColor.Red, PlayingCardSuite.Club, "", null, Elemental.None), false),
                new PlayingCardPlayableTestor(new AttackBasicPlayingCard(PlayingCardColor.Red, PlayingCardSuite.Club, "", null, Elemental.Lightning), false),
                new PlayingCardPlayableTestor(new AttackBasicPlayingCard(PlayingCardColor.Red, PlayingCardSuite.Club, "", null, Elemental.Fire), false),

                new PlayingCardPlayableTestor(new DuelScrollPlayingCard(PlayingCardColor.Red, PlayingCardSuite.Club, "", null), false),
                new PlayingCardPlayableTestor(new LightningDelayedScrollPlayingCard(PlayingCardColor.Red, PlayingCardSuite.Club, "", null), false),
                new PlayingCardPlayableTestor(new WardScrollPlayingCard(PlayingCardColor.Red, PlayingCardSuite.Club, "", null), true),
                new PlayingCardPlayableTestor(new StarvationDelayedScrollPlayingCard(PlayingCardColor.Red, PlayingCardSuite.Club, "", null), false),
                new PlayingCardPlayableTestor(new ContentmentDelayedScrollPlayingCard(PlayingCardColor.Red, PlayingCardSuite.Club, "", null), false),

                new PlayingCardPlayableTestor(new WeaponEquipmentPlayingCard(2, PlayingCardColor.Red, PlayingCardSuite.Club, "", null, null, null ,null), false),
                new PlayingCardPlayableTestor(new ShieldEquipmentPlayingCard(PlayingCardColor.Red, PlayingCardSuite.Club, "", null, null, null, null, null), false),
                new PlayingCardPlayableTestor(new HorseEquipmentPlayingCard(1, PlayingCardColor.Red, PlayingCardSuite.Club, "", null), false),
            };

            var ctx = new GameContext(null);
            var p = new Player("dab", 3, Roles.Minister);

            ctx.CurrentTurnStage = TurnStages.PlayScrollTargets;
            var duel = new DuelScrollPlayingCard(PlayingCardColor.Black, PlayingCardSuite.Club, "Play duel bro");

            Assert.IsTrue(duel.IsPlayedAsDuel());

            ctx.CurrentPlayStage.Cards = new SelectedCardsSender(new List<PlayingCard>() { duel }, duel);
            ctx.CurrentPlayStage.ExpectingIputFrom.Prompt = new Prompts.UserPrompt(Prompts.UserPromptType.CardsPlayerHand);

            foreach (var pcpt in cards)
            {
                pcpt.Card.Context = ctx;
                pcpt.Card.Owner = p;
                Assert.AreEqual(pcpt.Expected, pcpt.Card.IsPlayable(), String.Format("{0} (Type: {1}) failed the IsPlayable test here", pcpt.Card.ToString(), pcpt.Card.GetType().ToString()));
            }
        }
    }
}

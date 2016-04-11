using System;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using dab.SGS.Core;
using dab.SGS.Core.PlayingCards;
using dab.SGS.Core.Actions;
using System.Collections.Generic;
using dab.SGS.Core.PlayingCards.Basics;
using dab.SGS.Core.PlayingCards.Scrolls;
using dab.SGS.Core.PlayingCards.Equipments;

namespace dab.SGS.Core.Unit
{
    [TestClass]
    public class PlayTests
    {
        public List<PlayingCard> GetDeck()
        {
            var deck = Deck.LoadCards(FileIOHelper.ReadFromDefaultFile("ms-appx:///Assets/deck.json"),
                p => p.Hand[0], p => true);

            return deck;
        }

        public List<PlayingCard> GetAttackDodgeAlternateDeck(int count)
        {
            var d = new List<PlayingCard>();

            for (var i = 0; i < count; i += 2)
            {
                d.Add(new PlayingCards.Basics.AttackBasicPlayingCard(PlayingCardColor.Black, PlayingCardSuite.Club, "", new List<Actions.Action>() { new AttackAction("Attack", 1) }, PlayingCards.Basics.Elemental.None));
                d.Add(new PlayingCards.Basics.DodgeBasicPlayingCard(PlayingCardColor.Black, PlayingCardSuite.Club, "", new List<Actions.Action>() { new DodgeAction() }));
            }

            return d;
        }
        
        [TestMethod]
        public void TestTurnDodge()
        {
            Actions.Action action = null;

            // Always select the first card for discard (stupid I know right?)
            var ctx = new GameContext(new Deck(this.GetDeck())); 

            ctx.AddPlayer("P1", null, Roles.King);
            ctx.AddPlayer("P2", null, Roles.Minister);
            ctx.AddPlayer("P3", null, Roles.Rebel);
            ctx.AddPlayer("P4", null, Roles.Rebel);
            ctx.AddPlayer("P5", null, Roles.TurnCoat);
            
            ctx.SetupGame();

            Assert.AreEqual(Roles.King, ctx.CurrentPlayerTurn.Role);

            Assert.AreEqual(4, ctx.CurrentPlayerTurn.Hand.Count);
            Assert.AreEqual(ctx.CurrentPlayerTurn, ctx.CurrentPlayerTurn.Hand[0].Owner);

            // Skip to the next stage in our turn
            while (ctx.CurrentTurnStage != TurnStages.Play)
            {
                action = ctx.RoateTurnStage();
                action.Perform(ctx, ctx.CurrentPlayerTurn, ctx);
            }


            Assert.AreEqual(TurnStages.Play, ctx.CurrentTurnStage);
            Assert.AreEqual(6, ctx.CurrentPlayerTurn.Hand.Count);

            // Play an attack.
            var sender = new SelectedCardsSender(new List<PlayingCard>() { ctx.CurrentPlayerTurn.Hand[0] }, ctx.CurrentPlayerTurn.Hand[0]);
            
            // Play first playable card in the select cards (only 1 of the any should be playable).
            foreach (var card in sender) if (card.IsPlayable()) card.Play(sender);

            Assert.AreEqual(TurnStages.AttackChooseTargets, ctx.CurrentTurnStage);

            action = ctx.RoateTurnStage();

            Assert.AreEqual(TurnStages.AttackChooseTargets, ctx.CurrentTurnStage);
            Assert.AreEqual(TurnStages.Play, ctx.PreviousStages.Peek().Stage);

            // Choose a target (REUSE SENDER)
            ctx.CurrentPlayStage.Targets.Add(new TargetPlayer(ctx.CurrentPlayerTurn.Right));
            ctx.CurrentPlayerTurn.Hand[0].Play(sender); // Update target in attack

            action = ctx.RoateTurnStage();

            Assert.AreEqual(TurnStages.AttackSkillResponse, ctx.CurrentTurnStage);

            // Rotate through our skills for defense, but they shouldn't exist
            action = ctx.RoateTurnStage();

            Assert.AreEqual(TurnStages.AttackShieldResponse, ctx.CurrentTurnStage);
            // Attempt to execute our shield response, if it exists, but it shouldn't
            action = ctx.RoateTurnStage();

            foreach(var target in ctx.CurrentPlayStage.Targets)
            {
                // Our target must play a dodge or take damage. Poor soul
                var dodge = target.Target.Hand.Find(p => p.IsPlayable());

                var senderResp = new SelectedCardsSender(new List<PlayingCard>() { dodge }, dodge);
                dodge.Play(senderResp);
            }

            // Move on from CardResponse to Pre-Damage
            action = ctx.RoateTurnStage();

            // Move on from Pre-Damage to Damage
            action = ctx.RoateTurnStage();
            // calls the .Play of the card again, which should pop the previous state (also discards the cards)
            action.Perform(ctx, ctx.CurrentPlayerTurn, ctx); 

            //ctx.CurrentPlayerTurn.Hand[0].Play(sender);

            Assert.AreEqual(TurnStages.Play, ctx.CurrentTurnStage);
            Assert.AreEqual(4, ctx.CurrentPlayerTurn.Right.CurrentHealth);

            // SKip to the end stage of our turn
            while (ctx.CurrentTurnStage != TurnStages.Discard)
            {
                action = ctx.RoateTurnStage();
                if (action != null)
                    action.Perform(ctx, ctx.CurrentPlayerTurn, ctx);
            }

            while (!action.Perform(new SelectedCardsSender(new List<PlayingCard>() { ctx.CurrentPlayerTurn.Hand[0] }, null), ctx.CurrentPlayStage.Source.Target, ctx)) ;

            Assert.AreEqual(5, ctx.CurrentPlayerTurn.Hand.Count);
            ctx.RoateTurnStage();

            Assert.AreEqual(TurnStages.PostDiscard, ctx.CurrentTurnStage);

            ctx.RoateTurnStage().Perform(ctx, ctx.CurrentPlayerTurn, ctx);

            ctx.RoateTurnStage().Perform(ctx, ctx.CurrentPlayerTurn, ctx);


            Assert.AreEqual("P2", ctx.CurrentPlayerTurn.Display);
        }


        [TestMethod]
        public void TestTurnNoDodge()
        {
            Actions.Action action = null;

            // Always select the first card for discard (stupid I know right?)
            var ctx = new GameContext(new Deck(this.GetDeck()));

            ctx.AddPlayer("P1", null, Roles.King);
            ctx.AddPlayer("P2", null, Roles.Minister);
            ctx.AddPlayer("P3", null, Roles.Rebel);
            ctx.AddPlayer("P4", null, Roles.Rebel);
            ctx.AddPlayer("P5", null, Roles.TurnCoat);

            ctx.SetupGame();

            Assert.AreEqual(Roles.King, ctx.CurrentPlayerTurn.Role);
            Assert.AreEqual(4, ctx.CurrentPlayerTurn.Hand.Count);
            Assert.AreEqual(ctx.CurrentPlayerTurn, ctx.CurrentPlayerTurn.Hand[0].Owner);

            // Skip to the next stage in our turn
            while (ctx.CurrentTurnStage != TurnStages.Play)
            {
                action = ctx.RoateTurnStage();
                if (action != null)
                    action.Perform(ctx, ctx.CurrentPlayerTurn, ctx);
            }


            Assert.AreEqual(TurnStages.Play, ctx.CurrentTurnStage);
            Assert.AreEqual(6, ctx.CurrentPlayerTurn.Hand.Count);

            // Play an attack.
            var sender = new SelectedCardsSender(new List<PlayingCard>() { ctx.CurrentPlayerTurn.Hand[0] }, ctx.CurrentPlayerTurn.Hand[0]);

            // Play first playable card in the select cards (only 1 of the any should be playable).
            foreach (var card in sender) if (card.IsPlayable()) card.Play(sender);

            Assert.AreEqual(TurnStages.AttackChooseTargets, ctx.CurrentTurnStage);

            action = ctx.RoateTurnStage();

            Assert.AreEqual(TurnStages.AttackChooseTargets, ctx.CurrentTurnStage);
            Assert.AreEqual(TurnStages.Play, ctx.PreviousStages.Peek().Stage);

            // Choose a target (REUSE SENDER)
            ctx.CurrentPlayStage.Targets.Add(new TargetPlayer(ctx.CurrentPlayerTurn.Right));
            ctx.CurrentPlayerTurn.Hand[0].Play(sender); // Update target in attack

            action = ctx.RoateTurnStage();

            Assert.AreEqual(TurnStages.AttackSkillResponse, ctx.CurrentTurnStage);

            // Rotate through our skills for defense, but they shouldn't exist
            action = ctx.RoateTurnStage();

            Assert.AreEqual(TurnStages.AttackShieldResponse, ctx.CurrentTurnStage);
            // Attempt to execute our shield response, if it exists, but it shouldn't
            action = ctx.RoateTurnStage();
            
            // Move on from CardResponse to Pre-Damage
            action = ctx.RoateTurnStage();

            // Move on from Pre-Damage to Damage
            action = ctx.RoateTurnStage();
            // calls the .Play of the card again, which should pop the previous state (also discards the cards)
            action.Perform(ctx, ctx.CurrentPlayerTurn, ctx);

            Assert.AreEqual(TurnStages.Play, ctx.CurrentTurnStage);
            Assert.AreEqual(3, ctx.CurrentPlayerTurn.Right.CurrentHealth);

            // SKip to the end stage of our turn
            while (ctx.CurrentTurnStage != TurnStages.Discard)
            {
                action = ctx.RoateTurnStage();
                if (action != null)
                    action.Perform(ctx, ctx.CurrentPlayerTurn, ctx);
            }

            while (!action.Perform(new SelectedCardsSender(new List<PlayingCard>() { ctx.CurrentPlayerTurn.Hand[0] }, null), ctx.CurrentPlayStage.Source.Target, ctx)) ;

            Assert.AreEqual(5, ctx.CurrentPlayerTurn.Hand.Count);
            ctx.RoateTurnStage();

            Assert.AreEqual(TurnStages.PostDiscard, ctx.CurrentTurnStage);

            ctx.RoateTurnStage().Perform(ctx, ctx.CurrentPlayerTurn, ctx);

            ctx.RoateTurnStage().Perform(ctx, ctx.CurrentPlayerTurn, ctx);


            Assert.AreEqual("P2", ctx.CurrentPlayerTurn.Display);
        }


        [TestMethod]
        public void TestRoleAssignment()
        {
            var ctx = new GameContext(new Deck(this.GetAttackDodgeAlternateDeck(22)));

            ctx.AddPlayer("P1", null, Roles.King);
            ctx.AddPlayer("P2", null);
            ctx.AddPlayer("P3", null);
            ctx.AddPlayer("P4", null);
            ctx.AddPlayer("P5", null);

            ctx.SetupGame();

            Assert.AreEqual(Roles.King, ctx.Players[0].Role);

            var roles = new List<Roles>(Player.GetRoles(ctx.Players.Length));

            foreach (var player in ctx.Players)
            {
                roles.Remove(player.Role);
            }

            Assert.AreEqual(0, roles.Count);

        }

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

            foreach(var pcpt in cards)
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
                new PlayingCardPlayableTestor(new WardScrollPlayingCard(PlayingCardColor.Red, PlayingCardSuite.Club, "", null), true),
                new PlayingCardPlayableTestor(new StarvationDelayedScrollPlayingCard(PlayingCardColor.Red, PlayingCardSuite.Club, "", null), false),
                new PlayingCardPlayableTestor(new ContentmentDelayedScrollPlayingCard(PlayingCardColor.Red, PlayingCardSuite.Club, "", null), false),

                new PlayingCardPlayableTestor(new WeaponEquipmentPlayingCard(2, PlayingCardColor.Red, PlayingCardSuite.Club, "", null, null, null ,null), false),
                new PlayingCardPlayableTestor(new ShieldEquipmentPlayingCard(PlayingCardColor.Red, PlayingCardSuite.Club, "", null, null, null, null, null), false),
                new PlayingCardPlayableTestor(new HorseEquipmentPlayingCard(1, PlayingCardColor.Red, PlayingCardSuite.Club, "", null), false),
            };

            var ctx = new GameContext(null);
            var p = new Player("dab", 3, Roles.Minister);

            ctx.CurrentTurnStage = TurnStages.PlayScrollPlace;
            var duel = new DuelScrollPlayingCard(PlayingCardColor.Black, PlayingCardSuite.Club, "Duel", "Play duel bro");

            Assert.IsTrue(duel.IsPlayedAsDuel());

            ctx.CurrentPlayStage.Cards = new SelectedCardsSender(new List<PlayingCard>() { duel }, duel);

            foreach (var pcpt in cards)
            {
                pcpt.Card.Context = ctx;
                pcpt.Card.Owner = p;
                Assert.AreEqual(pcpt.Expected, pcpt.Card.IsPlayable(), String.Format("{0} failed the IsPlayable test here", pcpt.Card.ToString()));
            }


        }

        [TestMethod]
        public void TestTurnDuelAttack()
        {
            Actions.Action action = null;

            // Always select the first card for discard (stupid I know right?)
            var ctx = new GameContext(new Deck(this.GetDeck()));

            ctx.AddPlayer("P1", null, Roles.King);
            ctx.AddPlayer("P2", null, Roles.Minister);
            ctx.AddPlayer("P3", null, Roles.Rebel);
            ctx.AddPlayer("P4", null, Roles.Rebel);
            ctx.AddPlayer("P5", null, Roles.TurnCoat);

            ctx.SetupGame();

            Assert.AreEqual(Roles.King, ctx.CurrentPlayerTurn.Role);
            Assert.AreEqual(4, ctx.CurrentPlayerTurn.Hand.Count);
            Assert.AreEqual(ctx.CurrentPlayerTurn, ctx.CurrentPlayerTurn.Hand[0].Owner);

            // Skip to the next stage in our turn
            while (ctx.CurrentTurnStage != TurnStages.Play)
            {
                action = ctx.RoateTurnStage();
                if (action != null)
                    action.Perform(ctx, ctx.CurrentPlayerTurn, ctx);
            }


            Assert.AreEqual(TurnStages.Play, ctx.CurrentTurnStage);
            Assert.AreEqual(6, ctx.CurrentPlayerTurn.Hand.Count);

            // Play an attack.
            var sender = new SelectedCardsSender(new List<PlayingCard>() { ctx.CurrentPlayerTurn.Hand.Find(p => p.IsPlayedAsDuel()) }, ctx.CurrentPlayerTurn.Hand.Find(p => p.IsPlayedAsDuel()));

            // Play first playable card in the select cards (only 1 of the any should be playable).
            foreach (var card in sender) if (card.IsPlayable()) card.Play(sender);

            Assert.AreEqual(TurnStages.PlayScrollTargets, ctx.CurrentTurnStage);

            action = ctx.RoateTurnStage();

            Assert.AreEqual(TurnStages.PlayScrollTargets, ctx.CurrentTurnStage);
            Assert.AreEqual(TurnStages.Play, ctx.PreviousStages.Peek().Stage);

            // Choose a target (REUSE SENDER)
            ctx.CurrentPlayStage.Targets.Add(new TargetPlayer(ctx.CurrentPlayerTurn.Right));
            sender.Activator.Play(sender); // Update target in attack

            action = ctx.RoateTurnStage();

            Assert.AreEqual(TurnStages.PlayScrollPlace, ctx.CurrentTurnStage);

            foreach (var target in ctx.CurrentPlayStage.Targets)
            {
                // Our target must play a dodge or take damage. Poor soul
                var attack = target.Target.Hand.Find(p => p.IsPlayable() && p.GetType() == typeof(AttackBasicPlayingCard));

                var senderResp = new SelectedCardsSender(new List<PlayingCard>() { attack }, attack);
                attack.Play(senderResp);
            }

            // Move on from CardResponse to Pre-Damage
            action = ctx.RoateTurnStage();

            Assert.AreEqual(TurnStages.PlayScrollPlace, ctx.CurrentTurnStage);
            Assert.AreEqual(ctx.Players[0], ctx.CurrentPlayStage.ExpectingIputFrom);

            // Move on from Pre-Damage to Damage
            action = ctx.RoateTurnStage();
            // calls the .Play of the card again, which should pop the previous state (also discards the cards)
            action.Perform(sender, ctx.CurrentPlayerTurn, ctx);

            Assert.AreEqual(TurnStages.Play, ctx.CurrentTurnStage);
            Assert.AreEqual(3, ctx.CurrentPlayerTurn.Right.CurrentHealth);

            // SKip to the end stage of our turn
            while (ctx.CurrentTurnStage != TurnStages.Discard)
            {
                action = ctx.RoateTurnStage();
                if (action != null)
                    action.Perform(ctx, ctx.CurrentPlayerTurn, ctx);
            }

            while (!action.Perform(new SelectedCardsSender(new List<PlayingCard>() { ctx.CurrentPlayerTurn.Hand[0] }, null), ctx.CurrentPlayStage.Source.Target, ctx)) ;

            Assert.AreEqual(5, ctx.CurrentPlayerTurn.Hand.Count);
            ctx.RoateTurnStage();

            Assert.AreEqual(TurnStages.PostDiscard, ctx.CurrentTurnStage);

            ctx.RoateTurnStage().Perform(ctx, ctx.CurrentPlayerTurn, ctx);

            ctx.RoateTurnStage().Perform(ctx, ctx.CurrentPlayerTurn, ctx);


            Assert.AreEqual("P2", ctx.CurrentPlayerTurn.Display);
        }

    }
}

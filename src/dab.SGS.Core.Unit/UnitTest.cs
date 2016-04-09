using System;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using dab.SGS.Core;
using dab.SGS.Core.PlayingCards;
using dab.SGS.Core.Actions;
using System.Collections.Generic;
using dab.SGS.Core.PlayingCards.Basics;

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
            var ctx = new GameContext(new Deck(this.GetDeck()), p => p.Hand[0]); 

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

            ctx.CurrentPlayerTurn.Hand[0].Play(sender);

            Assert.AreEqual(TurnStages.AttackChooseTargets, ctx.CurrentTurnStage);

            // Choose a target (REUSE SENDER)
            ctx.CurrentPlayStage.Targets.Add(new TargetPlayer(ctx.CurrentPlayerTurn.Right));
            ctx.CurrentPlayerTurn.Hand[0].Play(sender); // Update target in attack

            action = ctx.RoateTurnStage();

            Assert.AreEqual(TurnStages.SkillResponse, ctx.CurrentTurnStage);

            // Rotate through our skills for defense, but they shouldn't exist
            action = ctx.RoateTurnStage();

            Assert.AreEqual(TurnStages.ShieldResponse, ctx.CurrentTurnStage);
            // Attempt to execute our shield response, if it exists, but it shouldn't
            action = ctx.RoateTurnStage();

            foreach(var target in ctx.CurrentPlayStage.Targets)
            {
                // Our target must play a dodge or take damage. Poor soul
                var dodge = target.Target.Hand.Find(p => p.IsPlayable());

                dodge.Play(dodge);
            }

            // Move on from CardResponse to Pre-Damage
            action = ctx.RoateTurnStage();

            // Move on from Pre-Damage to Damage
            action = ctx.RoateTurnStage();

            ctx.CurrentPlayerTurn.Hand[0].Play(sender);

            Assert.AreEqual(TurnStages.Play, ctx.CurrentTurnStage);
            Assert.AreEqual(4, ctx.CurrentPlayerTurn.Right.CurrentHealth);

            // SKip to the end stage of our turn
            while (ctx.CurrentTurnStage != TurnStages.End)
            {
                action = ctx.RoateTurnStage();
                if (action != null)
                    action.Perform(ctx, ctx.CurrentPlayerTurn, ctx);
            }

            Assert.AreEqual(5, ctx.CurrentPlayerTurn.Hand.Count);

            ctx.RoateTurnStage();

            Assert.AreEqual("P2", ctx.CurrentPlayerTurn.Display);
        }


        [TestMethod]
        public void TestTurnNoDodge()
        {
            Actions.Action action = null;

            // Always select the first card for discard (stupid I know right?)
            var ctx = new GameContext(new Deck(this.GetDeck()), p => p.Hand[0]);

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

            ctx.CurrentPlayerTurn.Hand[0].Play(sender);

            Assert.AreEqual(TurnStages.AttackChooseTargets, ctx.CurrentTurnStage);

            // Choose a target (REUSE SENDER)
            ctx.CurrentPlayStage.Targets.Add(new TargetPlayer(ctx.CurrentPlayerTurn.Right));
            ctx.CurrentPlayerTurn.Hand[0].Play(sender); // Update target in attack

            action = ctx.RoateTurnStage();

            Assert.AreEqual(TurnStages.SkillResponse, ctx.CurrentTurnStage);

            // Rotate through our skills for defense, but they shouldn't exist
            action = ctx.RoateTurnStage();

            Assert.AreEqual(TurnStages.ShieldResponse, ctx.CurrentTurnStage);
            // Attempt to execute our shield response, if it exists, but it shouldn't
            action = ctx.RoateTurnStage();
            
            // Move on from CardResponse to Pre-Damage
            action = ctx.RoateTurnStage();

            // Move on from Pre-Damage to Damage
            action = ctx.RoateTurnStage();

            ctx.CurrentPlayerTurn.Hand[0].Play(sender);

            Assert.AreEqual(TurnStages.Play, ctx.CurrentTurnStage);
            Assert.AreEqual(3, ctx.CurrentPlayerTurn.Right.CurrentHealth);

            // SKip to the end stage of our turn
            while (ctx.CurrentTurnStage != TurnStages.End)
            {
                action = ctx.RoateTurnStage();
                if (action != null)
                    action.Perform(ctx, ctx.CurrentPlayerTurn, ctx);
            }

            Assert.AreEqual(5, ctx.CurrentPlayerTurn.Hand.Count);

            ctx.RoateTurnStage();

            Assert.AreEqual("P2", ctx.CurrentPlayerTurn.Display);
        }


        [TestMethod]
        public void TestRoleAssignment()
        {
            var ctx = new GameContext(new Deck(this.GetAttackDodgeAlternateDeck(22)), p => p.Hand[0]);

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

        [TestMethod]
        public void TestValidCards()
        {
            var wine = new WineBasicPlayingCard(PlayingCardColor.Red, PlayingCardSuite.Club, "", null);
            var peach = new PeachBasicPlayingCard(PlayingCardColor.Red, PlayingCardSuite.Club, "", null);

            var dodge = new DodgeBasicPlayingCard(PlayingCardColor.Red, PlayingCardSuite.Club, "", null);

            var attack = new AttackBasicPlayingCard(PlayingCardColor.Red, PlayingCardSuite.Club, "", null, Elemental.None);
            var lightningAttack = new AttackBasicPlayingCard(PlayingCardColor.Red, PlayingCardSuite.Club, "", null, Elemental.Lightning);
            var fireAttack = new AttackBasicPlayingCard(PlayingCardColor.Red, PlayingCardSuite.Club, "", null, Elemental.Fire);

            var ctx = new GameContext(null, null);



        }
    }
}

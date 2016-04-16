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
    public class DuelUnitTests
    {

        [TestMethod]
        public void TestTurnDuelAttack()
        {
            Actions.Action action = null;

            // Always select the first card for discard (stupid I know right?)
            var ctx = new GameContext(new Deck(PlayTests.GetDeck()));

            ctx.AddPlayer("P1", null, Roles.King);
            ctx.AddPlayer("P2", null, Roles.Minister);
            ctx.AddPlayer("P3", null, Roles.Rebel);
            ctx.AddPlayer("P4", null, Roles.Rebel);
            ctx.AddPlayer("P5", null, Roles.TurnCoat);

            ctx.SetupGame();

            SelectedCardsSender sender = null;

            Assert.AreEqual(Roles.King, ctx.CurrentPlayerTurn.Role);
            Assert.AreEqual(4, ctx.CurrentPlayerTurn.Hand.Count);
            Assert.AreEqual(ctx.CurrentPlayerTurn, ctx.CurrentPlayerTurn.Hand[0].Owner);

            // Skip to the next stage in our turn
            while (ctx.CurrentTurnStage != TurnStages.Play)
            {
                action = ctx.RoateTurnStage();
                if (action != null)
                    action.Perform(sender, ctx.CurrentPlayerTurn, ctx);
            }


            Assert.AreEqual(TurnStages.Play, ctx.CurrentTurnStage);
            Assert.AreEqual(6, ctx.CurrentPlayerTurn.Hand.Count);

            // Play a duel.
            sender = new SelectedCardsSender(new List<PlayingCard>() { ctx.CurrentPlayerTurn.Hand.Find(p => p.IsPlayedAsDuel()) }, ctx.CurrentPlayerTurn.Hand.Find(p => p.IsPlayedAsDuel()));

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
            action.Perform(sender, ctx.CurrentPlayerTurn, ctx);

            Assert.AreEqual(TurnStages.PlayScrollPlaceResponse, ctx.CurrentTurnStage);

            //action = ctx.RoateTurnStage();
            //action.Perform(sender, ctx.CurrentPlayerTurn, ctx);

            while (ctx.CurrentTurnStage == TurnStages.PlayScrollPlaceResponse && ctx.CurrentPlayStage.ExpectingIputFrom.Player != null)
            {

                // Our target must play a dodge or take damage. Poor soul
                var attack = ctx.CurrentPlayStage.ExpectingIputFrom.Player.Target.Hand.Find(p => p.IsPlayable() && p.GetType() == typeof(AttackBasicPlayingCard));

                if (attack != null)
                {
                    var senderResp = new SelectedCardsSender(new List<PlayingCard>() { attack }, attack);
                    attack.Play(senderResp);
                }

                action = ctx.RoateTurnStage();
                action.Perform(sender, ctx.CurrentPlayerTurn, ctx);
            }

            Assert.AreEqual(TurnStages.PlayScrollPlaceResponse, ctx.CurrentTurnStage);
            action = ctx.RoateTurnStage();
            action.Perform(sender, ctx.CurrentPlayerTurn, ctx);

            Assert.AreEqual(TurnStages.Play, ctx.CurrentTurnStage);

            // SKip to the end stage of our turn
            while (ctx.CurrentTurnStage != TurnStages.Discard)
            {
                action = ctx.RoateTurnStage();
                action.Perform(sender, ctx.CurrentPlayerTurn, ctx);
            }

            while (!action.Perform(new SelectedCardsSender(new List<PlayingCard>() { ctx.CurrentPlayerTurn.Hand[0] }, null), ctx.CurrentPlayStage.Source.Target, ctx)) ;

            Assert.AreEqual(4, ctx.CurrentPlayerTurn.Hand.Count);
            ctx.RoateTurnStage();

            Assert.AreEqual(TurnStages.PostDiscard, ctx.CurrentTurnStage);

            ctx.RoateTurnStage().Perform(sender, ctx.CurrentPlayerTurn, ctx);

            ctx.RoateTurnStage().Perform(sender, ctx.CurrentPlayerTurn, ctx);


            Assert.AreEqual("P2", ctx.CurrentPlayerTurn.Display);
            Assert.AreEqual(3, ctx.CurrentPlayerTurn.CurrentHealth);
        }
        [TestMethod]
        public void TestTurnDuelWard()
        {
            Actions.Action action = null;

            // Always select the first card for discard (stupid I know right?)
            var ctx = new GameContext(new Deck(PlayTests.GetDeck()));

            ctx.AddPlayer("P1", null, Roles.King);
            ctx.AddPlayer("P2", null, Roles.Minister);
            ctx.AddPlayer("P3", null, Roles.Rebel);
            ctx.AddPlayer("P4", null, Roles.Rebel);
            ctx.AddPlayer("P5", null, Roles.TurnCoat);

            ctx.SetupGame();

            SelectedCardsSender sender = null;

            Assert.AreEqual(Roles.King, ctx.CurrentPlayerTurn.Role);
            Assert.AreEqual(4, ctx.CurrentPlayerTurn.Hand.Count);
            Assert.AreEqual(ctx.CurrentPlayerTurn, ctx.CurrentPlayerTurn.Hand[0].Owner);

            // Skip to the next stage in our turn
            while (ctx.CurrentTurnStage != TurnStages.Play)
            {
                action = ctx.RoateTurnStage();
                if (action != null)
                    action.Perform(sender, ctx.CurrentPlayerTurn, ctx);
            }


            Assert.AreEqual(TurnStages.Play, ctx.CurrentTurnStage);
            Assert.AreEqual(6, ctx.CurrentPlayerTurn.Hand.Count);

            // Play a duel.
            sender = new SelectedCardsSender(new List<PlayingCard>() { ctx.CurrentPlayerTurn.Hand.Find(p => p.IsPlayedAsDuel()) }, ctx.CurrentPlayerTurn.Hand.Find(p => p.IsPlayedAsDuel()));

            // Play first playable card in the select cards (only 1 of the any should be playable).
            foreach (var card in sender) if (card.IsPlayable()) card.Play(sender);

            Assert.AreEqual(TurnStages.PlayScrollTargets, ctx.CurrentTurnStage);

            action = ctx.RoateTurnStage();

            Assert.AreEqual(TurnStages.PlayScrollTargets, ctx.CurrentTurnStage);
            Assert.AreEqual(TurnStages.Play, ctx.PreviousStages.Peek().Stage);

            // Choose a target (REUSE SENDER)
            ctx.CurrentPlayStage.Targets.Add(new TargetPlayer(ctx.CurrentPlayerTurn.Right));
            sender.Activator.Play(sender); // Update target in attack

            // Sneak a ward into our target's hand
            ctx.CurrentPlayerTurn.Right.Hand.Add(new WardScrollPlayingCard(PlayingCardColor.Black, PlayingCardSuite.Club, "") { Context = ctx, Owner = ctx.CurrentPlayerTurn.Right });

            // Our target must play a dodge or take damage. Poor soul
            var ward = ctx.CurrentPlayStage.Targets[0].Target.Hand.Find(p => p.IsPlayable() && p.GetType() == typeof(WardScrollPlayingCard));

            if (ward != null)
            {
                var senderResp = new SelectedCardsSender(new List<PlayingCard>() { ward }, ward);
                ward.Play(senderResp);
            }

            action = ctx.RoateTurnStage();
            action.Perform(sender, ctx.CurrentPlayerTurn, ctx);

            Assert.AreEqual(TurnStages.PlayScrollPlaceResponse, ctx.CurrentTurnStage);

            while (ctx.CurrentTurnStage == TurnStages.PlayScrollPlaceResponse && ctx.CurrentPlayStage.ExpectingIputFrom.Player != null)
            {
                action = ctx.RoateTurnStage();
                action.Perform(sender, ctx.CurrentPlayerTurn, ctx);
            }

            Assert.AreEqual(TurnStages.PlayScrollPlaceResponse, ctx.CurrentTurnStage);
            action = ctx.RoateTurnStage();
            action.Perform(sender, ctx.CurrentPlayerTurn, ctx);

            Assert.AreEqual(TurnStages.Play, ctx.CurrentTurnStage);

            // SKip to the end stage of our turn
            while (ctx.CurrentTurnStage != TurnStages.Discard)
            {
                action = ctx.RoateTurnStage();
                action.Perform(sender, ctx.CurrentPlayerTurn, ctx);
            }

            while (!action.Perform(new SelectedCardsSender(new List<PlayingCard>() { ctx.CurrentPlayerTurn.Hand[0] }, null), ctx.CurrentPlayStage.Source.Target, ctx)) ;

            Assert.AreEqual(5, ctx.CurrentPlayerTurn.Hand.Count);
            ctx.RoateTurnStage();

            Assert.AreEqual(TurnStages.PostDiscard, ctx.CurrentTurnStage);

            ctx.RoateTurnStage().Perform(sender, ctx.CurrentPlayerTurn, ctx);

            ctx.RoateTurnStage().Perform(sender, ctx.CurrentPlayerTurn, ctx);


            Assert.AreEqual("P2", ctx.CurrentPlayerTurn.Display);
            Assert.AreEqual(4, ctx.CurrentPlayerTurn.CurrentHealth);
        }
        [TestMethod]
        public void TestTurnDuelSourceNoAttack()
        {
            Actions.Action action = null;

            // Always select the first card for discard (stupid I know right?)
            var ctx = new GameContext(new Deck(PlayTests.GetDeck()));

            ctx.AddPlayer("P1", null, Roles.King);
            ctx.AddPlayer("P2", null, Roles.Minister);
            ctx.AddPlayer("P3", null, Roles.Rebel);
            ctx.AddPlayer("P4", null, Roles.Rebel);
            ctx.AddPlayer("P5", null, Roles.TurnCoat);

            ctx.SetupGame();

            SelectedCardsSender sender = null;

            Assert.AreEqual(Roles.King, ctx.CurrentPlayerTurn.Role);
            Assert.AreEqual(4, ctx.CurrentPlayerTurn.Hand.Count);
            Assert.AreEqual(ctx.CurrentPlayerTurn, ctx.CurrentPlayerTurn.Hand[0].Owner);

            // Skip to the next stage in our turn
            while (ctx.CurrentTurnStage != TurnStages.Play)
            {
                action = ctx.RoateTurnStage();
                if (action != null)
                    action.Perform(sender, ctx.CurrentPlayerTurn, ctx);
            }


            Assert.AreEqual(TurnStages.Play, ctx.CurrentTurnStage);
            Assert.AreEqual(6, ctx.CurrentPlayerTurn.Hand.Count);

            // Play a duel.
            sender = new SelectedCardsSender(new List<PlayingCard>() { ctx.CurrentPlayerTurn.Hand.Find(p => p.IsPlayedAsDuel()) }, ctx.CurrentPlayerTurn.Hand.Find(p => p.IsPlayedAsDuel()));

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
            action.Perform(sender, ctx.CurrentPlayerTurn, ctx);

            Assert.AreEqual(TurnStages.PlayScrollPlaceResponse, ctx.CurrentTurnStage);

            //action = ctx.RoateTurnStage();
            //action.Perform(sender, ctx.CurrentPlayerTurn, ctx);

            if (ctx.CurrentTurnStage == TurnStages.PlayScrollPlaceResponse && ctx.CurrentPlayStage.ExpectingIputFrom.Player != null)
            {

                // Our target must play a dodge or take damage. Poor soul
                var attack = ctx.CurrentPlayStage.ExpectingIputFrom.Player.Target.Hand.Find(p => p.IsPlayable() && p.GetType() == typeof(AttackBasicPlayingCard));

                if (attack != null)
                {
                    var senderResp = new SelectedCardsSender(new List<PlayingCard>() { attack }, attack);
                    attack.Play(senderResp);
                }

                action = ctx.RoateTurnStage();
                action.Perform(sender, ctx.CurrentPlayerTurn, ctx);
            }

            Assert.AreEqual(TurnStages.PlayScrollPlaceResponse, ctx.CurrentTurnStage);
            action = ctx.RoateTurnStage();
            action.Perform(sender, ctx.CurrentPlayerTurn, ctx);

            action = ctx.RoateTurnStage();
            action.Perform(sender, ctx.CurrentPlayerTurn, ctx);

            Assert.AreEqual(TurnStages.Play, ctx.CurrentTurnStage);

            // SKip to the end stage of our turn
            while (ctx.CurrentTurnStage != TurnStages.Discard)
            {
                action = ctx.RoateTurnStage();
                action.Perform(sender, ctx.CurrentPlayerTurn, ctx);
            }

            while (!action.Perform(new SelectedCardsSender(new List<PlayingCard>() { ctx.CurrentPlayerTurn.Hand[0] }, null), ctx.CurrentPlayStage.Source.Target, ctx)) ;

            Assert.AreEqual(4, ctx.CurrentPlayerTurn.Hand.Count, "Hand count for player doesn't match");
            Assert.AreEqual(4, ctx.CurrentPlayerTurn.CurrentHealth, "P1 health isn't the expected value");

            ctx.RoateTurnStage();

            Assert.AreEqual(TurnStages.PostDiscard, ctx.CurrentTurnStage);

            ctx.RoateTurnStage().Perform(sender, ctx.CurrentPlayerTurn, ctx);

            ctx.RoateTurnStage().Perform(sender, ctx.CurrentPlayerTurn, ctx);


            Assert.AreEqual("P2", ctx.CurrentPlayerTurn.Display);
            Assert.AreEqual(4, ctx.CurrentPlayerTurn.CurrentHealth, "P2 health isn't the expected value");
        }
    }
}

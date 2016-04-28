using dab.SGS.Core.PlayingCards;
using dab.SGS.Core.PlayingCards.Basics;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dab.SGS.Core.Unit.Gameplay
{
    [TestClass]
    public class WineUnitTest
    {
        [TestMethod]
        public void TestWinePlayed()
        {
            Actions.Action action = null;

            // Always select the first card for discard (stupid I know right?)
            var ctx = new GameContext(new Deck(Unit.PlayTests.GetDeck()));

            ctx.AddPlayer("P1", null, Roles.King);
            ctx.AddPlayer("P2", null, Roles.Minister);
            ctx.AddPlayer("P3", null, Roles.Rebel);
            ctx.AddPlayer("P4", null, Roles.Rebel);
            ctx.AddPlayer("P5", null, Roles.TurnCoat);

            ctx.SetupGame();

            Assert.AreEqual(Roles.King, ctx.CurrentPlayerTurn.Role);
            Assert.AreEqual(4, ctx.CurrentPlayerTurn.Hand.Count);
            Assert.AreEqual(ctx.CurrentPlayerTurn, ctx.CurrentPlayerTurn.Hand[0].Owner);

            SelectedCardsSender sender = null;
            // Skip to the next stage in our turn
            while (ctx.CurrentTurnStage != TurnStages.Play)
            {
                action = ctx.RoateTurnStage();
                if (action != null)
                    action.Perform(sender, ctx.CurrentPlayerTurn, ctx);
            }


            Assert.AreEqual(TurnStages.Play, ctx.CurrentTurnStage);
            Assert.AreEqual(6, ctx.CurrentPlayerTurn.Hand.Count);

            // Insert a wine into the hand
            ctx.CurrentPlayerTurn.Hand.Add(new WineBasicPlayingCard(PlayingCardColor.Black, PlayingCardSuite.Club, "") { Context = ctx, Owner = ctx.CurrentPlayerTurn });

            // Play an attack.
            sender = new SelectedCardsSender(new List<PlayingCard>() { ctx.CurrentPlayerTurn.Hand.Find(p => p.IsPlayedAsWine()) },
                ctx.CurrentPlayerTurn.Hand.Find(p => p.IsPlayedAsWine()));

            // Play first playable card in the select cards (only 1 of the any should be playable).
            foreach (var card in sender) if (card.IsPlayable()) card.Play(sender);

            Assert.AreEqual(TurnStages.Play, ctx.CurrentTurnStage);

            action = ctx.RoateTurnStage();

            
            Assert.AreEqual(5, ctx.CurrentPlayerTurn.CurrentHealth);

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

        }


        [TestMethod]
        public void TestWineOnDeath()
        {
            Actions.Action action = null;

            // Always select the first card for discard (stupid I know right?)
            var ctx = new GameContext(new Deck(Unit.PlayTests.GetDeck()));

            ctx.AddPlayer("P1", null, Roles.King);
            ctx.AddPlayer("P2", null, Roles.Minister);
            ctx.AddPlayer("P3", null, Roles.Rebel);
            ctx.AddPlayer("P4", null, Roles.Rebel);
            ctx.AddPlayer("P5", null, Roles.TurnCoat);

            ctx.SetupGame();

            Assert.AreEqual(Roles.King, ctx.CurrentPlayerTurn.Role);

            Assert.AreEqual(4, ctx.CurrentPlayerTurn.Hand.Count);
            Assert.AreEqual(ctx.CurrentPlayerTurn, ctx.CurrentPlayerTurn.Hand[0].Owner);

            // Decrease player to the right's HP to 1.
            ctx.CurrentPlayerTurn.Right.CurrentHealth = 1;

            SelectedCardsSender sender = null;

            // Skip to the next stage in our turn
            while (ctx.CurrentTurnStage != TurnStages.Play)
            {
                action = ctx.RoateTurnStage();
                action.Perform(sender, ctx.CurrentPlayerTurn, ctx);
            }


            Assert.AreEqual(TurnStages.Play, ctx.CurrentTurnStage);
            Assert.AreEqual(6, ctx.CurrentPlayerTurn.Hand.Count);

            // Play an attack.
            sender = new SelectedCardsSender(new List<PlayingCard>()
                { ctx.CurrentPlayerTurn.Hand.Find(p => p.IsPlayedAsAttack()) },
                ctx.CurrentPlayerTurn.Hand.Find(p => p.IsPlayedAsAttack()));

            // Play first playable card in the select cards (only 1 of the any should be playable).
            foreach (var card in sender) if (card.IsPlayable()) card.Play(sender);

            Assert.AreEqual(TurnStages.AttackChooseTargets, ctx.CurrentTurnStage);

            action = ctx.RoateTurnStage();
            action.Perform(sender, ctx.CurrentPlayerTurn, ctx);

            Assert.AreEqual(TurnStages.AttackChooseTargets, ctx.CurrentTurnStage);
            Assert.AreEqual(TurnStages.Play, ctx.PreviousStages.Peek().Stage);

            // Choose a target (REUSE SENDER)
            ctx.CurrentPlayStage.Targets.Add(new TargetPlayer(ctx.CurrentPlayerTurn.Right));
            ctx.CurrentPlayerTurn.Hand[0].Play(sender); // Update target in attack

            action = ctx.RoateTurnStage();
            action.Perform(sender, ctx.CurrentPlayerTurn, ctx);

            Assert.AreEqual(TurnStages.AttackSkillResponse, ctx.CurrentTurnStage);

            // Rotate through our skills for defense, but they shouldn't exist
            action = ctx.RoateTurnStage();
            action.Perform(sender, ctx.CurrentPlayerTurn, ctx);

            Assert.AreEqual(TurnStages.AttackShieldResponse, ctx.CurrentTurnStage);
            // Attempt to execute our shield response, if it exists, but it shouldn't
            action = ctx.RoateTurnStage();
            action.Perform(sender, ctx.CurrentPlayerTurn, ctx);

            // Move on from CardResponse to Pre-Damage
            action = ctx.RoateTurnStage();
            action.Perform(sender, ctx.CurrentPlayerTurn, ctx);

            // Move on from Pre-Damage to Damage
            action = ctx.RoateTurnStage();
            // calls the .Play of the card again, which should move to player died
            action.Perform(sender, ctx.CurrentPlayerTurn, ctx);

            //ctx.CurrentPlayerTurn.Hand[0].Play(sender);

            Assert.AreEqual(TurnStages.PlayerDied, ctx.CurrentTurnStage);
            Assert.AreEqual(0, ctx.CurrentPlayerTurn.CurrentHealth);


            // Insert a peach into the hand
            ctx.CurrentPlayerTurn.Hand.Add(new WineBasicPlayingCard(PlayingCardColor.Black, PlayingCardSuite.Club, "") { Context = ctx, Owner = ctx.CurrentPlayerTurn });

            ctx.CurrentPlayerTurn.Hand.Find(p => p.IsPlayedAsWine()).Play(new SelectedCardsSender() { ctx.CurrentPlayerTurn.Right.Hand.Find(p => p.IsPlayedAsWine()) });

            Assert.AreEqual(TurnStages.PlayerRevived, ctx.CurrentTurnStage);
            Assert.AreEqual(1, ctx.CurrentPlayerTurn.CurrentHealth);


            action = ctx.RoateTurnStage();
            action.Perform(sender, ctx.CurrentPlayerTurn, ctx);

            // Continue with Attack damage
            Assert.AreEqual(TurnStages.AttackDamage, ctx.CurrentTurnStage);

            action = ctx.RoateTurnStage();
            action.Perform(sender, ctx.CurrentPlayerTurn, ctx);

            Assert.AreEqual(TurnStages.AttackEnd, ctx.CurrentTurnStage);

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
        }
    }
}

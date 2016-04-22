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
    public class PeachUnitTest
    {
        [TestMethod]
        public void TestPeachPlayValidLowHealth()
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

            // Pretend our current user has 1 less health
            ctx.CurrentPlayerTurn.CurrentHealth--;

            // Insert a wine into the hand
            ctx.CurrentPlayerTurn.Hand.Add(new PeachBasicPlayingCard(PlayingCardColor.Black, PlayingCardSuite.Club, "") { Context = ctx, Owner = ctx.CurrentPlayerTurn });

            // Play an attack.
            sender = new SelectedCardsSender(new List<PlayingCard>() { ctx.CurrentPlayerTurn.Hand.Find(p => p.IsPlayedAsPeach()) },
                ctx.CurrentPlayerTurn.Hand.Find(p => p.IsPlayedAsPeach()));

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
        public void TestPeachPlayValidMaxHealth()
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
            ctx.CurrentPlayerTurn.Hand.Add(new PeachBasicPlayingCard(PlayingCardColor.Black, PlayingCardSuite.Club, "") { Context = ctx, Owner = ctx.CurrentPlayerTurn });

            // Play an attack.
            sender = new SelectedCardsSender(new List<PlayingCard>() { ctx.CurrentPlayerTurn.Hand.Find(p => p.IsPlayedAsPeach()) },
                ctx.CurrentPlayerTurn.Hand.Find(p => p.IsPlayedAsPeach()));

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
    }
}

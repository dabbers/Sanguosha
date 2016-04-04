using System;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using dab.SGS.Core;
using dab.SGS.Core.PlayingCard;
using dab.SGS.Core.Actions;
using System.Collections.Generic;

namespace dab.SGS.Core.Unit
{
    [TestClass]
    public class PlayTests
    {
        public List<PlayingCard.PlayingCard> GetDeck()
        {
            var deck = Deck.LoadCards(FileIOHelper.ReadFromDefaultFile("ms-appx:///Assets/deck.json"),
                (p, r) => p.Right, p => p.Hand[0], p => true);

            return deck;
        }


        [TestMethod]
        public void TestRound()
        {
            Actions.Action action = null;

            // Always select the first card for discard (stupid I know right?)
            var ctx = new GameContext(new Deck(this.GetDeck()), p => p.Hand[0]); 

            ctx.AddPlayer("P1", null, Roles.King);
            ctx.AddPlayer("P2", null, Roles.Minister);
            ctx.AddPlayer("P3", null, Roles.Rebel);
            ctx.AddPlayer("P4", null, Roles.Rebel);
            ctx.AddPlayer("P5", null, Roles.TurnCoat);

            //ctx.Deck = ;

            ctx.SetupGame();

            Assert.AreEqual(Roles.King, ctx.Turn.Role);
            Assert.AreEqual(4, ctx.Turn.Hand.Count);
            Assert.AreEqual(ctx.Turn, ctx.Turn.Hand[0].Owner);

            // Skip to the next stage in our turn
            while (ctx.TurnStage != TurnStages.Play)
            {
                action = ctx.RoateTurnStage();
                if (action != null)
                    action.Perform(ctx, ctx.Turn, ctx);
            }


            Assert.AreEqual(TurnStages.Play, ctx.TurnStage);
            Assert.AreEqual(6, ctx.Turn.Hand.Count);

            // Play an attack.
            ctx.Turn.Hand[0].Play();

            Assert.AreEqual(TurnStages.ChooseTargets, ctx.TurnStage);

            ctx.AttackStageTracker.Targets.Add(ctx.Turn.Right);



            // SKip to the end stage of our turn
            while (ctx.TurnStage != TurnStages.End)
            {
                action = ctx.RoateTurnStage();
                if (action != null)
                    action.Perform(ctx, ctx.Turn, ctx);
            }

            Assert.AreEqual(5, ctx.Turn.Hand.Count);

        }
    }
}

using System;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using dab.SGS.Core;
using dab.SGS.Core.PlayingCards;
using dab.SGS.Core.Actions;
using System.Collections.Generic;

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
            var sender = new SelectedCardsSender(new List<PlayingCard>() { ctx.Turn.Hand[0] }, ctx.Turn.Hand[0]);

            ctx.Turn.Hand[0].Play(sender);

            Assert.AreEqual(TurnStages.ChooseTargets, ctx.TurnStage);

            // Choose a target (REUSE SENDER)
            ctx.AttackStageTracker.Targets.Add(new TargetPlayer() { Target = ctx.Turn.Right });
            ctx.Turn.Hand[0].Play(sender); // Update target in attack

            action = ctx.RoateTurnStage();
            Assert.IsNull(action);

            Assert.AreEqual(TurnStages.SkillResponse, ctx.TurnStage);

            // Rotate through our skills for defense, but they shouldn't exist
            action = ctx.RoateTurnStage();
            Assert.IsNull(action);

            Assert.AreEqual(TurnStages.ShieldResponse, ctx.TurnStage);
            // Attempt to execute our shield response, if it exists, but it shouldn't
            action = ctx.RoateTurnStage();
            Assert.IsNull(action);

            foreach(var target in ctx.AttackStageTracker.Targets)
            {
                // Our target must play a dodge or take damage. Poor soul
                var dodge = target.Target.Hand.Find(p => p.Display == "Dodge");

                dodge.Play(dodge);
            }

            // Move on from CardResponse to Pre-Damage
            action = ctx.RoateTurnStage();
            Assert.IsNull(action);

            // Move on from Pre-Damage to Damage
            action = ctx.RoateTurnStage();
            Assert.IsNull(action);

            ctx.Turn.Hand[0].Play(sender);

            Assert.AreEqual(TurnStages.Play, ctx.TurnStage);
            Assert.AreEqual(4, ctx.Turn.Right.CurrentHealth);

            // SKip to the end stage of our turn
            while (ctx.TurnStage != TurnStages.End)
            {
                action = ctx.RoateTurnStage();
                if (action != null)
                    action.Perform(ctx, ctx.Turn, ctx);
            }

            Assert.AreEqual(5, ctx.Turn.Hand.Count);

            ctx.RoateTurnStage();

            Assert.AreEqual("P2", ctx.Turn.Display);
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
            var sender = new SelectedCardsSender(new List<PlayingCard>() { ctx.Turn.Hand[0] }, ctx.Turn.Hand[0]);

            ctx.Turn.Hand[0].Play(sender);

            Assert.AreEqual(TurnStages.ChooseTargets, ctx.TurnStage);

            // Choose a target (REUSE SENDER)
            ctx.AttackStageTracker.Targets.Add(new TargetPlayer() { Target = ctx.Turn.Right });
            ctx.Turn.Hand[0].Play(sender); // Update target in attack

            action = ctx.RoateTurnStage();
            Assert.IsNull(action);

            Assert.AreEqual(TurnStages.SkillResponse, ctx.TurnStage);

            // Rotate through our skills for defense, but they shouldn't exist
            action = ctx.RoateTurnStage();
            Assert.IsNull(action);

            Assert.AreEqual(TurnStages.ShieldResponse, ctx.TurnStage);
            // Attempt to execute our shield response, if it exists, but it shouldn't
            action = ctx.RoateTurnStage();
            Assert.IsNull(action);
            
            // Move on from CardResponse to Pre-Damage
            action = ctx.RoateTurnStage();
            Assert.IsNull(action);

            // Move on from Pre-Damage to Damage
            action = ctx.RoateTurnStage();
            Assert.IsNull(action);

            ctx.Turn.Hand[0].Play(sender);

            Assert.AreEqual(TurnStages.Play, ctx.TurnStage);
            Assert.AreEqual(3, ctx.Turn.Right.CurrentHealth);

            // SKip to the end stage of our turn
            while (ctx.TurnStage != TurnStages.End)
            {
                action = ctx.RoateTurnStage();
                if (action != null)
                    action.Perform(ctx, ctx.Turn, ctx);
            }

            Assert.AreEqual(5, ctx.Turn.Hand.Count);

            ctx.RoateTurnStage();

            Assert.AreEqual("P2", ctx.Turn.Display);
        }
    }
}

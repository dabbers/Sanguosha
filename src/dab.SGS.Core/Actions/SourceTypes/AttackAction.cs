using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dab.SGS.Core.PlayingCards;

namespace dab.SGS.Core.Actions
{
    public class AttackAction : Action
    {
        public AttackAction(string display, int damage) : base(display)
        {
            this.damage = damage;
        }

        public override bool Perform(SelectedCardsSender sender, Player player, GameContext context)
        {
            switch (context.CurrentPlayStage.Stage)
            {
                // The attack was first played. Go into target select mode.
                case TurnStages.Play:
                    // An attack cannot be happening already
                    if (context.PreviousStages.Count != 0)
                    {
                        throw new Exception("There are other turnstages in the stack. Are we expecting this? (Previous turnstage:" + context.PreviousStages.Peek().ToString());
                    }

                    context.PreviousStages.Push(context.CurrentPlayStage);
                    var results = sender;


                    context.CurrentPlayStage = new PlayingCardStageTracker()
                    {
                        Cards = results,
                        Source = new TargetPlayer(player),
                        Targets = new List<TargetPlayer>(),
                        Stage = TurnStages.AttackChooseTargets
                    };

                    context.CurrentPlayStage.ExpectingIputFrom = context.CurrentPlayStage.Source;
                    
                    return false;
                case TurnStages.AttackChooseTargets:

                    foreach (var tp in context.CurrentPlayStage.Targets)
                    {
                        tp.Damage = this.damage;
                    }

                    context.CurrentPlayStage.ExpectingIputFrom = null;

                    return false;
                case TurnStages.AttackDamage:
                    var tmpStage = context.PreviousStages.Pop();

                    foreach(var tp in context.CurrentPlayStage.Targets)
                    {
                        if (tp.Result == TargetResult.Success || tp.Result == TargetResult.None)
                        {
                            // adjust for shield damage
                            tp.Target.CurrentHealth -= tp.Target.PlayerArea.Shield?.GetExtraDamage(context.CurrentPlayStage, context.CurrentPlayStage.Source.Target.PlayerArea.Weapon) ?? 0;

                            tp.Target.CurrentHealth -= tp.Damage;
                        }

                    }

                    // Clear up the stage tracker for the next turn.
                    context.CurrentPlayStage = tmpStage;
                    break;
                case TurnStages.PlayScrollPlaceResponse:
                case TurnStages.PlayScrollPlaced:
                    // Duel:
                    if (context.CurrentPlayStage.Cards.Activator.IsPlayedAsDuel())
                    {
                        context.CurrentTurnStage = TurnStages.PlayScrollPlaceResponse;

                        // Reset the next player so they can play their card.
                        context.CurrentPlayStage.ExpectingIputFrom.Result = TargetResult.Failed;
                        context.CurrentPlayStage.PeristedTargetEnumerator.PeekRotate.Result = TargetResult.None;
                        break;
                    }

                    throw new Exception("Attack was somehow activated for PlayScrollPlace(Response). But activator wasn't played as duel." + context.CurrentPlayStage.Cards.Activator.ToString());
                default:
                    throw new Exception("Unknown stage reached for attack action: " + context.CurrentPlayStage.Stage.ToString());
            }

            sender?.DiscardAll();
            return true;
        }

        public static new Action ActionFromJson(dynamic obj,
            SelectCard selectCard, IsValidCard validCard)
        {
            var display = obj.Display.ToString();
            int damage = obj.Damage;

            return new AttackAction(display, damage);
        }
        
        private int damage;
    }
}

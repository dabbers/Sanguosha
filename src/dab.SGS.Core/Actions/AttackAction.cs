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

        public override bool Perform(object sender, Player player, GameContext context)
        {
            // When can this happen? Attack card, or Borrowed sword

            // How attacking a player works:
            var results = (SelectedCardsSender)sender;

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

                    context.CurrentPlayStage = new PlayingCardStageTracker()
                    {
                        Cards = results,
                        Source = new TargetPlayer(player),
                        Targets = new List<TargetPlayer>(),
                        Stage = TurnStages.AttackPreStage
                    };
                    context.CurrentPlayStage.ExpectingIputFrom = context.CurrentPlayStage.Source;
                    
                    return false;
                case TurnStages.AttackChooseTargets:

                    foreach (var tp in context.CurrentPlayStage.Targets)
                    {
                        tp.Damage = this.damage;
                    }

                    //context.CurrentPlayStage.Stage++;
                    return false;
                case TurnStages.Damage:
                    var tmpStage = context.PreviousStages.Pop();

                    foreach(var tp in context.CurrentPlayStage.Targets)
                    {
                        // adjust for shield damage
                        tp.Target.CurrentHealth -= tp.Target.PlayerArea.Shield.GetExtraDamage(context.CurrentPlayStage, context.CurrentPlayStage.Source.Target.PlayerArea.Weapon);

                        tp.Target.CurrentHealth -= tp.Damage;
                    }

                    // Clear up the stage tracker for the next turn.
                    context.CurrentPlayStage = tmpStage;
                    break;
                case TurnStages.PlayScrollPlace:
                    // Duel:
                    if (context.CurrentPlayStage.Cards.Activator.IsPlayedAsDuel())
                    {
                        if (context.CurrentPlayStage.Source.Target != results.Activator.Owner)
                        {
                            context.CurrentPlayStage.ExpectingIputFrom = context.CurrentPlayStage.Source;
                        }
                        else
                        {
                            context.CurrentPlayStage.ExpectingIputFrom = context.CurrentPlayStage.Targets.First();
                        }
                        break;
                    }

                    throw new Exception("Attack was somehow activated for PlayScrollPlace. " + context.CurrentPlayStage.Cards.Activator.ToString());
                default:
                    throw new Exception("Unknown stage reached for attack action: " + context.CurrentPlayStage.Stage.ToString());
            }


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

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

            switch (context.TurnStage)
            {
                // The attack was first played. Go into target select mode.
                case TurnStages.Play:
                    // An attack cannot be happening already
                    if (context.PlayStageTracker != null)
                    {
                        throw new Exception("An attack is already happening. Is this a bug?");
                    }


                    context.PlayStageTracker = new PlayingCardStageTracker()
                    {
                        Cards = results,
                        Source = new TargetPlayer(player),
                        Targets = new List<TargetPlayer>(),
                        PreviousStages = new Stack<TurnStages>()
                    };

                    context.PlayStageTracker.PreviousStages.Push(context.TurnStage);

                    context.TurnStage = TurnStages.ChooseTargets;
                    return false;
                case TurnStages.ChooseTargets:

                    foreach (var tp in context.PlayStageTracker.Targets)
                    {
                        tp.Damage = this.damage;
                    }

                    //context.TurnStage++;
                    return false;
                case TurnStages.Damage:
                    context.TurnStage = context.PlayStageTracker.PreviousStages.Pop();

                    foreach(var tp in context.PlayStageTracker.Targets)
                    {
                        // adjust for shield damage
                        tp.Target.CurrentHealth -= tp.Target.PlayerArea.Shield.GetExtraDamage(context.PlayStageTracker, context.PlayStageTracker.Source.Target.PlayerArea.Weapon);

                        tp.Target.CurrentHealth -= tp.Damage;
                    }

                    // Clear up the stage tracker for the next turn.
                    context.PlayStageTracker = null;
                    break;
                case TurnStages.PlayScrollPlace:
                    // Duel:
                    if (context.PlayStageTracker.Cards.Activator.IsPlayedAsDuel())
                    {
                        if (context.PlayStageTracker.Source.Target != results.Activator.Owner)
                        {
                            context.PlayStageTracker.ExpectingIputFrom = context.PlayStageTracker.Source;
                        }
                        else
                        {
                            context.PlayStageTracker.ExpectingIputFrom = context.PlayStageTracker.Targets.First();
                        }
                        break;
                    }

                    throw new Exception("Attack was somehow activated for PlayScrollPlace. " + context.PlayStageTracker.Cards.Activator.ToString());
                default:
                    throw new Exception("Unknown stage reached for attack action: " + context.TurnStage.ToString());
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dab.SGS.Core.Actions
{
    public class DuelAction : Action
    {
        public DuelAction(string display) : base(display)
        {
        }

        public override bool Perform(object sender, Player player, GameContext context)
        {
            var results = (SelectedCardsSender)sender;

            switch(context.TurnStage)
            {
                // Just played this card
                case TurnStages.Play:

                    // An attack cannot be happening already
                    if (context.PlayStageTracker != null)
                    {
                        throw new Exception("Something is already happening. Is this a bug?");
                    }
                    
                    context.PlayStageTracker = new PlayingCardStageTracker()
                    {
                        Cards = results,
                        Source = new TargetPlayer(player),
                        Targets = new List<TargetPlayer>(),
                        PreviousStages = new Stack<TurnStages>()
                    };

                    context.PlayStageTracker.PreviousStages.Push(context.TurnStage);

                    context.TurnStage = TurnStages.PlayScrollTargets;
                    return false;
                case TurnStages.PlayScrollTargets:

                    foreach (var tp in context.PlayStageTracker.Targets)
                    {
                        tp.Damage = 1;
                    }
                    return false;
                case TurnStages.PlayScrollPlace:

                    context.TurnStage = context.PlayStageTracker.PreviousStages.Pop();

                    // Todo: Does duel take shield into consideation?
                    foreach (var tp in context.PlayStageTracker.Targets)
                    {
                        // adjust for shield damage
                        //tp.Target.CurrentHealth -= tp.Target.PlayerArea.Shield.GetExtraDamage(context.PlayStageTracker, context.PlayStageTracker.Source.Target.PlayerArea.Weapon);

                        tp.Target.CurrentHealth -= tp.Damage;
                    }

                    // Clear up the stage tracker for the next turn.
                    context.PlayStageTracker = null;

                    return false;
                default:
                    throw new Exception("Invalid turn stage for duel: " + context.TurnStage.ToString());
            }
        }
    }
}

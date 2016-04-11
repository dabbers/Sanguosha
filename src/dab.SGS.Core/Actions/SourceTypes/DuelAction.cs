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

        public DuelAction() : base("Duel Action")
        {
        }

        public override bool Perform(object sender, Player player, GameContext context)
        {
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
                        Stage = TurnStages.PlayScrollPreStage
                    };

                    context.CurrentPlayStage.ExpectingIputFrom = context.CurrentPlayStage.Source;
                    return false;
                case TurnStages.PlayScrollTargets:

                    foreach (var tp in context.CurrentPlayStage.Targets)
                    {
                        tp.Damage = 1;
                    }
                    return false;
                case TurnStages.PlayScrollPlace:

                    var tmpStage = context.PreviousStages.Pop();

                    // Todo: Does duel take shield into consideation?
                    foreach (var tp in context.CurrentPlayStage.Targets)
                    {
                        // adjust for shield damage
                        //tp.Target.CurrentHealth -= tp.Target.PlayerArea.Shield.GetExtraDamage(context.PlayStageTracker, context.PlayStageTracker.Source.Target.PlayerArea.Weapon);

                        tp.Target.CurrentHealth -= tp.Damage;
                    }

                    // Clear up the stage tracker for the next turn.
                    context.CurrentPlayStage = tmpStage;

                    return false;
                default:
                    throw new Exception("Invalid turn stage for duel: " + context.CurrentPlayStage.Stage.ToString());
            }
        }
    }
}

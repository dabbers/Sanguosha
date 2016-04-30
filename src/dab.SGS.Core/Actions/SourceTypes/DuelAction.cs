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

        public override bool Perform(SelectedCardsSender sender, Player player, GameContext context)
        {
            switch (context.CurrentPlayStage.Stage)
            {
                case TurnStages.PlayScrollEnd:

                    var tmpStage = context.PreviousStages.Pop();

                    // Todo: Does duel take shield into consideation?
                    foreach (var tp in context.CurrentPlayStage.Targets)
                    {
                        if (tp.Result == TargetResult.None || tp.Result == TargetResult.Success)
                        {
                            // adjust for shield damage
                            //tp.Target.CurrentHealth -= tp.Target.PlayerArea.Shield.GetExtraDamage(context.PlayStageTracker, context.PlayStageTracker.Source.Target.PlayerArea.Weapon);

                            //tp.Target.CurrentHealth -= tp.Damage;
                            new ReduceHealthToTargetAction(tp.Damage).Perform(sender, tp.Target, context);
                        }
                    }

                    // Clear up the stage tracker for the next turn.
                    context.CurrentPlayStage = tmpStage;
                    return true;
                default:
                    throw new Exceptions.InvalidScenarioException("Invalid turn stage for duel: " + context.CurrentPlayStage.Stage.ToString());
            }
        }
    }
}

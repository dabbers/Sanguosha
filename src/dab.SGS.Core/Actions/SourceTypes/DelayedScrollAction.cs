using dab.SGS.Core.PlayingCards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dab.SGS.Core.Actions
{
    public class DelayedScrollAction : Action
    {
        public DelayedScrollAction(string display, Action scrollAction) : base(display)
        {
        }

        public override bool Perform(SelectedCardsSender sender, Player player, GameContext context)
        {
            switch (context.CurrentPlayStage.Stage)
            {
                case TurnStages.PlayScrollTargets:

                    context.CurrentPlayStage.Targets.Add(context.CurrentPlayStage.Source);

                    foreach (var tp in context.CurrentPlayStage.Targets)
                    {
                        tp.Damage = 1;
                    }

                    return false;
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
                case TurnStages.PreJudgement:

                    return false;
                case TurnStages.Judgement:

                    this.scrollAction.Perform(new SelectedCardsSender() { context.Deck.Draw() }, player, context);
                    return true;
            }

            return true;

        }

        private Action scrollAction = null;
    }
}

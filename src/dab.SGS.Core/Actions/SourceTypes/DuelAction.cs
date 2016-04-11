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
                        Stage = TurnStages.PlayScrollTargets
                    };

                    context.CurrentPlayStage.ExpectingIputFrom = context.CurrentPlayStage.Source;
                    return false;
                case TurnStages.PlayScrollTargets:

                    foreach (var tp in context.CurrentPlayStage.Targets)
                    {
                        tp.Damage = 1;
                    }

                    context.CurrentPlayStage.ExpectingIputFrom = null;

                    // Add the ability for the activator card to be applied automatically. This way, when we move to the next stage, we can automatically get the player input.
                    context.CurrentPlayStage.Source.Target.TurnStageActions.Add(TurnStages.PlayScrollPlace, new Actions.PerformCardAction(() => context.CurrentPlayStage.Cards.Activator), true);

                    return false;
                case TurnStages.PlayScrollPlace:

                    var tmpStage = context.PreviousStages.Pop();

                    // Todo: Does duel take shield into consideation?
                    foreach (var tp in context.CurrentPlayStage.Targets)
                    {
                        if (tp.Result == TargetResult.None || tp.Result == TargetResult.Success)
                        {
                            // adjust for shield damage
                            //tp.Target.CurrentHealth -= tp.Target.PlayerArea.Shield.GetExtraDamage(context.PlayStageTracker, context.PlayStageTracker.Source.Target.PlayerArea.Weapon);

                            tp.Target.CurrentHealth -= tp.Damage;
                        }
                        
                    }

                    // Clear up the stage tracker for the next turn.
                    context.CurrentPlayStage = tmpStage;

                    var action = context.CurrentPlayStage.Source.Target.TurnStageActions[TurnStages.PlayScrollPlace];

                    // Remove our action from the chain so we don't get this called again for the next scroll (unless a duel is played, but
                    // let us re-add it later).
                    if (action.GetType() == typeof(ChainedActions))
                    {
                        ((ChainedActions)action).Actions.Remove(((ChainedActions)action).Actions.Find(p => p.GetType() == typeof(PerformCardAction)));
                    }

                    return false;
                default:
                    throw new Exception("Invalid turn stage for duel: " + context.CurrentPlayStage.Stage.ToString());
            }
        }
    }
}

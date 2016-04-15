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
            var results = sender;

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

                    context.CurrentPlayStage.ExpectingIputFrom.Player = context.CurrentPlayStage.Source;
                    context.CurrentPlayStage.ExpectingIputFrom.Prompt = new Prompts.UserPrompt(Prompts.UserPromptType.TargetRangeMN)
                        { MinRange = 1, MaxRange = 999 };

                    return false;
                case TurnStages.PlayScrollTargets:

                    context.CurrentPlayStage.Targets.Add(context.CurrentPlayStage.Source);

                    foreach (var tp in context.CurrentPlayStage.Targets)
                    {
                        tp.Damage = 1;
                    }

                    context.CurrentPlayStage.ExpectingIputFrom.Player = null;

                    // Add the ability for the activator card to be applied automatically. This way, when we move to the next stage, we can automatically get the player input.
                    context.CurrentPlayStage.Source.Target.TurnStageActions.Add(TurnStages.PlayScrollPlaced, new Actions.PerformCardAction(() => context.CurrentPlayStage.Cards.Activator), true);
                    context.CurrentPlayStage.Source.Target.TurnStageActions.Add(TurnStages.PlayScrollPlaceResponse, new Actions.PerformCardAction(() => context.CurrentPlayStage.Cards.Activator), true);
                    context.CurrentPlayStage.Source.Target.TurnStageActions.Add(TurnStages.PlayScrollEnd, new Actions.PerformCardAction(() => context.CurrentPlayStage.Cards.Activator), true);

                    return false;
                case TurnStages.PlayScrollPlaced:
                    for(var i = context.CurrentPlayStage.Targets.Count - 1; i >= 0 ; i--)
                    {
                        var target = context.CurrentPlayStage.Targets[i];

                        if (target.Result == TargetResult.Warded)
                        {
                            context.CurrentPlayStage.Targets.Remove(target);
                        }

                        if (target.Target == context.CurrentPlayStage.Source.Target)
                        {
                            target.Result = TargetResult.Failed;
                        }
                    }

                    if (context.CurrentPlayStage.Targets.Count > 1)
                    {
                        context.CurrentPlayStage.PeristedTargetEnumerator = new PeekEnumerator<TargetPlayer>(context.CurrentPlayStage.Targets.GetEnumerator());
                        context.CurrentPlayStage.PeristedTargetEnumerator.MoveNext();

                        context.CurrentPlayStage.ExpectingIputFrom.Player = context.CurrentPlayStage.PeristedTargetEnumerator.Current;
                        context.CurrentPlayStage.ExpectingIputFrom.Prompt = new Prompts.UserPrompt(Prompts.UserPromptType.CardsPlayerHand);

                        context.CurrentPlayStage.Stage = TurnStages.PlayScrollPlaceResponse;
                    }
                    else
                    {
                        context.CurrentPlayStage.ExpectingIputFrom.Player = null;
                    }
                    context.CurrentPlayStage.Stage = TurnStages.PlayScrollPlaceResponse;

                    return false;
                case TurnStages.PlayScrollPlaceResponse:

                    // Loop through our target list (intentionally doing this)
                    var previous = context.CurrentPlayStage.ExpectingIputFrom;

                    context.CurrentPlayStage.PeristedTargetEnumerator.MoveNextReset();
                    
                    context.CurrentPlayStage.ExpectingIputFrom.Player = context.CurrentPlayStage.PeristedTargetEnumerator.Current;
                    context.CurrentPlayStage.ExpectingIputFrom.Prompt = new Prompts.UserPrompt(Prompts.UserPromptType.CardsPlayerHand);

                    // If the targetresult is not none, it wasn't reset because the previous player didn't play an attack.
                    // If this is the case, the previous player should recieve damage.
                    if (context.CurrentPlayStage.ExpectingIputFrom.Player.Result != TargetResult.None || (previous?.Player.Result ?? TargetResult.Failed) == TargetResult.None)
                    {
                        if (previous != null && previous.Player.Result != TargetResult.Warded)
                        {
                            previous.Player.Result = TargetResult.Success;

                        }

                        foreach (var target in context.CurrentPlayStage.Targets)
                        {
                            if (target != previous.Player) target.Result = TargetResult.Failed;
                        }

                        // this lets the turnstage progress, and tells the game engine
                        // we aren't expecting any new player input.
                        context.CurrentPlayStage.ExpectingIputFrom.Player = null;
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

                            tp.Target.CurrentHealth -= tp.Damage;
                        }
                    }

                    // Clear up the stage tracker for the next turn.
                    context.CurrentPlayStage = tmpStage;

                    var action = context.CurrentPlayStage.Source.Target.TurnStageActions[TurnStages.PlayScrollPlaced];

                    // Remove our DuelAction from the chain so we don't get this called again for the next scroll played. (or get called twice if we played another duel)
                    if (action.GetType() == typeof(ChainedActions))
                    {
                        ((ChainedActions)action).Actions.Remove(((ChainedActions)action).Actions.Find(p => p.GetType() == typeof(PerformCardAction)));
                    }

                    action = context.CurrentPlayStage.Source.Target.TurnStageActions[TurnStages.PlayScrollPlaceResponse];

                    // Remove our DuelAction from the chain so we don't get this called again for the next scroll played. (or get called twice if we played another duel)
                    if (action.GetType() == typeof(ChainedActions))
                    {
                        ((ChainedActions)action).Actions.Remove(((ChainedActions)action).Actions.Find(p => p.GetType() == typeof(PerformCardAction)));
                    }

                    action = context.CurrentPlayStage.Source.Target.TurnStageActions[TurnStages.PlayScrollEnd];

                    // Remove our DuelAction from the chain so we don't get this called again for the next scroll played. (or get called twice if we played another duel)
                    if (action.GetType() == typeof(ChainedActions))
                    {
                        ((ChainedActions)action).Actions.Remove(((ChainedActions)action).Actions.Find(p => p.GetType() == typeof(PerformCardAction)));
                    }
                    sender.DiscardAll();
                    return true;
                default:
                    throw new Exception("Invalid turn stage for duel: " + context.CurrentPlayStage.Stage.ToString());
            }
        }
    }
}

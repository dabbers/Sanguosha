using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dab.SGS.Core.Actions
{

    public class ScrollAction : Action
    {
        public const int AttackRange = -1;
        public const int TargetRange = 0;
        public const int AnyTarget = 999;

        public ScrollAction(int minTarget, int maxTarget, int minRange, int maxRange) : base("Scroll Action")
        {
            this.minRange = minRange;
            this.maxRange = maxRange;
            this.minTargets = minTarget;
            this.maxTargets = maxTarget;
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
                        throw new Exceptions.InvalidScenarioException("There are other turnstages in the stack. Are we expecting this? (Previous turnstage:" + context.PreviousStages.Peek().ToString());
                    }

                    context.PreviousStages.Push(context.CurrentPlayStage);

                    context.CurrentPlayStage = new PlayingCardStageTracker()
                    {
                        Cards = sender,
                        Source = new TargetPlayer(player),
                        Targets = new List<TargetPlayer>(),
                        Stage = TurnStages.PlayScrollTargets
                    };

                    context.CurrentPlayStage.ExpectingIputFrom.Player = context.CurrentPlayStage.Source;
                    context.CurrentPlayStage.ExpectingIputFrom.Prompt = new Prompts.UserPrompt(Prompts.UserPromptType.TargetRangeMN)
                        { MinRange = this.minRange, MaxRange = this.maxRange, MinTargets = this.minTargets, MaxTargets = this.maxTargets };

                    return false;
                case TurnStages.PlayScrollTargets:

                    // Add the ability for the activator card to be applied automatically. This way, when we move to the next stage, we can automatically get the player input.
                    context.CurrentPlayStage.Source.Target.TurnStageActions.Add(TurnStages.PlayScrollPlaced, new TurnScopeAction("Perform Attack Card", new PerformCardAction(() => context.CurrentPlayStage.Cards.Activator)), true);
                    context.CurrentPlayStage.Source.Target.TurnStageActions.Add(TurnStages.PlayScrollPlaceResponse, new TurnScopeAction("Perform Attack Card", new PerformCardAction(() => context.CurrentPlayStage.Cards.Activator)), true);
                    context.CurrentPlayStage.Source.Target.TurnStageActions.Add(TurnStages.PlayScrollEnd, new TurnScopeAction("Perform Attack Card", new PerformCardAction(() => context.CurrentPlayStage.Cards.Activator)), true);

                    context.CurrentPlayStage.ExpectingIputFrom.Player = null;
                    return true; // another action must return false otherwise the card/action flow won't continue.
                case TurnStages.PlayScrollPlaced:
                    for (var i = context.CurrentPlayStage.Targets.Count - 1; i >= 0; i--)
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
                    var previous = context.CurrentPlayStage.ExpectingIputFrom.Player;

                    context.CurrentPlayStage.PeristedTargetEnumerator.MoveNextReset();

                    context.CurrentPlayStage.ExpectingIputFrom.Player = context.CurrentPlayStage.PeristedTargetEnumerator.Current;
                    context.CurrentPlayStage.ExpectingIputFrom.Prompt = new Prompts.UserPrompt(Prompts.UserPromptType.CardsPlayerHand);

                    // If the targetresult is not none, it wasn't reset because the previous player didn't play an attack.
                    // If this is the case, the previous player should recieve damage.
                    if (context.CurrentPlayStage.ExpectingIputFrom.Player.Result != TargetResult.None || (previous?.Result ?? TargetResult.Failed) == TargetResult.None)
                    {
                        if (previous != null && previous.Result != TargetResult.Warded)
                        {
                            previous.Result = TargetResult.Success;
                        }

                        foreach (var target in context.CurrentPlayStage.Targets)
                        {
                            if (target != previous) target.Result = TargetResult.Failed;
                        }

                        // this lets the turnstage progress, and tells the game engine
                        // we aren't expecting any new player input.
                        context.CurrentPlayStage.ExpectingIputFrom.Player = null;
                    }

                    return false;
                case TurnStages.PlayScrollEnd:
                    sender.DiscardAll();
                    return true;
                case TurnStages.PreJudgement:

                    return true;
                case TurnStages.Judgement:
                    return true;
                default:
                    throw new Exceptions.InvalidScenarioException("Invalid turn stage for ScrollAction: " + context.CurrentPlayStage.Stage.ToString());
            }
        } // end Perform

        public static new Action ActionFromJson(dynamic obj,
            SelectCard selectCard, IsValidCard validCard)
        {
            return new ScrollAction((int)obj.MinTargets, (int)obj.MaxTargets, (int)obj.MinRange, (int)obj.MaxRange);
        }

        private int minTargets = 1;
        private int maxTargets = 1;
        private int minRange = 1;
        private int maxRange = 1;
    }
}

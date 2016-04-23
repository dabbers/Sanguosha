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
        public DelayedScrollAction(string display) : base(display)
        {
        }

        public override bool Perform(SelectedCardsSender sender, Player player, GameContext context)
        {
            switch (context.CurrentPlayStage.Stage)
            {
                case TurnStages.Play:

                    context.PreviousStages.Push(context.CurrentPlayStage);


                    context.CurrentPlayStage = new PlayingCardStageTracker()
                    {
                        Cards = sender,
                        Source = new TargetPlayer(player),
                        Targets = new List<TargetPlayer>(),
                        Stage = TurnStages.AttackChooseTargets
                    };

                    context.CurrentPlayStage.ExpectingIputFrom.Player = context.CurrentPlayStage.Source;
                    context.CurrentPlayStage.ExpectingIputFrom.Prompt = new Prompts.UserPrompt(Prompts.UserPromptType.TargetRangeMN)
                        { MinRange = 1, MaxRange = player.GetAttackRange(), MaxCards = 1, MinTargets = 1 };
                    return false;
                case TurnStages.PlayScrollTargets:
                    context.CurrentPlayStage.ExpectingIputFrom.Player = null;

                    context.CurrentPlayStage.Targets.First().Target.PlayerArea.DelayedScrolls.Add(sender.Activator);


                    {
                        foreach (var card in sender)
                        {
                            if (card != sender.Activator) card.Discard();
                        }
                    }

                    sender.Activator.Owner.Hand.Remove(sender.Activator);

                    context.CurrentPlayStage = context.PreviousStages.Pop();
                    return false;
                case TurnStages.Judgement:


                    {
                        var card = context.Deck.Draw();

                        this.scrollAction.Perform(new SelectedCardsSender() { card }, player, context);

                        card.Discard();
                        sender.Activator.Discard();
                    }

                    return true;
            }

            return true;

        }

        private Action scrollAction = null;
    }
}

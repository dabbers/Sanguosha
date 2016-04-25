using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dab.SGS.Core.Actions
{
    public class PlayerDiedAction : Action
    {
        public PlayerDiedAction() : base("Player Died")
        {
        }

        public override bool Perform(SelectedCardsSender sender, Player player, GameContext context)
        {

            switch (context.CurrentTurnStage)
            {
                case TurnStages.PlayerDied:
                    throw new Exception("PlayerDied in PlayerDiedAction should not be called here.");

                case TurnStages.PlayerDiedPreStage:


                    throw new Exception("PlayerDied pre stage shouldn't ever be used");
                case TurnStages.PlayerRevived:
                case TurnStages.PlayerRevivedEnd:

                    context.CurrentPlayStage = context.PreviousStages.Pop();
                    return true;
                case TurnStages.PlayerEliminated:
                case TurnStages.PlayerEliminatedEnd:

                    context.EliminatePlayer(context.CurrentPlayStage.Source.Target);

                    context.CurrentPlayStage = context.PreviousStages.Pop();

                    return true;
                default:

                    context.PreviousStages.Push(context.CurrentPlayStage);

                    context.CurrentPlayStage = new PlayingCardStageTracker()
                    {
                        Cards = sender,
                        Source = new TargetPlayer(player),
                        Targets = new List<TargetPlayer>(),
                        Stage = TurnStages.PlayerDied
                    };

                    context.CurrentPlayStage.ExpectingIputFrom.Player = context.AnyPlayer;
                    context.CurrentPlayStage.ExpectingIputFrom.Prompt = new Prompts.UserPrompt(Prompts.UserPromptType.CardsPlayerHand);
                    return false;

            }

        }
    }
}

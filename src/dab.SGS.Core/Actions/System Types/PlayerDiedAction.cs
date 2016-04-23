using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dab.SGS.Core.Actions
{
    public class PlayerDiedAction : Action
    {
        public PlayerDiedAction(string display) : base(display)
        {
        }

        public override bool Perform(SelectedCardsSender sender, Player player, GameContext context)
        {
            if (context.CurrentTurnStage != TurnStages.PlayerDied)
            {
                // We need to setup the stage for player died.

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
            else
            {

                return false;
            }
        }
    }
}

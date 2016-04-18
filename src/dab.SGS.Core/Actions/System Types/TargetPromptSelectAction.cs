using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dab.SGS.Core.Actions.System_Types
{
    public class TargetPromptSelectAction : Action
    {
        public TargetPromptSelectAction(string display) : base(display)
        {
        }

        public override bool Perform(SelectedCardsSender sender, Player player, GameContext context)
        {

            switch (context.CurrentTurnStage)
            {
                case TurnStages.Play:

                    return false;
                case TurnStages.Prompt:

                    return false;
                case TurnStages.PromptEnd:
                default:

                    return true;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dab.SGS.Core.Actions
{
    public class PeachAction : Action
    {
        public PeachAction() : base("Peach Action")
        {
        }

        public override bool Perform(SelectedCardsSender sender, Player player, GameContext context)
        {
            if (context.CurrentTurnStage == TurnStages.Play || context.CurrentTurnStage == TurnStages.PlayerDied)
            {
                context.CurrentPlayStage.Targets.Add(context.CurrentPlayStage.Source);
                return true;
            }

            return false;
        }
    }
}

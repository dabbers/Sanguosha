using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dab.SGS.Core.Actions
{
    public class ReduceHealthToTargetAction : Action
    {
        public ReduceHealthToTargetAction(int decHealthBy = 1) : base("Peach")
        {
            if (decHealthBy < 0) throw new ArgumentException("decHealthBy cannot be negative");

            this.decHealthBy = decHealthBy;
        }

        public override bool Perform(SelectedCardsSender sender, Player player, GameContext context)
        {
            var targets = context.CurrentPlayStage.Targets;

            player.CurrentHealth = player.CurrentHealth - this.decHealthBy;

            // Do we need to begin a player died event?
            if (player.CurrentHealth < 1)
            {
                new PlayerDiedAction().Perform(sender, player, context);
            }

            return true;
        }

        private int decHealthBy = 1;
    }
}

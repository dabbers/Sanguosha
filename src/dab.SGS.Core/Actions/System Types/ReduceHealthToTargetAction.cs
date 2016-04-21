using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dab.SGS.Core.Actions.System_Types
{
    public class ReduceHealthToTargetAction : Action
    {
        public ReduceHealthToTargetAction(int maxTargets = 1, int decHealthBy = 1) : base("Peach")
        {
            this.maxTargets = maxTargets;
            this.decHealthBy = decHealthBy;
        }

        public override bool Perform(SelectedCardsSender sender, Player player, GameContext context)
        {
            var targets = context.CurrentPlayStage.Targets;
            var maxTargets = Math.Min(targets.Count, this.maxTargets);

            for (var i = 0; i < maxTargets; i++)
            {
                targets[i].Target.CurrentHealth = Math.Min(targets[i].Target.CurrentHealth - this.decHealthBy, targets[i].Target.MaxHealth);
            }

            return true;
        }

        private int maxTargets = 1;
        private int decHealthBy = 1;
    }
}

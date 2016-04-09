using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dab.SGS.Core.Actions
{
    public class IncreaseHealthToTargetAction : Action
    {
        public IncreaseHealthToTargetAction(int maxTargets = 1, int incHealthBy = 1) : base("Peach")
        {
            this.maxTargets = maxTargets;
            this.incHealthBy = incHealthBy;
        }

        public override bool Perform(object sender, Player player, GameContext context)
        {
            var targets = context.CurrentPlayStage.Targets;
            var maxTargets = Math.Min(targets.Count, this.maxTargets);

            for (var i = 0; i < maxTargets; i++)
            {
                targets[i].Target.CurrentHealth = Math.Min(targets[i].Target.CurrentHealth + 1, targets[i].Target.MaxHealth);
            }

            return true;
        }

        private int maxTargets = 1;
        private int incHealthBy = 1;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dab.SGS.Core.Actions
{
    public class ResetAttackCounterAction : Action
    {
        public ResetAttackCounterAction() : base("Reset Attack Counter")
        {
        }

        public override bool Perform(object sender, Player player, GameContext context)
        {
            player.AttacksLeft = GameContext.DEFAULT_MAX_ATTACKS;

            return true;
        }
    }
}

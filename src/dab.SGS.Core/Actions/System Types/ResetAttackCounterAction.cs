using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dab.SGS.Core.Actions
{
    public class ResetAttackCounterAction : Action
    {
        public ResetAttackCounterAction() : 
            this(GameContext.DEFAULT_MAX_ATTACKS)
        {
        }

        public ResetAttackCounterAction(int max) : base("Reset Attack Counter")
        {
            this.maxAttacks = max;
        }

        public override bool Perform(SelectedCardsSender sender, Player player, GameContext context)
        {
            player.AttacksLeft = this.maxAttacks;

            return true;
        }

        private int maxAttacks = 0;
    }
}

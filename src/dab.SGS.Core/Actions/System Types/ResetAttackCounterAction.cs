using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dab.SGS.Core.Actions
{
    public class ResetPlayerCountersAction : Action
    {
        public ResetPlayerCountersAction() : 
            this(GameContext.DEFAULT_MAX_ATTACKS, GameContext.DEFAULT_MAX_NONDEATHWINES)
        {
        }

        public ResetPlayerCountersAction(int maxAttack, int maxWines) : base("Reset Attack Counter")
        {
            this.maxAttacks = maxAttack;
            this.maxWines = maxWines;
        }

        public override bool Perform(SelectedCardsSender sender, Player player, GameContext context)
        {
            player.AttacksLeft = this.maxAttacks;
            player.WinesLeft = this.maxWines;
            player.WineInEffect = false;


            return true;
        }

        private int maxAttacks = 0;
        private int maxWines = 0;
    }
}

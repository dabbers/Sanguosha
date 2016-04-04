using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dab.SGS.Core.PlayingCard;

namespace dab.SGS.Core.Actions
{
    public class EndTurnAction : Action
    {
        public EndTurnAction() : base("End Turn")
        {
        }

        public override bool Perform(object sender, Player player, GameContext context)
        {
            return true;
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dab.SGS.Core.PlayingCards;

namespace dab.SGS.Core.Actions
{
    public class ReduceHandsizeDiscardAction : DiscardAction
    {
        public ReduceHandsizeDiscardAction(SelectCard select)
            : base(0, select)
        {

        }

        public override bool Perform(object sender, Player player, GameContext context)
        {
            this.numberCards = player.Hand.Count - player.CurrentHealth;

            if (this.NumberOfCards < 0) return true;

            return base.Perform(sender, player, context);
        }
    }
}

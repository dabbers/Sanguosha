﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dab.SGS.Core.PlayingCards;

namespace dab.SGS.Core.Actions
{
    public class ReduceHandsizeDiscardAction : DiscardAction
    {
        public ReduceHandsizeDiscardAction()
            : base(0)
        {

        }

        public override bool Perform(SelectedCardsSender sender, Player player, GameContext context)
        {
            this.numberCards = player.Hand.Count - player.MaxHandSize;

            if (this.NumberOfCards < 0) return true;

            return base.Perform(sender, player, context);
        }
    }
}

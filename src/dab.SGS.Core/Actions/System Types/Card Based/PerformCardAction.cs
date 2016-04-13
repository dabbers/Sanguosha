using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dab.SGS.Core.Actions
{
    public delegate PlayingCards.PlayingCard GetPerformCard();
    public class PerformCardAction : Action
    {
        public GetPerformCard CardChoice { get; private set; }
        public PerformCardAction(GetPerformCard card) : base("Perform a card, action")
        {
            this.CardChoice = card;
        }


        public override bool Perform(SelectedCardsSender sender, Player player, GameContext context)
        {
            // Pass the sender
            return this.CardChoice().Play(sender);
        }


    }
}

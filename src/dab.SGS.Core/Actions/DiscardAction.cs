using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dab.SGS.Core.Actions
{
    public class DiscardAction : CardChangeAction
    {
        public DiscardAction(int numberCards, SelectCard select) : base("Discard", numberCards)
        {
            if (numberCards > 0) throw new Exception("Invalid number of cards, must be negative!");
            
            this.select = select;
        }

        public override bool Perform(object sender, Player player, GameContext context)
        {
            for(var i = 0; i < this.NumberOfCards;i++)
            {
                var card = select(player);
                context.Deck.Discard.Add(card);
            }

            return true;
        }
        
        private SelectCard select;
    }
}

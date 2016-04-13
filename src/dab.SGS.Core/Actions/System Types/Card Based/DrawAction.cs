using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dab.SGS.Core.Actions
{
    public class DrawAction : CardChangeAction
    {
        public DrawAction(int numberCards) : base("Draw", numberCards)
        {
            if (numberCards < 0) throw new Exception("Invalid number of cards, must be positive!");
        }

        public override bool Perform(SelectedCardsSender sender, Player player, GameContext context)
        {
            for (var i = 0; i < this.NumberOfCards; i++)
            {
                var card = context.Deck.Draw();
                card.Owner = player;
                player.Hand.Add(card);
            }

            return true;
        }
    }
}

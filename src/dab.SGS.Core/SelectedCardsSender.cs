using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dab.SGS.Core
{
    public class SelectedCardsSender
    {
        public List<PlayingCard.PlayingCard> Cards { get; private set; }
        public PlayingCard.PlayingCard Activator { get; set; }

        public SelectedCardsSender(List<PlayingCard.PlayingCard> cards, PlayingCard.PlayingCard sender)
        {
            this.Cards = cards;
            this.Activator = sender;
        }
    }
}

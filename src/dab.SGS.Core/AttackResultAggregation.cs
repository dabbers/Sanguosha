using dab.SGS.Core.PlayingCards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dab.SGS.Core
{

    public class SelectedCardsSender : List<PlayingCard>
    {
        //public List<PlayingCards.PlayingCard> Cards { get; private set; }
        public PlayingCards.PlayingCard Activator { get; set; }
        
        public SelectedCardsSender()
        {
        }

        public SelectedCardsSender(IEnumerable<PlayingCard> collection, PlayingCards.PlayingCard sender) : base(collection)
        {
            this.Activator = sender;
        }

        public SelectedCardsSender(int capacity) : base(capacity)
        {
        }

        public int WinesPlayed()
        {
            return this.countNumOfCardsType(typeof(PlayingCards.Basics.WineBasicPlayingCard));
        }


        public int AttacksPlayed()
        {
            return this.countNumOfCardsType(typeof(PlayingCards.Basics.AttackBasicPlayingCard));
        }

        private int countNumOfCardsType(Type type)
        {
            int wineCount = 0;

            foreach (var card in this)
            {
                if (card.IsPlayedAsType(type))
                    wineCount++;
            }

            return wineCount;
        }
    }
}

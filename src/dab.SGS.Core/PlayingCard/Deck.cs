using dab.SGS.Core.Actions;
using dab.SGS.Core.PlayingCard;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dab.SGS.Core.PlayingCard
{
    public class Deck
    {
        public List<PlayingCard> AllCards { get; private set; }

        public List<PlayingCard> Discard { get; private set; }

        public List<PlayingCard> DrawPile { get; private set; }

        public Deck(List<PlayingCard> deck)
        {
            this.AllCards = deck;
            this.Discard = new List<PlayingCard>(deck);
#if DEBUG
            this.DrawPile = new List<PlayingCard>(deck);
            this.Discard.Clear();
#else
            this.shuffle();
#endif
        }

        /// <summary>
        /// Don't shuffle if the draw pile isn't empty
        /// </summary>
        private void shuffle()
        {
            if ((this.DrawPile?.Count ?? 0) > 0) return;

            this.DrawPile = this.Discard;
            this.DrawPile.Shuffle(new Random());

            this.Discard = new List<PlayingCard>();
        }
        public PlayingCard Draw()
        {
            if (this.DrawPile.Count() == 0)
            {
                this.shuffle();
            }

            var card = this.DrawPile.First();
            this.DrawPile.RemoveAt(0);

            return card;
        }

        public static List<PlayingCard> LoadCards(string json, 
            SelectCard selectCard, IsValidCard validCard)
        {
            dynamic cards = JsonConvert.DeserializeObject(json);
            var res = new List<PlayingCard>();

            foreach(var card in cards)
            {
                res.Add(PlayingCard.GetCardFromJson(card, selectCard, validCard));
            }

            return res;            
        }
    }

}

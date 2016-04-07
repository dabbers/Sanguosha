using dab.SGS.Core.Actions;
using dab.SGS.Core.PlayingCards;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dab.SGS.Core.PlayingCards
{
    public class Deck
    {
        public List<PlayingCard> AllCards { get; private set; }

        public List<PlayingCard> DiscardPile { get; private set; }

        public List<PlayingCard> DrawPile { get; private set; }

        public Deck(List<PlayingCard> deck)
        {
            this.AllCards = deck;
            this.DiscardPile = new List<PlayingCard>(deck);
#if DEBUG
            this.DrawPile = new List<PlayingCard>(deck);
            this.DiscardPile.Clear();
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

            this.DrawPile = this.DiscardPile;
            this.DrawPile.Shuffle(new Random());

            this.DiscardPile = new List<PlayingCard>();
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

        /// <summary>
        /// Adds this card to the discard pile. It also removes owner, and other temporary (current life span) attributes.
        /// </summary>
        /// <param name="card"></param>
        public void Discard(PlayingCard card)
        {
            card.Owner = null;
            card.BeingUsedAs = null;
            this.DiscardPile.Add(card);
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

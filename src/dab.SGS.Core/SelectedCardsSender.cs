﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dab.SGS.Core
{
    public class SelectedCardsSender
    {
        public List<PlayingCards.PlayingCard> Cards { get; private set; }
        public PlayingCards.PlayingCard Activator { get; set; }

        public SelectedCardsSender(List<PlayingCards.PlayingCard> cards, PlayingCards.PlayingCard sender)
        {
            this.Cards = cards;
            this.Activator = sender;
        }
    }
}

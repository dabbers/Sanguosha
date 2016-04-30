using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dab.SGS.Core.Exceptions
{
    public class NoJudgementException : SgsException
    {
        public PlayingCards.PlayingCard Card { get; private set; }

        public NoJudgementException(PlayingCards.PlayingCard card)
            : base(String.Format("{0} cannot be played as a judgement", card.ToString()))
        {
            this.Card = card;
        }
    }
}

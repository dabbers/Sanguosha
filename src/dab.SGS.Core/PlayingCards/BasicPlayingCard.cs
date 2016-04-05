using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dab.SGS.Core.PlayingCards
{
    public abstract class BasicPlayingCard : PlayingCard
    {
        public BasicPlayingCard()
            : base(PlayingCardColor.None)
        {
        }
        public BasicPlayingCard(PlayingCardColor color, PlayingCardSuite suite, string display,
            string details, List<Actions.Action> actions, Type[] responses)
            : base(color, suite, display, details, actions, responses)
        {
        }
    }
}

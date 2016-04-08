using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dab.SGS.Core.PlayingCards.Scrolls
{
    public abstract class DelayedScrollPlayingCard : ScrollPlayingCard
    {
        public DelayedScrollPlayingCard(PlayingCardColor color, PlayingCardSuite suite, string display,
            string details, List<Actions.Action> actions) : base(color, suite, display, details, actions)
        {
        }
    }
}

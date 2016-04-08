using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dab.SGS.Core.PlayingCards.Scrolls
{
    public class StarvationDelayedScrollPlayingCard : DelayedScrollPlayingCard
    {
        public StarvationDelayedScrollPlayingCard(PlayingCardColor color, PlayingCardSuite suite, string details) : base(color, suite, "Starvation", details)
        {
        }
    }
}

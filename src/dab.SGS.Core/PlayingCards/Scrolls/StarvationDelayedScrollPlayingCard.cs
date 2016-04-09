using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dab.SGS.Core.PlayingCards.Scrolls
{
    public class StarvationDelayedScrollPlayingCard : DelayedScrollPlayingCard
    {
        public StarvationDelayedScrollPlayingCard(PlayingCardColor color, PlayingCardSuite suite, string details) : base(color, suite, "Starvation", details, new List<Core.Actions.Action>())
        {
        }
        public StarvationDelayedScrollPlayingCard(PlayingCardColor color, PlayingCardSuite suite, string details, List<Actions.Action> actions) : base(color, suite, "Starvation", details, actions)
        {
        }
    }
}

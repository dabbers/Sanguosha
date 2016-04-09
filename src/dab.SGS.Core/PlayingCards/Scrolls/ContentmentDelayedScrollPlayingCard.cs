using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dab.SGS.Core.PlayingCards.Scrolls
{
    public class ContentmentDelayedScrollPlayingCard : DelayedScrollPlayingCard
    {
        public ContentmentDelayedScrollPlayingCard(PlayingCardColor color, PlayingCardSuite suite, string details) 
            : base(color, suite, "Contentment", details, new List<Core.Actions.Action>())
        {
        }
        public ContentmentDelayedScrollPlayingCard(PlayingCardColor color, PlayingCardSuite suite, string details, List<Core.Actions.Action> actions)
            : base(color, suite, "Contentment", details, actions)
        {
        }
    }
}

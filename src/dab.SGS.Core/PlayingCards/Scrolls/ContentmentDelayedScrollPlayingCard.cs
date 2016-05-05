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
            : base(color, suite, "Contentment", details, 
                  new List<Core.Actions.Action>() {
                      new Actions.ScrollAction(1, 1, 1, Core.Actions.ScrollAction.AnyTarget),
                      new Actions.DelayedScrollAction("Contentment Delayed Scroll", new Actions.ContentmentJudgementAction())
                  })
        {
        }
        public ContentmentDelayedScrollPlayingCard(PlayingCardColor color, PlayingCardSuite suite, string details, List<Core.Actions.Action> actions)
            : base(color, suite, "Contentment", details, actions)
        {
        }
        public override bool IsPlayable()
        {
            return this.Context.CurrentTurnStage == TurnStages.Play;
        }

        public override bool PlayJudgement(PlayingCard card)
        {
            return this.playAction(new SelectedCardsSender() { card }, this.Actions);
        }
    }
}

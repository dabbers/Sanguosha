using dab.SGS.Core.Actions;
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
        public override bool IsPlayable()
        {
            return this.Context.CurrentTurnStage == TurnStages.Play;
        }
        public new static PlayingCard GetCardFromJson(dynamic obj,
            SelectCard selectCard, IsValidCard validCard)
        {
            var color = (PlayingCardColor)Enum.Parse(typeof(PlayingCardColor), obj.PlayingCardColor.ToString());
            var suite = (PlayingCardSuite)Enum.Parse(typeof(PlayingCardSuite), obj.PlayingCardSuite.ToString());
            var details = obj.Details.ToString();

            var actions = Core.Actions.Action.ActionsFromJson(obj.Actions,
                selectCard, validCard);

            return new StarvationDelayedScrollPlayingCard(color, suite, details, actions);
        }
    }
}

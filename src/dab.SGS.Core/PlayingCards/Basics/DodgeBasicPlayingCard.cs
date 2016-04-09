using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dab.SGS.Core.Actions;

namespace dab.SGS.Core.PlayingCards.Basics
{
    public class DodgeBasicPlayingCard : BasicPlayingCard
    {
        public DodgeBasicPlayingCard(PlayingCardColor color, PlayingCardSuite suite, string details)
            : base(color, suite, "Dodge", details, new List<Core.Actions.Action>() { new DodgeAction() })
        {
        }
        public DodgeBasicPlayingCard(PlayingCardColor color, PlayingCardSuite suite, string details, List<Actions.Action> actions)
            : base(color, suite, "Dodge", details, actions)
        {
        }
        public new static PlayingCard GetCardFromJson(dynamic obj,
            SelectCard selectCard, IsValidCard validCard)
        {
            var color = (PlayingCardColor)Enum.Parse(typeof(PlayingCardColor), obj.PlayingCardColor.ToString());
            var suite = (PlayingCardSuite)Enum.Parse(typeof(PlayingCardSuite), obj.PlayingCardSuite.ToString());
            var details = obj.Details.ToString();

            var actions = Core.Actions.Action.ActionsFromJson(obj.Actions,
                selectCard, validCard);

            return new DodgeBasicPlayingCard(color, suite, details, actions);
        }

        public override bool IsPlayable()
        {
            return (this.Context.CurrentPlayStage.Stage == TurnStages.AttackCardResponse || this.Context.CurrentPlayStage.Stage == TurnStages.Discard);
        }
    }
}

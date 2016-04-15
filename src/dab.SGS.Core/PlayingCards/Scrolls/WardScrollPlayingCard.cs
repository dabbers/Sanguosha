using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dab.SGS.Core.Actions;

namespace dab.SGS.Core.PlayingCards.Scrolls
{
    public class WardScrollPlayingCard : ScrollPlayingCard
    {
        public WardScrollPlayingCard(PlayingCardColor color, PlayingCardSuite suite, string details) 
            : base(color, suite, "Ward", details, new List<Actions.Action>() { new WardAction() })
        {
        }

        public WardScrollPlayingCard(PlayingCardColor color, PlayingCardSuite suite, string details,
            List<Actions.Action> actions) : base(color, suite, "Ward", details, actions)
        {
        }

        public override bool Play(SelectedCardsSender sender)
        {
            return base.Play(sender);
        }

        public override bool IsPlayable()
        {
            // can't use ward on delayed scrolls immediately. Only on prejudgement
            return (((this.Context.CurrentPlayStage.Stage == TurnStages.PlayScrollTargets) && (!(this.Context.CurrentPlayStage?.Cards.Activator.IsPlayedAsDelayScroll() ?? false)))
                || this.Context.CurrentPlayStage.Stage == TurnStages.PreJudgement);
        }

        public new static PlayingCard GetCardFromJson(dynamic obj,
            SelectCard selectCard, IsValidCard validCard)
        {
            var color = (PlayingCardColor)Enum.Parse(typeof(PlayingCardColor), obj.PlayingCardColor.ToString());
            var suite = (PlayingCardSuite)Enum.Parse(typeof(PlayingCardSuite), obj.PlayingCardSuite.ToString());
            var details = obj.Details.ToString();

            var actions = Core.Actions.Action.ActionsFromJson(obj.Actions,
                selectCard, validCard);

            return new WardScrollPlayingCard(color, suite, details, actions);
        }
    }
}

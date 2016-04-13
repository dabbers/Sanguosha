using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dab.SGS.Core.Actions;

namespace dab.SGS.Core.PlayingCards.Scrolls
{
    public class DuelScrollPlayingCard : ScrollPlayingCard
    {
        public DuelScrollPlayingCard(PlayingCardColor color, PlayingCardSuite suite, string display, string details, List<Actions.Action> actions)
            : base(color, suite, display, details, actions)
        {
        }
        public DuelScrollPlayingCard(PlayingCardColor color, PlayingCardSuite suite, string display, string details)
            : base(color, suite, display, details, new List<Core.Actions.Action>() { new DuelAction("Duel Action") })
        {
        }

        public override bool Play(SelectedCardsSender sender)
        {
            return base.Play(sender);
        }

        public override bool IsPlayable()
        {
            // Can only play Duels on 
            return this.Context.CurrentPlayStage.Stage == TurnStages.Play;
        }
        public new static PlayingCard GetCardFromJson(dynamic obj,
            SelectCard selectCard, IsValidCard validCard)
        {
            var color = (PlayingCardColor)Enum.Parse(typeof(PlayingCardColor), obj.PlayingCardColor.ToString());
            var suite = (PlayingCardSuite)Enum.Parse(typeof(PlayingCardSuite), obj.PlayingCardSuite.ToString());
            var details = obj.Details.ToString();
            var display = obj.Display.ToString();

            var actions = Core.Actions.Action.ActionsFromJson(obj.Actions,
                selectCard, validCard);

            return new DuelScrollPlayingCard(color, suite, display, details, actions);
        }
    }
}

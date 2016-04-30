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
        public DuelScrollPlayingCard(PlayingCardColor color, PlayingCardSuite suite, string details, List<Actions.Action> actions)
            : base(color, suite, "Duel", details, actions)
        {
        }

        public DuelScrollPlayingCard(PlayingCardColor color, PlayingCardSuite suite, string details)
            : base(color, suite, "Duel", details, new List<Core.Actions.Action>() { new ScrollAction(1, 1, 1, 999), new DuelAction("Duel Action") })
        {
        }

        public override bool Play(SelectedCardsSender sender)
        {
            return base.Play(sender);
        }

        public override bool IsPlayable()
        {
            // Can only play Duels on 
            return this.Context.CurrentPlayStage.Stage == TurnStages.Play || 
                (this.Context.CurrentPlayStage.Stage == TurnStages.PlayScrollTargets && this.Context.CurrentPlayStage.ExpectingIputFrom.Prompt.Type.HasFlag(Prompts.UserPromptType.TargetRangeMN)) ||
                (this.Context.CurrentPlayStage.Stage >= TurnStages.PlayScrollPlaced && this.Context.CurrentTurnStage <= TurnStages.PlayScrollEnd 
                    && this.Context.CurrentTurnStage != TurnStages.PlayScrollPlaceResponse);
        }

        public new static PlayingCard GetCardFromJson(dynamic obj,
            SelectCard selectCard, IsValidCard validCard)
        {
            var color = (PlayingCardColor)Enum.Parse(typeof(PlayingCardColor), obj.PlayingCardColor.ToString());
            var suite = (PlayingCardSuite)Enum.Parse(typeof(PlayingCardSuite), obj.PlayingCardSuite.ToString());
            var details = obj.Details.ToString();

            var actions = Core.Actions.Action.ActionsFromJson(obj.Actions,
                selectCard, validCard);

            return new DuelScrollPlayingCard(color, suite, details, actions);
        }
    }
}

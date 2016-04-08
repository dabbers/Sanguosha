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

        public override bool IsPlayable()
        {
            // can't use ward on delayed scrolls immediately. Only on prejudgement
            return (((this.Context.TurnStage == TurnStages.PlayScrollPlace) && (!(this.Context.PlayStageTracker?.Cards.Activator.IsPlayedAsDelayScroll() ?? true)))
                || this.Context.TurnStage == TurnStages.PreJudgement);
        }
    }
}

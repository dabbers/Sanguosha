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

        public override bool Play(object sender)
        {
            return base.Play(sender);
        }

        public override bool IsPlayable()
        {
            // Can only play Duels on 
            return this.Context.TurnStage == TurnStages.Play;
        }
    }
}

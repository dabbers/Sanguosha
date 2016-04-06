using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dab.SGS.Core.Actions;

namespace dab.SGS.Core.PlayingCards.Basics
{
    public class PeachBasicPlayingCard : BasicPlayingCard
    {
        public PeachBasicPlayingCard(PlayingCardColor color, PlayingCardSuite suite, string display, string details, 
            List<Actions.Action> actions) : base(color, suite, display, details, actions)
        {
        }
        public override bool Play(object sender)
        {
            return base.Play(sender);
        }

        public override bool IsPlayable(GameContext ctx)
        {
            if (ctx.TurnStage == TurnStages.PlayerDied || this.Owner.CurrentHealth < this.Owner.MaxHealth)
            {
                return true;
            }

            return false;
        }
    }
}

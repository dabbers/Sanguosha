using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dab.SGS.Core.Actions;

namespace dab.SGS.Core.PlayingCards.Basics
{
    public class WineBasicPlayingCard : BasicPlayingCard
    {
        public WineBasicPlayingCard(PlayingCardColor color, PlayingCardSuite suite, string display, string details, 
            List<Actions.Action> actions) : base(color, suite, display, details, actions)
        {
        }

        /// <summary>
        /// This card is ONLY played when the player has died.
        /// </summary>
        /// <param name="sender"></param>
        /// <returns></returns>
        public override bool Play(object sender)
        {
            if (this.Owner.CurrentHealth < 1) return base.Play(sender);

            return false;
        }

        public override bool IsPlayable(GameContext ctx)
        {
            return (ctx.Turn == this.Owner && ctx.TurnStage == TurnStages.PlayerDied || ctx.TurnStage == TurnStages.Play);
        }
    }
}

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
        public PeachBasicPlayingCard(PlayingCardColor color, PlayingCardSuite suite, string details) 
            : base(color, suite, "Peach", details, new List<Core.Actions.Action>() { new PeachAction(), new IncreaseHealthToTargetAction(1, 1) })
        {
        }

        public PeachBasicPlayingCard(PlayingCardColor color, PlayingCardSuite suite, string details,
            List<Actions.Action> actions) : base(color, suite, "Peach", details, actions)
        {
        }

        public override bool Play(SelectedCardsSender sender)
        {
            return base.Play(sender);
        }

        public override bool IsPlayable()
        {
            // Any player has died, or the owner's health is less than our own.
            if (this.Context.CurrentPlayStage.Stage == TurnStages.PlayerDied || (this.Context.CurrentTurnStage == TurnStages.Play && this.Owner.CurrentHealth < this.Owner.MaxHealth))
            {
                return true;
            }

            return false;
        }
    }
}

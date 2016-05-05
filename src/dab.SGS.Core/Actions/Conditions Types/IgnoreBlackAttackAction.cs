using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dab.SGS.Core.PlayingCards;
using dab.SGS.Core.PlayingCards.Equipments;

namespace dab.SGS.Core.Actions
{
    /// <summary>
    /// Played as a shield action. If there is a card played as an attack && the card is black, returns false. 
    /// True otherwise.
    /// </summary>
    public class IgnoreBlackAttackAction : Action
    {
        public IgnoreBlackAttackAction(string display) : base(display)
        {
        }

        public override bool Perform(SelectedCardsSender sender, Player player, GameContext context)
        {
            throw new NotImplementedException("IgnoreBlackAttackAction should never be used as a default peform");
        }

        public override bool Perform(object sender, Player player, GameContext context, PlayingCardStageTracker result, WeaponEquipmentPlayingCard weapon)
        {
            if (result.Cards.AttacksPlayed() > 0)
            {
                if (result.Cards.Count(p => p.IsPlayedAsAttack() && p.Color == PlayingCardColor.Black) > 0) return false;
            }

            return true;
        }
    }
}

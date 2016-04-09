using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dab.SGS.Core.PlayingCards.Equipments
{
    public class ShieldEquipmentPlayingCard : EquipmentPlayingCard
    {
        public List<Actions.Action> PlaceActions { get; private set; }
        public List<Actions.Action> RemoveActions { get; private set; }
        
        public ShieldEquipmentPlayingCard(PlayingCardColor color, PlayingCardSuite suite, string display,
            string details, List<Actions.Action> placeActions, List<Actions.Action> defendActions, List<Actions.Action> removeActions,
            List<Actions.Action> damageCalculator) : base(color, suite, display, details, defendActions)
        {
            this.PlaceActions = placeActions;
            this.RemoveActions = removeActions;
            this.damageCalc = damageCalculator;
        }
        public override bool Play(object sender)
        {
            if (this.Owner.PlayerArea.Shield != null)
            {
                this.Owner.PlayerArea.Shield.RemoveAction(sender);
            }

            this.Context.Deck.Discard(this.Owner.PlayerArea.Shield);
            this.Owner.PlayerArea.Shield = this;
            this.Owner.Hand.Remove(this);

            return this.playAction(sender, this.PlaceActions);
        }

        public override bool IsPlayable()
        {
            return this.Context.CurrentTurnStage == TurnStages.Play;
        }

        public bool CanBeAttacked(PlayingCardStageTracker result, WeaponEquipmentPlayingCard weapon)
        {
            if (this.Actions == null) return true;

            foreach (var action in this.Actions)
            {
                if (!action.Perform(this, this.Owner, this.Context, result, weapon)) // Why does this return bool? What do I use this for?
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Gets the extra damage an attack would cause with this weapon and this shield.
        /// </summary>
        /// <param name="result"></param>
        /// <param name="weapon"></param>
        /// <returns></returns>
        public int GetExtraDamage(PlayingCardStageTracker result, WeaponEquipmentPlayingCard weapon)
        {
            int damage = 0;

            if (this.Actions == null) return damage;

            foreach (var action in this.damageCalc)
            {
                 damage += action.Perform(this, this.Owner, result, weapon);
            }

            return damage;
        }

        public bool RemoveAction(object sender)
        {
            return this.playAction(sender, this.RemoveActions);
        }

        private List<Actions.Action> damageCalc;
    }
}

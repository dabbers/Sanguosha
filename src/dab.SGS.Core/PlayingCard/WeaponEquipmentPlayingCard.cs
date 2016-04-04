using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dab.SGS.Core.PlayingCard
{


    public class WeaponEquipmentPlayingCard : EquipmentPlayingCard
    {
        public int Range { get { return this.range; } }
        public List<Actions.Action> PlaceActions { get; private set; }
        public List<Actions.Action> RemoveActions { get; private set; }

        public WeaponEquipmentPlayingCard(PlayingCardColor color) : base(color)
        {
        }

        public WeaponEquipmentPlayingCard(PlayingCardColor color, int range) : base(color)
        {
            this.range = range;
        }

        public WeaponEquipmentPlayingCard(int range, PlayingCardColor color, PlayingCardSuite suite, string display,
            string details, List<Actions.Action> placeActions, List<Actions.Action> attackActions, List<Actions.Action> removeActions)
            : base(color, suite, display, details, attackActions)
        {
            this.PlaceActions = placeActions;
            this.RemoveActions = removeActions;
        }

        public WeaponEquipmentPlayingCard(int range):
            this(PlayingCardColor.None)
        {
            this.range = range;
        }

        public override bool Play()
        {
            if (this.Owner.PlayerArea.Weapon != null)
            {
                this.Owner.PlayerArea.Weapon.RemoveAction();
            }

            this.Context.Deck.Discard.Add(this.Owner.PlayerArea.Weapon);
            this.Owner.PlayerArea.Weapon = this;
            this.Owner.Hand.Remove(this);

            return this.playAction(this.PlaceActions);
        }

        public void AttackOccured(Actions.AttackResult result)
        {
            if (this.Actions == null) return;

            foreach (var action in this.Actions)
            {
                action.Perform(this, this.Owner, this.Context, result); // Why does this return bool? What do I use this for?
            }

            return;
        }

        public bool RemoveAction()
        {
            return this.playAction(this.RemoveActions);
        }

        private int range = 1;
    }
}

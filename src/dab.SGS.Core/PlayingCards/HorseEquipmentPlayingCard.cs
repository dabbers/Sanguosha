using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dab.SGS.Core.PlayingCards
{
    public class HorseEquipmentPlayingCard : EquipmentPlayingCard
    {
        public int Distance { get { return this.distance; } }

        public HorseEquipmentPlayingCard(PlayingCardColor color) : base(color)
        {
        }

        public HorseEquipmentPlayingCard(PlayingCardColor color, PlayingCardSuite suite, string display, 
            string details, List<Actions.Action> actions) : base(color, suite, display, details, actions)
        {
        }

        public HorseEquipmentPlayingCard(int distance, PlayingCardColor color, PlayingCardSuite suite, 
            string display, string details, List<Actions.Action> actions) : base(color, suite, display, details, actions)
        {
            this.distance = distance;
        }


        private int distance = 0;

        public override bool Play(object sender)
        {
            if (this.Distance > 0 && this.Owner.PlayerArea.PlusHorse == null)
            {
                this.Owner.PlayerArea.PlusHorse = this;
                return true;
            }
            else if (this.Distance < 0 && this.Owner.PlayerArea.MinusHorse == null)
            {
                this.Owner.PlayerArea.MinusHorse = this;
                return true;
            }

            return false;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dab.SGS.Core.PlayingCard
{
    public abstract class EquipmentPlayingCard : PlayingCard
    {
        public EquipmentPlayingCard(PlayingCardColor color) : base(color)
        {
        }

        public EquipmentPlayingCard(PlayingCardColor color, PlayingCardSuite suite, string display, 
            string details, List<Actions.Action> actions) 
            : base(color, suite, display, details, actions)
        {
        }
    }
}

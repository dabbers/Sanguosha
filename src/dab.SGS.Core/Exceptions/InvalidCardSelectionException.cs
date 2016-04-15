using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dab.SGS.Core.Exceptions
{
    public class InvalidCardSelectionException : SgsException
    {
        public SelectedCardsSender Cards { get; protected set; }
        public TurnStages Stage { get; protected set; }
        public InvalidCardSelectionException(SelectedCardsSender cards, TurnStages stage) 
            : base(String.Format("The card(s) {0} cannot be played during this stage {1}", String.Join(", ", cards), stage.ToString()))
        {
            this.Cards = cards;
            this.Stage = stage;
        }
    }
}

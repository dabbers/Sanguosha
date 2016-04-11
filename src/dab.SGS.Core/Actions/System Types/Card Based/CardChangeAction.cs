using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dab.SGS.Core.Actions
{
    public abstract class CardChangeAction : Action
    {
        public int NumberOfCards { get { return this.numberCards; } }

        public CardChangeAction(string display, int numberCards) : base(display)
        {
            this.numberCards = numberCards;
        }

        protected int numberCards = 0;
    }
}

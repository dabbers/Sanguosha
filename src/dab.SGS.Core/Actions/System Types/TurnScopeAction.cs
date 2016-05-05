using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dab.SGS.Core.Actions
{
    public class TurnScopeAction : Action
    {
        public TurnScopeAction(string display, Action action) : base(display)
        {
            this.action = action;
        }

        public override bool Perform(SelectedCardsSender sender, Player player, GameContext context)
        {
            return this.action.Perform(sender, player, context);
        }

        private Action action = null;
    }
}

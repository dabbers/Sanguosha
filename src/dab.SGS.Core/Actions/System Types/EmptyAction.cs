using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dab.SGS.Core.Actions
{
    public class EmptyAction : Action
    {
        public EmptyAction(string display) : base(display)
        {
        }

        public override bool Perform(object sender, Player player, GameContext context)
        {
            return true;
        }
    }
}

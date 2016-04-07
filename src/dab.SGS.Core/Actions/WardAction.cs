using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dab.SGS.Core.Actions
{
    public class WardAction : Action
    {
        public WardAction() : base("Ward")
        {
        }

        public override bool Perform(object sender, Player player, GameContext context)
        {
            throw new NotImplementedException();
        }
    }
}

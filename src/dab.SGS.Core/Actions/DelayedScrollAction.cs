using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dab.SGS.Core.Actions
{
    public class DelayedScrollAction : Action
    {
        public DelayedScrollAction(string display) : base(display)
        {
        }

        public override bool Perform(object sender, Player player, GameContext context)
        {
            switch (context.TurnStage)
            {
                case TurnStages.Play:

                    return false;
                case TurnStages.PlayScrollTargets:

                    return false;
                case TurnStages.PlayScrollPlace:

                    return false;
            }

            return true;

        }
    }
}

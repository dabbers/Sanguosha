using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dab.SGS.Core.Actions
{
    public class ChainedActions : Action
    {
        public List<Action> Actions { get; set; }

        public ChainedActions(string display, List<Action> actions) : base(display)
        {
            this.Actions = actions;
        }

        public override bool Perform(SelectedCardsSender sender, Player player, GameContext context)
        {
#if !DEBUG
            try
#endif
            {
                //foreach (var action in this.Actions)
                for(var i = this.Actions.Count - 1; i >= 0; i--)
                {
                    if (!this.Actions[i].Perform(sender, player, context))
                    {
                        return false;
                    }
                }
            }
#if !DEBUG
            catch(InvalidOperationException)
            {
            }
#endif


            return true;
        }
    }
}

﻿using System;
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
            try
            {
                foreach (var action in this.Actions)
                {
                    if (!action.Perform(sender, player, context))
                    {
                        return false;
                    }
                }
            }
            catch(InvalidOperationException)
            {

            }


            return true;
        }
    }
}

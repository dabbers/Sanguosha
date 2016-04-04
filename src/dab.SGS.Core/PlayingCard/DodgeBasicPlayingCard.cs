﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dab.SGS.Core.Actions;

namespace dab.SGS.Core.PlayingCard
{
    public class DodgeBasicPlayingCard : BasicPlayingCard
    {
        public DodgeBasicPlayingCard()
        {
        }

        public DodgeBasicPlayingCard(PlayingCardColor color, PlayingCardSuite suite, 
            string display, string details, List<Actions.Action> actions)
            : base(color, suite, display, details, actions, null)
        {
        }


    }
}

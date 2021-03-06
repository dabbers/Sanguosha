﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dab.SGS.Core.PlayingCards.Scrolls
{
    public class LightningDelayedScrollPlayingCard : DelayedScrollPlayingCard
    {
        public LightningDelayedScrollPlayingCard(PlayingCardColor color, PlayingCardSuite suite, string details) : base(color, suite, "Lightning", details, new List<Core.Actions.Action>())
        {
        }
        public LightningDelayedScrollPlayingCard(PlayingCardColor color, PlayingCardSuite suite, string details, List<Core.Actions.Action> actions) : base(color, suite, "Lightning", details, actions)
        {
        }

        public override bool IsPlayable()
        {
            return this.Context.CurrentTurnStage == TurnStages.Play;
        }
    }
}

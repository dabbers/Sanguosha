﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dab.SGS.Core.Actions
{
    public class IncreaseHealthToTargetAction : Action
    {
        public IncreaseHealthToTargetAction(int maxTargets = 1, int incHealthBy = 1) : base("Increase Health to Target")
        {
            this.maxTargets = maxTargets;
            this.incHealthBy = incHealthBy;
        }

        public override bool Perform(SelectedCardsSender sender, Player player, GameContext context)
        {
            var targets = context.CurrentPlayStage.Targets;

            if (targets.Count == 0) targets.Add(context.CurrentPlayStage.Source);

            var maxTargets = Math.Min(targets.Count, this.maxTargets);

            for (var i = 0; i < maxTargets; i++)
            {
                targets[i].Target.CurrentHealth = Math.Min(targets[i].Target.CurrentHealth + this.incHealthBy, targets[i].Target.MaxHealth);
            }

            if (context.CurrentTurnStage == TurnStages.PlayerDied && targets.First().Target.CurrentHealth > 0)
            {
                context.CurrentTurnStage = TurnStages.PlayerRevived; 
            }

            return true;
        }

        public static new Action ActionFromJson(dynamic obj,
    SelectCard selectCard, IsValidCard validCard)
        {
            int maxTargets = obj.MaxTargets;
            int incHealthBy = obj.IncHealthBy;

            return new IncreaseHealthToTargetAction(maxTargets, incHealthBy);
        }

        private int maxTargets = 1;
        private int incHealthBy = 1;
    }
}

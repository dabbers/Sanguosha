using dab.SGS.Core.PlayingCards.Scrolls;
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
            switch(context.CurrentTurnStage)
            {
                case TurnStages.PlayScrollPlace:
                    {
                        // Can't ward delay scrolls until their judgment phase
                        if (context.CurrentPlayStage.Cards.Activator.IsPlayedAsDelayScroll()) return false;

                        var results = (SelectedCardsSender)sender;

                        context.CurrentPlayStage.ExpectingIputFrom.Result = TargetResult.Warded;

                        context.CurrentPlayStage.ExpectingIputFrom = context.AnyPlayer;

                        return true;
                    }
                case TurnStages.PreJudgement:
                    {
                        context.CurrentPlayStage.ExpectingIputFrom.Result = TargetResult.Warded;
                        context.CurrentPlayStage.ExpectingIputFrom = context.AnyPlayer;
                        var results = (SelectedCardsSender)sender;

                        //((DelayedScrollPlayingCard)context.CurrentPlayStage.Cards.Activator).Warded(player, results);
                        return true;
                    }
            }

            return false;
        }
        public static new Action ActionFromJson(dynamic obj,
            SelectCard selectCard, IsValidCard validCard)
        {
            return new WardAction();
        }
    }
}

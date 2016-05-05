using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dab.SGS.Core.Actions
{
    public class ContentmentJudgementAction : Action
    {
        public ContentmentJudgementAction(string display) : base(display)
        {
        }

        public ContentmentJudgementAction() : base("Contetnment Judgement Action")
        {
        }

        public override bool Perform(SelectedCardsSender sender, Player player, GameContext context)
        {
            var card = sender.Activator;

            // If true, the judgemenet happens, if false, it does not. 
            // If not hearts, player loses their action phase
            var missesTurn = card.Suite != PlayingCards.PlayingCardSuite.Heart;



            return missesTurn;
        }
    }
}

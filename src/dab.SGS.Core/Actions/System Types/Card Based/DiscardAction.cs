using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dab.SGS.Core.Actions
{
    public class DiscardAction : CardChangeAction
    {
        public DiscardAction(int numberCards) : base("Discard", numberCards)
        {
            if (numberCards < 0) throw new Exception("Invalid number of cards, must be positive!");
        }

        public override bool Perform(object sender, Player player, GameContext context)
        {
            if (context.CurrentPlayStage.ExpectingIputFrom == null)
            {
                context.CurrentPlayStage.ExpectingIputFrom = context.CurrentPlayStage.Source;
                context.CurrentPlayStage.PeristedEnumerator = new int[this.NumberOfCards].GetEnumerator();
            }
            else
            {
                // How attacking a player works:
                var results = (SelectedCardsSender)sender;

                var enumer = results.GetEnumerator();

                // If multiple cards were selected, only discard up until we either run out of select cards,
                // we run out of required cards to discard, or both
                while (enumer.MoveNext() && context.CurrentPlayStage.PeristedEnumerator.MoveNext())
                {
                    enumer.Current.Discard();
                }

                if (context.CurrentPlayStage.PeristedEnumerator.MoveNext()) return true;

                context.CurrentPlayStage.ExpectingIputFrom = null;
            }

            return true;
        }
        
        private SelectCard select;
    }
}

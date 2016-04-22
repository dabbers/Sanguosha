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

        public override bool Perform(SelectedCardsSender sender, Player player, GameContext context)
        {
            // Player hasn't had a chance to select a card yet. We will tell the game context 
            // that we are now expecting input from a player.
            if (context.CurrentPlayStage.ExpectingIputFrom.Player == null)
            {
                var t = (new object[this.NumberOfCards]).ToList().GetEnumerator();

                context.CurrentPlayStage.ExpectingIputFrom.Player = context.CurrentPlayStage.Source;
                context.CurrentPlayStage.ExpectingIputFrom.Prompt = new Prompts.UserPrompt(Prompts.UserPromptType.CardsPlayerHand);
                context.CurrentPlayStage.PeristedEnumerator = new PeekEnumerator<object>(t);
                return false;
            }
            else
            {
                // How attacking a player works:
                var results = sender;

                var enumer = results.GetEnumerator();

                // If multiple cards were selected, only discard up until we either run out of select cards,
                // we run out of required cards to discard, or both
                while (enumer.MoveNext() && context.CurrentPlayStage.PeristedEnumerator.MoveNext())
                {
                    enumer.Current.Discard();
                }

                if (context.CurrentPlayStage.PeristedEnumerator.CanMoveNext()) return false;

                context.CurrentPlayStage.ExpectingIputFrom.Player = null;
            }

            return true;
        }
    }
}

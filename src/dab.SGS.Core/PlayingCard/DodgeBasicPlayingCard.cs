using System;
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
        public new static PlayingCard GetCardFromJson(dynamic obj,
            SelectCard selectCard, IsValidCard validCard)
        {
            var color = (PlayingCardColor)Enum.Parse(typeof(PlayingCardColor), obj.PlayingCardColor.ToString());
            var suite = (PlayingCardSuite)Enum.Parse(typeof(PlayingCardSuite), obj.PlayingCardSuite.ToString());
            var display = obj.Display.ToString();
            var details = obj.Details.ToString();

            var actions = Core.Actions.Action.ActionsFromJson(obj.Actions,
                selectCard, validCard);

            return new DodgeBasicPlayingCard(color, suite, display, details, actions);
        }
    }
}

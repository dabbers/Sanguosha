using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dab.SGS.Core.Actions;

namespace dab.SGS.Core.PlayingCards.Basics
{
    public enum Elemental
    {
        None,
        Fire,
        Lightning
    }

    public class AttackBasicPlayingCard : BasicPlayingCard
    {
        public Elemental Element { get; private set; }
        public AttackBasicPlayingCard(PlayingCardColor color, PlayingCardSuite suite, string display, 
            string details, List<Actions.Action> actions, Elemental element) 
            : base(color, suite, display, details, actions)
        {
        }

        public new static PlayingCard GetCardFromJson(dynamic obj,
            SelectCard selectCard, IsValidCard validCard)
        {
            var color = (PlayingCardColor)Enum.Parse(typeof(PlayingCardColor), obj.PlayingCardColor.ToString());
            var suite = (PlayingCardSuite)Enum.Parse(typeof(PlayingCardSuite), obj.PlayingCardSuite.ToString());
            var element = (Elemental)Enum.Parse(typeof(Elemental), obj.Elemental.ToString());
            var display = obj.Display.ToString();
            var details = obj.Details.ToString();

            var actions = Core.Actions.Action.ActionsFromJson(obj.Actions,
                selectCard, validCard);

            return new AttackBasicPlayingCard(color, suite, display, details, actions, element);
        }

        public override bool IsPlayable(GameContext ctx)
        {

            return base.IsPlayable(ctx);
        }
    }
}

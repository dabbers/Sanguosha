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
        public AttackBasicPlayingCard(PlayingCardColor color, PlayingCardSuite suite,
            string details, Elemental element)
            : base(color, suite, (element != Elemental.None ? element.ToString() + " " : "") + "Attack", details, new List<Core.Actions.Action>() { new AttackAction("Attack", 1) })
        {
        }
        public AttackBasicPlayingCard(PlayingCardColor color, PlayingCardSuite suite, 
            string details, List<Actions.Action> actions, Elemental element) 
            : base(color, suite, (element != Elemental.None ? element.ToString() + " " : "") + "Attack", details, actions)
        {
        }

        public new static PlayingCard GetCardFromJson(dynamic obj,
            SelectCard selectCard, IsValidCard validCard)
        {
            var color = (PlayingCardColor)Enum.Parse(typeof(PlayingCardColor), obj.PlayingCardColor.ToString());
            var suite = (PlayingCardSuite)Enum.Parse(typeof(PlayingCardSuite), obj.PlayingCardSuite.ToString());
            var element = (Elemental)Enum.Parse(typeof(Elemental), obj.Elemental.ToString());
            var details = obj.Details.ToString();

            var actions = Core.Actions.Action.ActionsFromJson(obj.Actions,
                selectCard, validCard);

            return new AttackBasicPlayingCard(color, suite, details, actions, element);
        }

        public override bool IsPlayable(GameContext ctx)
        {
            return (ctx.TurnStage == TurnStages.Play || ctx.TurnStage == TurnStages.PlayScrollPlace);
        }
    }
}

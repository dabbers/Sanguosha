﻿using System;
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

        public override bool Play(SelectedCardsSender sender)
        {
            // Devnote: is there a scenario where we don't want this?
            //this.Context.CurrentPlayStage.Cards.Activator = this;

            return base.Play(sender);
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

        public override bool IsPlayable()
        {
            return ( (this.Context.CurrentTurnStage == TurnStages.Play && this.Owner.AttacksLeft > 0) ||
                (this.Context.CurrentTurnStage >= TurnStages.AttackChooseTargets && this.Context.CurrentTurnStage <= TurnStages.AttackEnd &&
                    this.Context.CurrentTurnStage != TurnStages.AttackCardResponse) ||
                (
                    this.Context.CurrentTurnStage == TurnStages.PlayScrollPlaceResponse &&

                    ( this.Context.CurrentPlayStage.Cards.Activator.IsPlayedAsDuel() || false ) // Todo: Add barabarians check here
                )
            );
        }

        public override string ToString()
        {

            return (this.Element != Elemental.None ? this.Element.ToString() + " " : "") + base.ToString();
        }
    }
}

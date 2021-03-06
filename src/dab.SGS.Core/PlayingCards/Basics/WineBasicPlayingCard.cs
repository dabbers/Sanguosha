﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dab.SGS.Core.Actions;

namespace dab.SGS.Core.PlayingCards.Basics
{
    public class WineBasicPlayingCard : BasicPlayingCard
    {
        public WineBasicPlayingCard(PlayingCardColor color, PlayingCardSuite suite, string details) 
            : base(color, suite, "Wine", details, new List<Core.Actions.Action>() { new IncreaseHealthToTargetAction(1, 1) })
        {
        }

        public WineBasicPlayingCard(PlayingCardColor color, PlayingCardSuite suite, string details, 
            List<Actions.Action> actions) : base(color, suite, "Wine", details, actions)
        {
        }

        /// <summary>
        /// This card is ONLY played when the player has died.
        /// </summary>
        /// <param name="sender"></param>
        /// <returns></returns>
        public override bool Play(SelectedCardsSender sender)
        {
            if (this.Context.CurrentPlayStage.Stage == TurnStages.Play && sender.Count(p => p.IsPlayedAsAttack()) == 0 && 
                !this.Owner.WineInEffect && this.Owner.WinesLeft > 0)
            {
                this.Owner.WinesLeft--;
                this.Owner.WineInEffect = true;
                this.Discard();
                return true;
            }

            return base.Play(sender);
        }

        public override bool IsPlayable()
        {
            return ((this.Context.CurrentPlayStage.Stage == TurnStages.PlayerDied && this.Context.CurrentPlayStage.Source.Target == this.Owner
                && this.Owner.CurrentHealth < 1)
                || this.Context.CurrentPlayStage.Stage == TurnStages.Play && !this.Owner.WineInEffect && this.Owner.WinesLeft > 0);
        }

        public new static PlayingCard GetCardFromJson(dynamic obj,
            SelectCard selectCard, IsValidCard validCard)
        {
            var color = (PlayingCardColor)Enum.Parse(typeof(PlayingCardColor), obj.PlayingCardColor.ToString());
            var suite = (PlayingCardSuite)Enum.Parse(typeof(PlayingCardSuite), obj.PlayingCardSuite.ToString());
            var details = obj.Details.ToString();

            var actions = Core.Actions.Action.ActionsFromJson(obj.Actions,
                selectCard, validCard);

            return new WineBasicPlayingCard(color, suite, details, actions);
        }
    }
}

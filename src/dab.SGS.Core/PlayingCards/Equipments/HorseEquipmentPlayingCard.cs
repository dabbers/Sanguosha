﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dab.SGS.Core.PlayingCards.Equipments
{
    public class HorseEquipmentPlayingCard : EquipmentPlayingCard
    {
        public int Distance { get { return this.distance; } }
        
        public HorseEquipmentPlayingCard(int distance, PlayingCardColor color, PlayingCardSuite suite, 
            string display, string details) : base(color, suite, display, details, null)
        {
            this.distance = distance;
        }
        
        private int distance = 1;

        public override bool Play(SelectedCardsSender sender)
        {
            if (this.Distance > 0)
            {
                if (this.Owner.PlayerArea.PlusHorse != null)
                {
                    this.Owner.PlayerArea.PlusHorse.Discard();
                }

                this.Owner.PlayerArea.PlusHorse = this;
                return true;
            }
            else if (this.Distance < 0)
            {
                if (this.Owner.PlayerArea.MinusHorse != null)
                {
                    this.Owner.PlayerArea.MinusHorse.Discard();
                }

                this.Owner.PlayerArea.MinusHorse = this;
                return true;
            }

            return false;
        }

        public override bool IsPlayable()
        {
            return this.Context.CurrentTurnStage == TurnStages.Play;
        }
    }
}

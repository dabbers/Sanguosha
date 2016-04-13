﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dab.SGS.Core.Prompts
{
    [Flags]
    public enum UserPromptType
    {
        // Standard turn stuff: 
        //

        /// <summary>
        /// Select a skill(s) to use
        /// </summary>
        Skills = 1 << 0,
        
        //
        // End of standard skill stuff

        /// <summary>
        /// Choose a card from the player's hand
        /// </summary>
        CardsPlayerHand = 1 << 1,

        /// <summary>
        /// Choose a card from the player's play area
        /// </summary>
        CardsPlayerPlayArea = 1 << 2,

        /// <summary>
        /// Choose a card from the taget's hand
        /// </summary>
        CardsTargetHand = 1 << 3,

        /// <summary>
        /// Choose a card from the target's play area
        /// </summary>
        CardsTargetPlayArea = 1 << 4,
        
        /// <summary>
        /// Choose any card in the game (not deck or discard)
        /// </summary>
        AllCards = 1 << 5,

        /// <summary>
        /// The cards in the "holding area"
        /// </summary>
        HoldingArea = 1 << 6,

        /// <summary>
        /// Choose a target with configurable range M to N (min max)
        /// </summary>
        TargetRangeMN = 1 << 7,

        /// <summary>
        /// Select from a list of options
        /// </summary>
        Options = 1 << 8,

        /// <summary>
        /// Yes or No Prompt
        /// </summary>
        YesNo = 1 << 9,

    }


    public class UserPrompt
    {
        public List<Player> Players { get; set; }
        public List<PlayingCards.PlayingCard> Cards { get; set; }
        public int MinTargets { get; set; }
        public int MaxTargets { get; set; }
        public int MinRange { get; set; }
        public int MaxRange { get; set; }
        public int MinCards { get; set; }
        public int MaxCards { get; set; }

        public UserPromptType Type { get; protected set; }
        

        public UserPrompt(UserPromptType type)
        {
            this.Type = type;
        }
    }
}

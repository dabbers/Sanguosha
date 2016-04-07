using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dab.SGS.Core.Prompts
{
    public enum UserPromptType
    {
        // Standard turn stuff: 
        //

        /// <summary>
        /// Select a skill(s) to use
        /// </summary>
        Skills,

        /// <summary>
        /// Select a skill or card to use
        /// </summary>
        SkillsOrCards,
        
        //
        // End of standard skill stuff

        /// <summary>
        /// Yes or No Prompt
        /// </summary>
        YesNo,

        /// <summary>
        /// Choose a card from the player's hand
        /// </summary>
        CardsPlayerHand,

        /// <summary>
        /// Choose a card from the player's play area
        /// </summary>
        CardsPlayerPlayArea,

        /// <summary>
        /// Choose any card of the player's
        /// </summary>
        CardsPlayerAll,

        /// <summary>
        /// Choose a card from the taget's hand
        /// </summary>
        CardsTargetHand,

        /// <summary>
        /// Choose a card from the target's play area
        /// </summary>
        CardsTargetPlayArea,

        /// <summary>
        /// Choose any card from the target
        /// </summary>
        CardsTargetAll,


        /// <summary>
        /// Choose any card in the game (not deck or discard)
        /// </summary>
        AllCards,
        
        /// <summary>
        /// The cards in the "holding area"
        /// </summary>
        HoldingArea,

        /// <summary>
        /// Choose a target within a range
        /// </summary>
        TargetAttackRange,

        /// <summary>
        /// Choose a target with configurable range n
        /// </summary>
        TargetRangeN,

        /// <summary>
        /// Choose a target with configurable range M to N (min max)
        /// </summary>
        TargetRangeMN,

        /// <summary>
        /// Choose any target
        /// </summary>
        TargettAny,

        /// <summary>
        /// Select from a list of options
        /// </summary>
        Options,

    }


    public abstract class UserPrompt
    {
        public UserPromptType Type { get; protected set; }
        public UserPrompt(UserPromptType type)
        {
            this.Type = type;
        }
    }
}

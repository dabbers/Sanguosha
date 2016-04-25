using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dab.SGS.Core
{

    public enum TurnStages
    {
        /// <summary>
        /// Start of a turn. Player has done nothing yet
        /// </summary>
        Start,
        /// <summary>
        /// Before a judgement is being drawn (can use ward/negate card here)
        /// </summary>
        PreJudgement,
        /// <summary>
        /// Perform the judgement
        /// </summary>
        Judgement,
        /// <summary>
        /// Before drawing
        /// </summary>
        PreDraw,
        /// <summary>
        /// The actual action for drawing
        /// </summary>
        Draw,
        /// <summary>
        /// After a draw has happened
        /// </summary>
        PostDraw,
        /// <summary>
        /// An action for the player
        /// </summary>
        Play,
        
        /// <summary>
        /// Before the discard phase
        /// </summary>
        PreDiscard,
        /// <summary>
        /// The actual action for discarding
        /// </summary>
        Discard,
        /// <summary>
        /// After discarding
        /// </summary>
        PostDiscard,
        /// <summary>
        /// End of a player's turn. Add all extra turn stages before here.
        /// There is special logic that determines wrap-around for End (among 
        /// other things).
        /// </summary>
        End,




        // DIFFERENT SECTIONS OF Turn Stages

        // DO NOT ADD ANY ACTIONS TO PlayScrollPreStage. IT WON'T EXECUTE. This is to keep logic consistent between stage jumps
        PlayScrollPreStage,
        /// <summary>
        /// Player played a scroll card, have them select a target
        /// </summary>
        PlayScrollTargets,
        /// <summary>
        /// A scroll was played.
        /// </summary>
        PlayScrollPlaced,
        /// <summary>
        /// Target(s) made a response
        /// </summary>
        PlayScrollPlaceResponse,
        /// <summary>
        /// The scroll play has ended
        /// </summary>
        PlayScrollEnd,



        // DO NOT ADD ANY ACTIONS TO PlayerDiedPreStage. IT WON'T EXECUTE. This is to keep logic consistent between stage jumps
        PlayerDiedPreStage,
        /// <summary>
        /// The notification that a player has died (players should select to peach/revive or not)
        /// </summary>
        PlayerDied,
        /// <summary>
        /// Player did not get enough peaches.
        /// </summary>
        PlayerEliminated,

        /// <summary>
        /// We play at the end.
        /// </summary>
        PlayerEliminatedEnd,

        /// <summary>
        /// Player was revived
        /// </summary>
        PlayerRevived,

        /// <summary>
        /// We step after revived to be end.
        /// </summary>
        PlayerRevivedEnd,

        // DO NOT ADD ANY ACTIONS TO PromptPreStage. IT WON'T EXECUTE. This is to keep logic consistent between stage jumps
        PromptPreStage,
        /// <summary>
        /// We are prompting a player currently.
        /// </summary>
        Prompt,
        /// <summary>
        /// A response to the prompt has occured
        /// </summary>
        PromptEnd,


        // ATTACK STAGES

        // DO NOT ADD ANY ACTIONS TO AttackPreStage. IT WON'T EXECUTE. This is to keep logic consistent between stage jumps
        AttackPreStage,
        // Do not add any Turn stages below here.
        /// <summary>
        /// Select the target(s) to attack. Some skills or weapons allow multiple targets
        /// </summary>
        AttackChooseTargets,

        /// <summary>
        /// The target's skill might have an anti-attack response
        /// </summary>
        AttackSkillResponse,

        /// <summary>
        /// Activate/validate any shield 
        /// </summary>
        AttackShieldResponse,

        /// <summary>
        /// Play a card in response to this attack
        /// </summary>
        AttackCardResponse,

        /// <summary>
        /// Damage is about to occur (possibly)
        /// </summary>
        AttackBeforeDamage,

        /// <summary>
        /// Damage has occured (target's health has been lowered)
        /// </summary>
        AttackDamage,

        /// <summary>
        /// The attack has finished
        /// </summary>
        AttackEnd,
    }
}

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
        /// Happens after a judgement. Good/bad luck.
        /// </summary>
        PostJudgement,
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

        /// <summary>
        /// Player played a scroll card, have them select a target
        /// </summary>
        PlayTargets,
        /// <summary>
        /// A scroll was played.
        /// </summary>
        PlayPlace,

        /// <summary>
        /// The notification that a player has died (players should select to peach/revive or not)
        /// </summary>
        PlayerDied,
        /// <summary>
        /// Player was revived
        /// </summary>
        PlayerRevived,
        /// <summary>
        /// Player did not get enough peaches.
        /// </summary>
        PlayerEliminated,


        // ATTACK STAGES
        // Do not add any Turn stages below here.
        /// <summary>
        /// Select the target(s) to attack. Some skills or weapons allow multiple targets
        /// </summary>
        ChooseTargets,

        /// <summary>
        /// The target's skill might have an anti-attack response
        /// </summary>
        SkillResponse,

        /// <summary>
        /// Activate/validate any shield 
        /// </summary>
        ShieldResponse,

        /// <summary>
        /// Play a card in response to this attack
        /// </summary>
        CardResponse,

        /// <summary>
        /// Damage is about to occur (possibly)
        /// </summary>
        BeforeDamage,

        /// <summary>
        /// Damage has occured (target's health has been lowered)
        /// </summary>
        Damage,

        /// <summary>
        /// The attack has finished
        /// </summary>
        AttackEnd,
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dab.SGS.Core
{

    public class PlayingCardStageTracker
    {
        public TurnStages Stage { get; set; }
        public List<TargetPlayer> Targets { get; set; }
        public TargetPlayerResponse ExpectingIputFrom { get; set; }
        public SelectedCardsSender Cards { get; set; }

        public TargetPlayer Source { get; set; }

        public PlayingCardStageTracker()
        {
            this.ExpectingIputFrom = new TargetPlayerResponse();
            this.Cards = new SelectedCardsSender();
            this.Targets = new List<TargetPlayer>();
        }

        /// <summary>
        /// Can be used for whatever purposes an action needs.
        /// </summary>
        public PeekEnumerator<object> PeristedEnumerator { get; set; }

        /// <summary>
        /// Used to track the enumeration of targets.
        /// </summary>
        public PeekEnumerator<TargetPlayer> PeristedTargetEnumerator { get; set; }
    }
}

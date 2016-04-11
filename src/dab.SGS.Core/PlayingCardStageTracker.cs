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
        public TargetPlayer ExpectingIputFrom { get; set; }
        public SelectedCardsSender Cards { get; set; }

        public TargetPlayer Source { get; set; }

        /// <summary>
        /// Can be used for whatever purposes an action needs.
        /// </summary>
        public IEnumerator PeristedEnumerator { get; set; }

        /// <summary>
        /// Used to track the enumeration of targets.
        /// </summary>
        public IEnumerator PeristedTargetEnumerator { get; set; }
    }
}

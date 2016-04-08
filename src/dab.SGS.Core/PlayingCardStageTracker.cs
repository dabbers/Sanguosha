using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dab.SGS.Core
{

    public class PlayingCardStageTracker
    {
        public Stack<TurnStages> PreviousStages { get; set; }
        public List<TargetPlayer> Targets { get; set; }
        public TargetPlayer ExpectingIputFrom { get; set; }
        public SelectedCardsSender Cards { get; set; }
        public TargetPlayer Source { get; set; }

    }
}

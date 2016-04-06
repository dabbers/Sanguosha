using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dab.SGS.Core.Prompts
{
    public class TargetUserPrompt : UserPrompt
    {
        public int MinTargets { get; protected set; }
        public int MaxTargets { get; protected set; }
        public int MinRange { get; protected set; }
        public int MaxRange { get; protected set; }

        public List<Player> Targets { get; set; }

        public TargetUserPrompt(UserPromptType type, int min, int max, int rangeMin, int rangeMax) : base(type)
        {
            this.MinTargets = min;
            this.MaxTargets = max;
            this.MinRange = rangeMin;
            this.MaxRange = rangeMax;
        }
    }
}

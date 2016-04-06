using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dab.SGS.Core.Prompts
{
    public class CardUserPrompt : UserPrompt
    {
        public int MinCards { get; protected set; }
        public int MaxCards { get; protected set; }

        public List<PlayingCards.PlayingCard> Cards { get; set; }

        public CardUserPrompt(UserPromptType type, int min, int max)
            :base(type)
        {
            this.MinCards = min;
            this.MaxCards = max;
        }
    }
}

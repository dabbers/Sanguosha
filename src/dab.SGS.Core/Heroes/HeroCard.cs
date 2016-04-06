using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dab.SGS.Core.Heroes
{
    public enum PlayableVerdict
    {
        Playable,
        NotPlayable,
        Unknown
    }

    public enum Kingdoms
    {
        Wei,
        Shu,
        Wu,
        Hero
    }



    
    public abstract class HeroCard
    {
        public string Display { get; private set; }
        public string Details { get; private set; }


        public abstract PlayableVerdict IsCardPlayable(GameContext ctx, PlayingCards.PlayingCard card);
    }
}

using dab.SGS.Core.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace dab.SGS.Core.PlayingCards
{
    public enum PlayingCardSuite
    {
        Club,
        Spade,
        Heart,
        Diamonds
    }
    public enum PlayingCardColor
    {
        Red,
        Black,
        None
    }

    public abstract class PlayingCard
    {
        public PlayingCardSuite Suite { get { return this.suite; } }
        public PlayingCardColor Color { get { return this.color; } }

        public string Display { get { return this.display; } }
        public string Details { get { return this.details; } }
        
        /// <summary>
        /// If this card isn't being used as what it is, what IS it being used as?
        /// </summary>
        public Type BeingUsedAs { get; set; }

        /// <summary>
        /// The current player who holds this card (either in their hand or on their playarea)
        /// </summary>
        public Player Owner { get; set; }

        /// <summary>
        /// The actions this card performs
        /// </summary>
        public List<Actions.Action> Actions { get; private set; }

        public GameContext Context { get; set; }
        
        public PlayingCard(PlayingCardColor color, PlayingCardSuite suite, string display,
            string details, List<Actions.Action> actions)
        {
            this.suite = suite;
            this.display = display;
            this.details = details;
            this.Actions = actions;
            this.color = color;
        }

        public virtual bool Play(object sender)
        {
            var res = this.playAction(sender, this.Actions);

            if (res)
            {
                this.Discard();
            }
            
            return res;
        }

        public virtual bool IsPlayable()
        {
            return true;
        }

        public virtual void Discard()
        {
            if (this.Owner.Hand.Contains(this))
                this.Owner.Hand.Remove(this);

            this.Context.Deck.Discard(this);
        }

        #region IsPlayedAsType methods
        public bool IsPlayedAsType(Type type)
        {
            return this.GetType() == type || this.BeingUsedAs == type;
        }

        public bool IsPlayedAsAttack()
        {
            return this.IsPlayedAsType(typeof(Basics.AttackBasicPlayingCard));
        }
        public bool IsPlayedAsDodge()
        {
            return this.IsPlayedAsType(typeof(Basics.DodgeBasicPlayingCard));
        }
        public bool IsPlayedAsPeach()
        {
            return this.IsPlayedAsType(typeof(Basics.PeachBasicPlayingCard));
        }
        public bool IsPlayedAsWine()
        {
            return this.IsPlayedAsType(typeof(Basics.WineBasicPlayingCard));
        }
        public bool IsPlayedAsWard()
        {
            return this.IsPlayedAsType(typeof(Scrolls.WardScrollPlayingCard));
        }
        public bool IsPlayedAsDuel()
        {
            return this.IsPlayedAsType(typeof(Scrolls.WardScrollPlayingCard));
        }
        public bool IsPlayedAsLightning()
        {
            return this.IsPlayedAsType(typeof(Scrolls.LightningDelayedScrollPlayingCard));
        }
        public bool IsPlayedAsContentment()
        {
            return this.IsPlayedAsType(typeof(Scrolls.ContentmentDelayedScrollPlayingCard));
        }
        public bool IsPlayedAsStarvation()
        {
            return this.IsPlayedAsType(typeof(Scrolls.StarvationDelayedScrollPlayingCard));
        }
        public bool IsPlayedAsDelayScroll()
        {
            return this.IsPlayedAsLightning() || this.IsPlayedAsStarvation() || this.IsPlayedAsContentment();
        }
        #endregion

        protected bool playAction(object sender, List<Actions.Action> actions)
        {

            if (actions == null) return false;

            foreach (var action in actions)
            {
                if (!action.Perform(sender, this.Owner, this.Context)) // Why does this return bool? What do I use this for?
                {
                    return false;
                }
            }
            return true;
        }

        public static PlayingCard GetCardFromJson(dynamic obj,
            SelectCard selectCard, IsValidCard validCard)
        {
            string cardType = obj.Type.ToString();
            var type = Type.GetType(String.Format("dab.SGS.Core.PlayingCards.{0}", cardType));
            var fnc = type.GetMethod("GetCardFromJson");

            return (PlayingCard)fnc.Invoke(null, new object[] { obj, selectCard, validCard });
        }
        

        private string display = String.Empty;
        private string details = String.Empty;
        private PlayingCardSuite suite;
        private PlayingCardColor color;

    }
}

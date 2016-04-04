using dab.SGS.Core.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace dab.SGS.Core.PlayingCard
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

        public Type[] Responses { get; protected set; }

        /// <summary>
        /// The current player who holds this card (either in their hand or on their playarea)
        /// </summary>
        public Player Owner { get; set; }

        /// <summary>
        /// The actions this card performs
        /// </summary>
        public List<Actions.Action> Actions { get; private set; }

        public GameContext Context { get; set; }

        public PlayingCard(PlayingCardColor color)
        {
            this.color = color;
        }

        public PlayingCard(PlayingCardColor color, PlayingCardSuite suite, string display,
            string details, List<Actions.Action> actions)
            : this(color)
        {
            this.suite = suite;
            this.display = display;
            this.details = details;
            this.Actions = actions;
        }
        public PlayingCard(PlayingCardColor color, PlayingCardSuite suite, string display,
            string details, List<Actions.Action> actions, Type[] responses)
            : this(color, suite, display, details, actions)
        {
            this.Responses = responses;
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

        public void Discard()
        {
            this.Owner.Hand.Remove(this);
            this.Context.Deck.Discard.Add(this);
        }

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
            var type = Type.GetType(String.Format("dab.SGS.Core.PlayingCard.{0}", cardType));
            var fnc = type.GetMethod("GetCardFromJson");

            return (PlayingCard)fnc.Invoke(null, new object[] { obj, selectCard, validCard });
        }

        /// <summary>
        /// Pass in an array
        /// </summary>
        /// <param name="cardTypes"></param>
        /// <returns></returns>
        public static Type[] ParseResponsesFromjson(dynamic cardTypes)
        {
            var l = new List<Type>();
            foreach(var cardType in cardTypes)
            {
                l.Add(Type.GetType(String.Format("dab.SGS.Core.PlayingCard.{0}", cardType)));
            }

            return l.ToArray();
        }


        private string display = String.Empty;
        private string details = String.Empty;
        private PlayingCardSuite suite;
        private PlayingCardColor color;

    }
}

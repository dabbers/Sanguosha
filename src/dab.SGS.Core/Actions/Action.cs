using dab.SGS.Core.PlayingCards;
using dab.SGS.Core.PlayingCards.Equipments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace dab.SGS.Core.Actions
{
    /// <summary>
    /// Have the player select a card
    /// </summary>
    /// <returns></returns>
    public delegate PlayingCard SelectCard(Player player);

    /// <summary>
    /// Have the player select maxCards cards
    /// </summary>
    /// <returns></returns>
    public delegate PlayingCard[] SelectCards(Player player, int minCards, int maxCards);

    /// <summary>
    /// Select a player to target
    /// </summary>
    /// <returns></returns>
    public delegate Player SelectTargetWithinRange(Player player, int range = 0);


    /// <summary>
    /// Is this a valid card to perform this action on?
    /// </summary>
    /// <param name="card"></param>
    /// <returns></returns>
    public delegate bool IsValidCard(PlayingCard card);

    public delegate Action SelectAction(Player player);



    public abstract class Action
    {
        public string Display { get { return this.display; } }

        public Action(string display)
        {
            this.display = display;
        }

        public abstract bool Perform(SelectedCardsSender sender, Player player, GameContext context);

        public virtual bool Perform(object sender, Player player, GameContext context, PlayingCardStageTracker result)
        {
            return false;
        }

        public virtual bool Perform(object sender, Player player, GameContext context, PlayingCardStageTracker result, WeaponEquipmentPlayingCard weapon)
        {
            return true;
        }

        public virtual int Perform(object sender, Player player, PlayingCardStageTracker result, WeaponEquipmentPlayingCard weapon)
        {
            return 0;
        }

        public static Action ActionFromJson(dynamic obj, SelectCard selectCard, IsValidCard validCard)
        {
            string cardType = obj.Type.ToString();
            var type = Type.GetType(String.Format("dab.SGS.Core.Actions.{0}", cardType));
            var fnc = type.GetMethod("ActionFromJson");

            // If no static ActionFromJson method is created, we can use the "defaul" constructor.
            if (fnc == null)
            {
                return (Action)Activator.CreateInstance(type);
            }
            else
            {
                return (Action)fnc.Invoke(null, new object[] { obj, selectCard, validCard });
            }
        }

        internal static List<Action> ActionsFromJson(dynamic actions, SelectCard selectCard, IsValidCard validCard)
        {
            if (actions == null) return null;

            var lst = new List<Action>();

            foreach(var action in actions)
            {
                lst.Add(Action.ActionFromJson(action, selectCard, validCard));
            }

            return lst;
        }

        private string display = String.Empty;
    }
}

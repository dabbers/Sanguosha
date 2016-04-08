using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dab.SGS.Core.PlayingCards;
using System.Reflection;

namespace dab.SGS.Core
{
    

    public class GameContext
    {
        public static int DEFAULT_MAX_ATTACKS = 1;

        public Player[] Players { get { return this.players.ToArray(); } }

        public List<Roles> Roles { get; set; }

        public Player Turn { get; set; }
        public TurnStages TurnStage { get; set; }
        public PlayingCardStageTracker PlayStageTracker { get; set; }

        public Deck Deck { get; protected set; }

        public class GenericHoldingArea
        {
            public enum HoldingAreaVisibility
            {
                All,
                Player
            }

            public HoldingAreaVisibility Visibilty { get; set; }

            public List<PlayingCard> Cards { get; set; }
        }

        /// <summary>
        /// The generic holding area for events like peach garden, or special abilities where a player draws n cards and picks m.
        /// </summary>
        public GenericHoldingArea HoldingArea { get; set; }

        /// <summary>
        /// Create a new game context to hold a game. This context handles the players,
        /// their turns, the turn stages, and attack stages.
        /// </summary>
        /// <param name="discardSelect">The delegate for selcting a card to discard.</param>
        public GameContext(Deck deck, Actions.SelectCard discardSelect)
        {
            this.DefaultDraw = new Actions.DrawAction(2);
            this.DefaultDiscard = new Actions.ReduceHandsizeDiscardAction(discardSelect);
            this.Roles = new List<Core.Roles>();
            this.TurnStage = TurnStages.End;

            foreach (var card in deck.AllCards)
            {
                card.Context = this;
            }

            this.Deck = deck;
        }

        public Actions.Action SetupGame(bool dealCards = true)
        {
            if (this.Players.Count() < 3) throw new Exception("Not enough players");

            this.dealCardsToPlayers();
            this.Turn = this.Players.First().Left;

            var roles = Player.GetRoles(this.Players.Count());
            var computedRoles = this.Players.Select(p => p.Role).ToArray();

            Array.Sort(computedRoles);
            Array.Sort(roles);

            var rolesList = new List<Roles>(roles);

            for (var i = 0; i < computedRoles.Length; i++)
            {
                if (computedRoles[i] != Core.Roles.Random && computedRoles[i] != roles[i])
                {
                    throw new Exception("Invalid roles assigned!");
                }
            }

            if (computedRoles.Contains(Core.Roles.Random))
            {
                var newRoles = this.Players.Where(p =>
                {
                    if (rolesList.Contains(p.Role))
                    {
                        rolesList.Remove(p.Role);
                        return false;
                    }

                    return true;
                }).ToArray();

                if (rolesList.Count > 1)
                    rolesList.Shuffle(new Random());

                for(var i = 0; i < rolesList.Count; i++)
                {
                    newRoles[i].Role = rolesList[i];
                }
            }

            if (dealCards)
                return this.RoateTurnStage();
            else
                return this.EmptyAction;
        }

        private void dealCardsToPlayers()
        {
            foreach (var player in this.Players)
            {
                for(var i = 0; i < 4; i++)
                {
                    var card = this.Deck.Draw();
                    card.Owner = player;
                    player.Hand.Add(card);
                }
            }
        }

        /// <summary>
        /// Proceed to the next turn stage. If it is the end of a player's turn, auto switch to the next player
        /// and beghin his turn.
        /// 
        /// Will return the action you must perform for the player. 
        /// </summary>
        /// <returns></returns>
        public Actions.Action RoateTurnStage()
        {
            if (this.TurnStage != TurnStages.End && this.TurnStage != TurnStages.AttackEnd)
            {
                this.TurnStage++;
            }
            else if (this.TurnStage == TurnStages.End)
            {
                this.TurnStage = TurnStages.Start;
                this.Turn = this.Turn.Right;
            }
            else if (this.TurnStage == TurnStages.AttackEnd)
            {
                // Resume turn
                this.TurnStage = TurnStages.Play;
            }


            Actions.Action res = this.EmptyAction;

            if (!this.Turn.TurnStageActions.TryGetValue(this.TurnStage, out res))
            { // Is this needed? Not sure if it will overwrite with null...
                res = this.EmptyAction;
            }

            return res;
        }

        public void AddPlayer(string display, Heroes.HeroCard hero, Roles role)
        {
            var p = new Player(display);
            p.CurrentHealth = 4;
            p.DistanceModifiers = 0;
            p.MaxHealth = 4 + (role == Core.Roles.King ? 1 : 0);
            p.MaxHandSize = p.MaxHealth;
            p.Role = role;

            p.TurnStageActions.Add(TurnStages.Draw, this.DefaultDraw);
            p.TurnStageActions.Add(TurnStages.Discard, this.DefaultDiscard);
            p.TurnStageActions.Add(TurnStages.End, new Actions.ResetAttackCounterAction());
            
            
            // TODO: Load Hero modifications. Do it AFTER assigning defaults, so our skills 
            //      can chain our draws if they want.

            if (this.players.Count == 0)
            {
                p.Left = p.Right = p;
            }
            else
            {
                p.Left = this.players.Last();
                p.Right = this.players.First();
                this.players.Last().Right = p;
                this.Players.First().Left = p;
            }

            this.players.Add(p);
        }

        public void AddPlayer(string display, Heroes.HeroCard hero)
        {
            this.AddPlayer(display, hero, Core.Roles.Random);
        }


        private Actions.Action DefaultDraw;
        private Actions.Action DefaultDiscard;
        private Actions.Action EmptyAction = new Actions.EmptyAction("Empty Action");
        private List<Player> players = new List<Player>();
    }
}

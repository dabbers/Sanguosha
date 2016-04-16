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

        public TargetPlayer AnyPlayer { get { return this.anyPlayer; } }

        public List<Roles> Roles { get; set; }

        public Player CurrentPlayerTurn { get { return this.CurrentPlayStage.Source.Target; } set { this.CurrentPlayStage.Source = new TargetPlayer(value); } }
        public TurnStages CurrentTurnStage { get { return this.CurrentPlayStage.Stage; } set { this.CurrentPlayStage.Stage = value; } }
        public PlayingCardStageTracker CurrentPlayStage { get; set; }
        public Stack<PlayingCardStageTracker> PreviousStages { get; set; }

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
        public GameContext(Deck deck)
        {
            this.DefaultDraw = new Actions.DrawAction(2);
            this.DefaultDiscard = new Actions.ReduceHandsizeDiscardAction();
            this.Roles = new List<Core.Roles>();
            this.CurrentPlayStage = new PlayingCardStageTracker()
            {
                Stage = TurnStages.End
            };
            this.PreviousStages = new Stack<PlayingCardStageTracker>();

            if (null != deck)
            {
                foreach (var card in deck.AllCards)
                {
                    card.Context = this;
                }
            }

            this.Deck = deck;
        }

        public Actions.Action SetupGame(bool dealCards = true)
        {
            if (this.Players.Count() < 3) throw new Exception("Not enough players");

            this.dealCardsToPlayers();
            this.CurrentPlayStage.Source = new TargetPlayer(this.Players.First().Left);

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
        /// Will return the action you must perform for the player. It will not do any pop/push of the stage tracker stack
        /// That is only done by the actions that perform new "mini turns"
        /// 
        /// This method won't proceed to the next stage if we are still expecting input from players. Set expectinginputfrom to null
        /// or AnyPlayer to proceed to the next stage.
        /// </summary>
        /// <returns></returns>
        public Actions.Action RoateTurnStage()
        {
            if (this.CurrentPlayStage.Stage != TurnStages.End && this.CurrentPlayStage.Stage != TurnStages.AttackEnd &&
                this.CurrentPlayStage.Stage != TurnStages.PlayScrollEnd && 
                (this.CurrentPlayStage.ExpectingIputFrom.Player == null || this.CurrentPlayStage.ExpectingIputFrom.Player == this.AnyPlayer))
            {
                this.CurrentPlayStage.Stage++;
            }
            else if (this.CurrentPlayStage.Stage == TurnStages.End)
            {
                this.CurrentPlayStage.Stage = TurnStages.Start;
                this.CurrentPlayStage.Source = new TargetPlayer(this.CurrentPlayStage.Source.Target.Right);
            }
            else if (this.CurrentPlayStage.Stage == TurnStages.AttackEnd || this.CurrentPlayStage.Stage == TurnStages.PlayScrollEnd)
            {
                // Resume turn
                this.CurrentPlayStage.Stage = TurnStages.Play;
            }


            Actions.Action res = this.EmptyAction;

            if (!this.CurrentPlayStage.Source.Target.TurnStageActions.TryGetValue(this.CurrentPlayStage.Stage, out res))
            { // Is this needed? Not sure if it will overwrite with null...
                res = this.EmptyAction;
            }

            return res;
        }

        public void AddPlayer(string display, Heroes.HeroCard hero, Roles role)
        {
            var p = new Player(display, 4, role);
            p.DistanceModifiers = 0;
            p.HandSizeModifies = 0;

            p.TurnStageActions.Add(TurnStages.Draw, this.DefaultDraw);
            p.TurnStageActions.Add(TurnStages.Discard, this.DefaultDiscard);
            p.TurnStageActions.Add(TurnStages.End, new Actions.ResetAttackCounterAction());
            // Perform the card so we can finish up its special damage stuff
            p.TurnStageActions.Add(TurnStages.AttackDamage, new Actions.PerformCardAction(() => this.CurrentPlayStage.Cards.Activator));
            
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
        private TargetPlayer anyPlayer = new TargetPlayer(new Player("Any Player", 0, Core.Roles.Random));
        private Actions.Action EmptyAction = new Actions.EmptyAction("Empty Action");
        private List<Player> players = new List<Player>();
    }
}

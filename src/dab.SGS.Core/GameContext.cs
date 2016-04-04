using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dab.SGS.Core
{
    

    public class GameContext
    {
        public Player[] Players { get { return this.players.ToArray(); } }

        public List<Roles> Roles { get; set; }

        public Player Turn { get; set; }
        public TurnStages TurnStage { get; set; }
        public AttackStageTracker AttackStageTracker { get; set; }

        public PlayingCard.Deck Deck { get; protected set; }

        /// <summary>
        /// Create a new game context to hold a game. This context handles the players,
        /// their turns, the turn stages, and attack stages.
        /// </summary>
        /// <param name="discardSelect">The delegate for selcting a card to discard.</param>
        public GameContext(PlayingCard.Deck deck, Actions.SelectCard discardSelect)
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

        public Actions.Action SetupGame()
        {
            if (this.Players.Count() < 3) throw new Exception("Not enough players");

            this.dealCardsToPlayers();
            this.Turn = this.Players.First().Left;

            return this.RoateTurnStage();
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


            Actions.Action res = null;

            this.Turn.TurnStageActions.TryGetValue(this.TurnStage, out res);

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
                this.players.First().Right = p;
                this.Players.First().Left = p;
            }

            this.players.Add(p);
        }
        
        private Actions.Action DefaultDraw;
        private Actions.Action DefaultDiscard;
        private List<Player> players = new List<Player>();
    }
}

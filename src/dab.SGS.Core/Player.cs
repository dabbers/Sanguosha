using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dab.SGS.Core.PlayingCards;
using dab.SGS.Core.PlayingCards.Equipments;

namespace dab.SGS.Core
{
    public enum Roles
    {
        King,
        Minister,
        Rebel,
        TurnCoat,
        Random
    }

    public class Player
    {
        public string Display { get; set; }
        public Player Left { get; set; }
        public Player Right { get; set; }
        public int MaxHealth { get; set; }
        public int CurrentHealth { get; set; }
        public int MaxHandSize { get { return this.CurrentHealth + this.HandSizeModifies; } }
        public int HandSizeModifies { get; set; }
        public int DistanceModifiers { get; set; }

        public bool Flipped { get; set; }
        public bool Chained { get; set; }

        public bool IsDying { get { return this.CurrentHealth < 1; } }

        public Roles Role { get; set; }
        public int AttacksLeft { get; set; }

        public int WinesLeft { get; set; }
        public bool WineInEffect { get; set; }

        public List<Heroes.HeroCard> Heros { get; set; }

        public Heroes.HeroCard ActiveHero
        {
            get
            {
                return this.activeHero;
            }
            set
            {
                if (!this.Heros.Contains(value)) throw new Exceptions.InvalidScenarioException("Hero must be in the list of heroes");

                this.activeHero = value;
            }
        }

        public List<PlayingCard> Hand
        {
            get;
            set;
        }

        public TurnStageDictionary TurnStageActions { get; set; }

        public Player(string name, int maxhealth, Roles role)
        {
            this.Display = name;
            this.PlayerArea = new PlayArea();
            this.Hand = new List<PlayingCard>();
            this.Heros = new List<Heroes.HeroCard>();
            this.TurnStageActions = new TurnStageDictionary();
            this.AttacksLeft = GameContext.DEFAULT_MAX_ATTACKS;
            this.WinesLeft = GameContext.DEFAULT_MAX_NONDEATHWINES;
            this.WineInEffect = false;
            this.Role = role;
            this.CurrentHealth = this.MaxHealth = maxhealth + (this.Role == Roles.King ? 1 : 0);

        }

        public class PlayArea
        {
            public ShieldEquipmentPlayingCard Shield { get; set; }
            public WeaponEquipmentPlayingCard Weapon { get; set; }

            public HorseEquipmentPlayingCard PlusHorse { get; set; }

            public HorseEquipmentPlayingCard MinusHorse { get; set; }

            public List<PlayingCard> DelayedScrolls { get; set; }

            public List<PlayingCard> FaceUpPlayingCards { get; set; }

            public List<PlayingCard> FaceDownPlayingCards { get; set; }

            public void DiscardArea()
            {
                if (this.DelayedScrolls != null)
                    foreach (var card in this.DelayedScrolls) card.Discard();
                if (this.FaceDownPlayingCards != null)
                    foreach (var card in this.FaceDownPlayingCards) card.Discard();
                if (this.FaceUpPlayingCards != null)
                    foreach (var card in this.FaceUpPlayingCards) card.Discard();

                this.Shield?.Discard();
                this.Weapon?.Discard();
                this.PlusHorse?.Discard();
                this.MinusHorse?.Discard();

            }

            public int NumberOfCards {  get
                {
                    return (this.Shield != null ? 1 : 0) + (this.Weapon != null ? 1 : 0) + (this.PlusHorse != null ? 1 : 0) +
                        (this.MinusHorse != null ? 1 : 0) + (this.DelayedScrolls?.Count ?? 0) + (this.FaceUpPlayingCards?.Count ?? 0) +
                        (this.FaceDownPlayingCards?.Count ?? 0);
                }
            }
        }

        public PlayArea PlayerArea { get; private set; }

        public bool IsPlayerInAttackRange(Player target)
        {
            
            return this.GetAttackRange() >= this.GetDistance(target);
        }

        public int GetDistance(Player target)
        {
            int dist = target.GetDistanceTotal() + 1;
            var pPlayerR = this.Right;
            var pPlayerL = this.Left;

            while(pPlayerL != target || pPlayerR != target)
            {
                dist++;
                pPlayerL = pPlayerL.Left;
                pPlayerR = pPlayerR.Right;
            }

            return dist;
        }

        public int GetAttackRange()
        {
            int range = this.PlayerArea.Weapon?.Range ?? 1;
            range += (this.PlayerArea.MinusHorse?.Distance ?? 0) * -1;

            if (this.DistanceModifiers < 0) range += (this.DistanceModifiers * -1);

            return range;
        }

        public int GetDistanceTotal()
        {
            return (this.PlayerArea.PlusHorse?.Distance ?? 0) + this.DistanceModifiers;
        }

        public int GetTotalCardsToPlayer()
        {
            return this.Hand.Count + this.PlayerArea.NumberOfCards;
        }

        public override string ToString()
        {
            return this.Display;
        }

        public static Roles[] GetRoles(int players)
        {
            switch(players)
            {
                case 2:
                    return new Roles[] { Roles.King, Roles.Rebel };
                case 3:
                    return new Roles[] { Roles.King, Roles.Rebel, Roles.Minister };
                case 4:
                    return new Roles[] { Roles.King, Roles.Rebel, Roles.Rebel, Roles.Minister };
                case 5:
                    return new Roles[] { Roles.King, Roles.Rebel, Roles.Rebel, Roles.Minister, Roles.TurnCoat };
                case 6:
                    return new Roles[] { Roles.King, Roles.Rebel, Roles.Rebel, Roles.Rebel, Roles.Minister, Roles.TurnCoat };
                case 7:
                    return new Roles[] { Roles.King, Roles.Rebel, Roles.Rebel, Roles.Rebel, Roles.Minister, Roles.Minister, Roles.TurnCoat };
                case 8:
                    return new Roles[] { Roles.King, Roles.Rebel, Roles.Rebel, Roles.Rebel, Roles.Rebel, Roles.Minister, Roles.Minister, Roles.TurnCoat };
                case 9:
                    return new Roles[] { Roles.King, Roles.Rebel, Roles.Rebel, Roles.Rebel, Roles.Rebel, Roles.Minister, Roles.Minister, Roles.Minister, Roles.TurnCoat };
                case 10:
                    return new Roles[] { Roles.King, Roles.Rebel, Roles.Rebel, Roles.Rebel, Roles.Rebel, Roles.Rebel, Roles.Minister, Roles.Minister, Roles.Minister, Roles.TurnCoat };
                default:
                    throw new Exceptions.InvalidScenarioException("Invalid player number for roles");

            }
        }

        private Heroes.HeroCard activeHero;
    }
}

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
        public int MaxHandSize { get; set; }

        public int DistanceModifiers { get; set; }

        public bool Flipped { get; set; }
        public bool Chained { get; set; }

        public bool IsDying { get { return this.CurrentHealth < 1; } }

        public Roles Role { get; set; }
        public int AttacksLeft { get; set; }

        public List<Heroes.HeroCard> Heros { get; set; }

        public Heroes.HeroCard ActiveHero
        {
            get
            {
                return this.activeHero;
            }
            set
            {
                if (!this.Heros.Contains(value)) throw new Exception("Hero must be in the list of heroes");

                this.activeHero = value;
            }
        }

        public List<PlayingCard> Hand { get; set; }

        public TurnStageDictionary TurnStageActions { get; set; }

        public Player(string name)
        {
            this.Display = name;
            this.PlayerArea = new PlayArea();
            this.Hand = new List<PlayingCard>();
            this.Heros = new List<Heroes.HeroCard>();
            this.TurnStageActions = new TurnStageDictionary();
        }

        public class PlayArea
        {
            public ShieldEquipmentPlayingCard Shield { get; set; }
            public WeaponEquipmentPlayingCard Weapon { get; set; }

            public HorseEquipmentPlayingCard PlusHorse { get; set; }

            public HorseEquipmentPlayingCard MinusHorse { get; set; }

            public List<Object> DelayedScrolls { get; set; }

            public List<PlayingCard> FaceUpPlayingCards { get; set; }

            public List<PlayingCard> FaceDownPlayingCards { get; set; }
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
                    throw new Exception("Invalid player number for roles");

            }
        }

        private Heroes.HeroCard activeHero;
    }
}

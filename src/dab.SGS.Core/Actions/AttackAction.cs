using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dab.SGS.Core.PlayingCard;

namespace dab.SGS.Core.Actions
{
    public class AttackAction : Action
    {
        public AttackAction(string display, int damage) : base(display)
        {
            this.damage = damage;
        }

        public override bool Perform(object sender, Player player, GameContext context)
        {
            // When can this happen? Attack card, or Borrowed sword

            if (sender is AttackBasicPlayingCard)
            {
                var attack = (AttackBasicPlayingCard)sender;

            }

            // How attacking a player works:
            var card = (PlayingCard.PlayingCard)sender;
            
            // An attack cannot be happening already
            if (context.AttackStageTracker != null)
            {
                throw new Exception("An attack is already happening. Is this a bug?");
            }

            context.AttackStageTracker = new AttackStageTracker()
            {
                Cards = new List<PlayingCard.PlayingCard>() { card },
                Source = player,
                Targets = new List<Player>()
            };

            context.TurnStage = TurnStages.ChooseTargets;
            return true;
        }

        public static new Action ActionFromJson(dynamic obj,
            SelectCard selectCard, IsValidCard validCard)
        {
            var display = obj.Display.ToString();
            int damage = obj.Damage;

            return new AttackAction(display, damage);
        }
        
        private int damage;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dab.SGS.Core.Actions
{
    public class DodgeAction : Action
    {
        public DodgeAction() : base("Dodge")
        {
        }

        public override bool Perform(object sender, Player player, GameContext context)
        {
            switch (context.TurnStage)
            {
                case TurnStages.CardResponse:
                    var target = context.AttackStageTracker.Targets.Find(p => p.Target.Display == player.Display);
                    target.Damage = 0;
                    break;
            }


            return true;
        }
        public static new Action ActionFromJson(dynamic obj,
            SelectCard selectCard, IsValidCard validCard)
        {
            return new DodgeAction();
        }

    }
}

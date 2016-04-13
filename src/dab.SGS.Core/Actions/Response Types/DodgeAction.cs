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

        public override bool Perform(SelectedCardsSender sender, Player player, GameContext context)
        {
            switch (context.CurrentPlayStage.Stage)
            {
                case TurnStages.AttackCardResponse:
                    var target = context.CurrentPlayStage.Targets.Find(p => p.Target.Display == player.Display);
                    target.Damage = 0;
                    target.Result = TargetResult.Failed;
                    break;
                default: // We cannot use this card here.
                    return false;
            }

            sender.DiscardAll();

            return true;
        }

        public static new Action ActionFromJson(dynamic obj,
            SelectCard selectCard, IsValidCard validCard)
        {
            return new DodgeAction();
        }

    }
}

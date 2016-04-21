using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dab.SGS.Core.PlayingCards;

namespace dab.SGS.Core.Actions
{
    public class AttackAction : Action
    {
        public AttackAction(string display, int damage) : base(display)
        {
            this.damage = damage;
        }

        public override bool Perform(SelectedCardsSender sender, Player player, GameContext context)
        {
            switch (context.CurrentPlayStage.Stage)
            {
                // The attack was first played. Go into target select mode.
                case TurnStages.Play:
                    // An attack cannot be happening already
                    if (context.PreviousStages.Count != 0)
                    {
                        throw new Exception("There are other turnstages in the stack. Are we expecting this? (Previous turnstage:" + context.PreviousStages.Peek().ToString());
                    }

                    context.PreviousStages.Push(context.CurrentPlayStage);
                    var results = sender;


                    context.CurrentPlayStage = new PlayingCardStageTracker()
                    {
                        Cards = results,
                        Source = new TargetPlayer(player),
                        Targets = new List<TargetPlayer>(),
                        Stage = TurnStages.AttackChooseTargets
                    };

                    context.CurrentPlayStage.ExpectingIputFrom.Player = context.CurrentPlayStage.Source;
                    context.CurrentPlayStage.ExpectingIputFrom.Prompt = new Prompts.UserPrompt(Prompts.UserPromptType.TargetRangeMN)
                        { MinRange = 1, MaxRange = player.GetAttackRange(), MaxCards = 1, MinTargets = 1 };

                    player.AttacksLeft--;

                    return false;
                case TurnStages.AttackChooseTargets:
                    int extra = (sender.Count(p => p.IsPlayedAsWine()) > 0 || player.WineInEffect ? 1 : 0);
                    player.WineInEffect = false;

                    foreach (var tp in context.CurrentPlayStage.Targets)
                    {
                        tp.Damage = this.damage + extra;
                    }

                    context.CurrentPlayStage.ExpectingIputFrom.Player = null;

                    return false;
                case TurnStages.AttackDamage:
                    var tmpStage = context.PreviousStages.Pop();

                    foreach(var tp in context.CurrentPlayStage.Targets)
                    {
                        if (tp.Result == TargetResult.Success || tp.Result == TargetResult.None)
                        {
                            // adjust for shield damage
                            tp.Target.CurrentHealth -= tp.Target.PlayerArea.Shield?.GetExtraDamage(context.CurrentPlayStage, context.CurrentPlayStage.Source.Target.PlayerArea.Weapon) ?? 0;

                            tp.Target.CurrentHealth -= tp.Damage;
                        }

                    }

                    // Clear up the stage tracker for the next turn.
                    context.CurrentPlayStage = tmpStage;
                    break;
                case TurnStages.PlayScrollPlaceResponse:
                case TurnStages.PlayScrollPlaced:
                    // Duel:
                    if (context.CurrentPlayStage.Cards.Activator.IsPlayedAsDuel())
                    {
                        context.CurrentTurnStage = TurnStages.PlayScrollPlaceResponse;

                        // Reset the next player so they can play their card.
                        context.CurrentPlayStage.ExpectingIputFrom.Player.Result = TargetResult.Failed;
                        context.CurrentPlayStage.PeristedTargetEnumerator.PeekRotate.Result = TargetResult.None;
                        break;
                    }

                    throw new Exception("Attack was somehow activated for PlayScrollPlace(Response). But activator wasn't played as duel." + context.CurrentPlayStage.Cards.Activator.ToString());
                default:
                    throw new Exception("Unknown stage reached for attack action: " + context.CurrentPlayStage.Stage.ToString());
            }

            sender?.DiscardAll();
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

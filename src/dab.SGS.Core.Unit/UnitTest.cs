using System;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using dab.SGS.Core;
using dab.SGS.Core.PlayingCards;
using dab.SGS.Core.Actions;
using System.Collections.Generic;
using dab.SGS.Core.PlayingCards.Basics;
using dab.SGS.Core.PlayingCards.Scrolls;
using dab.SGS.Core.PlayingCards.Equipments;

namespace dab.SGS.Core.Unit
{
    [TestClass]
    public class PlayTests
    {
        public static List<PlayingCard> GetDeck()
        {
            var deck = Deck.LoadCards(FileIOHelper.ReadFromDefaultFile("ms-appx:///Assets/deck.json"),
                p => p.Hand[0], p => true);

            return deck;
        }

        public static List<PlayingCard> GetAttackDodgeAlternateDeck(int count)
        {
            var d = new List<PlayingCard>();

            for (var i = 0; i < count; i += 2)
            {
                d.Add(new PlayingCards.Basics.AttackBasicPlayingCard(PlayingCardColor.Black, PlayingCardSuite.Club, "", new List<Actions.Action>() { new AttackAction("Attack", 1) }, PlayingCards.Basics.Elemental.None));
                d.Add(new PlayingCards.Basics.DodgeBasicPlayingCard(PlayingCardColor.Black, PlayingCardSuite.Club, "", new List<Actions.Action>() { new DodgeAction() }));
            }

            return d;
        }
        
        [TestMethod]
        public void TestRoleAssignment()
        {
            var ctx = new GameContext(new Deck(PlayTests.GetAttackDodgeAlternateDeck(22)));

            ctx.AddPlayer("P1", null, Roles.King);
            ctx.AddPlayer("P2", null);
            ctx.AddPlayer("P3", null);
            ctx.AddPlayer("P4", null);
            ctx.AddPlayer("P5", null);

            ctx.SetupGame();

            Assert.AreEqual(Roles.King, ctx.Players[0].Role);

            var roles = new List<Roles>(Player.GetRoles(ctx.Players.Length));

            foreach (var player in ctx.Players)
            {
                roles.Remove(player.Role);
            }

            Assert.AreEqual(0, roles.Count);

        }





    }
}

namespace dab.SGS.Core.Actions
{
    public class PlayCardAction : Action
    {
        public PlayCardAction(string display, IsValidCard valid, SelectCard select) : base(display)
        {
            this.valid = valid;
            this.select = select;
        }

        public override bool Perform(object sender, Player player, GameContext context)
        {
            var card = this.select(player);

            if (! this.valid(card))
            {
                return false;
            }

            return card.Play(sender);
        }

        private IsValidCard valid;
        private SelectCard select;
    }
}

using System.Collections.Generic;

namespace Game.Domain
{
    public interface ICommand
    {
        void Resolve();
    }

    public class DrawCardCommand : ICommand
    {
        public int PlayerId;

        public void Resolve()
        {
            
        }
    }

    public class PlayCardCommand : ICommand
    {
        public int PlayerId;
        public int CardId;
        public List<int> targetIds;

        public void Resolve()
        {
            Card card = CardDatabase.Get(CardId);
            //ctx.selectedCards = cmd.targetIds;
            //ExecuteCard(card, state, ctx); // 里面要加上events.Add
            //广播发生在网络层，不在 Domain
        }
    }
}
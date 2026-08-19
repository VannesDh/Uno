using UnoBackend.Interfaces;

namespace UnoBackend.Models;

public class Discarded : IDiscarded
{
    public Stack<ICard> DiscardedCards { get; set;}
        public Discarded()
    {
        DiscardedCards = new Stack<ICard>();
    }
}
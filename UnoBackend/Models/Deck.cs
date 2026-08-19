using UnoBackend.Interfaces;
namespace UnoBackend.Models;
public class Deck : IDeck
{
    public Stack<ICard> DeckPiles{get; set;}

    public Deck(Stack<ICard> deckPiles)
    {
        DeckPiles = deckPiles;
    }
}
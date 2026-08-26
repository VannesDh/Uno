using UnoBackend.Interfaces;
using UnoBackend.Models.Enum;

namespace UnoBackend.Models;
public class Card : ICard
{
    public Guid Id { get; } = Guid.NewGuid();
    public Color Color {get; }
    public CardValue CardValue {get;}

    public Card(Color color, CardValue cardValue)
    {
        Color = color;
        CardValue = cardValue;
    }
    public Card(Guid id, Color color, CardValue cardValue)
    {
        Id = id;
        Color = color;
        CardValue = cardValue;
    }

}




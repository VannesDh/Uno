using UnoBackend.Interfaces;
using UnoBackend.Models.Enum;

namespace UnoBackend.Models;
public class Card : ICard
{
    public Color Color {get; }
    public CardValue CardValue {get;}

    public Card(Color color, CardValue cardValue)
    {
        Color = color;
        CardValue = cardValue;
    }
}




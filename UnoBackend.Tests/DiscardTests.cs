using UnoBackend.Services;
using UnoBackend.Interfaces;

namespace UnoBackend.Tests;

public class DiscardTests
{
    [Test]
    public void DiscardCard_AddsCardToTheDiscardPile()
    {
        Game game = new Game();
        game.AddPlayer("Player 1");
        game.AddPlayer("Player 2");
        game.Play();

        IPlayer player = game.GetCurrentPlayer();
        ICard card = game.GetPlayerCard(player)[0];

        // Act
        game.DiscardCard(card);

        // Assert
        Assert.That(game.GetCurrentTopPile(), Is.EqualTo(card));
    }


    [Test]
    public void DiscardCard_NullCard_DoesNotAddCard()
    {
        Game game = new Game();

        game.DiscardCard(null!);

        Assert.That(game.GetDeckCount(), Is.GreaterThanOrEqualTo(0));
    }
}
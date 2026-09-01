using UnoBackend.Services;
using UnoBackend.Interfaces;
using UnoBackend.Models;

namespace UnoBackend.Tests;

public class DrawCardTests
{
    [Test]
    public void DrawCard_PlayerReceivesCard()
    {
        // Arrange
        Game game = new Game();

        game.AddPlayer("Player 1");
        game.AddPlayer("Player 2");
        game.Play();

        IPlayer player = game.GetCurrentPlayer();
        int cardsBefore = game.GetPlayerCard(player).Count;

        game.DrawCard(player);

        int cardsAfter = game.GetPlayerCard(player).Count;

        Assert.That(cardsAfter, Is.EqualTo(cardsBefore + 1));
    }

    [Test]
    public void DrawCard_EmptyDeck_RenewDeck()
    {
        Game game = new Game();

        game.AddPlayer("Player 1");
        game.AddPlayer("Player 2");
        game.Play();
        
        IPlayer player = game.GetCurrentPlayer();
        game._deck.DeckPiles = new Stack<ICard>();

        
        game._discardedPile.DiscardedCards.Push(new Card(Models.Enum.Color.Red,Models.Enum.CardValue.One));
        game._discardedPile.DiscardedCards.Push(new Card(Models.Enum.Color.Red,Models.Enum.CardValue.Two));
        game._discardedPile.DiscardedCards.Push(new Card(Models.Enum.Color.Red,Models.Enum.CardValue.Three));
        game._discardedPile.DiscardedCards.Push(new Card(Models.Enum.Color.Red,Models.Enum.CardValue.Four));
        int discardedLength = game._discardedPile.DiscardedCards.Count();
        game.DrawCard(player);

        Assert.That(game._deck.DeckPiles.Count(), Is.EqualTo(discardedLength-2));
    }

    [Test]
    public void DrawCard_Penalty_PlayerReceivesCard()
    {
        // Arrange
        Game game = new Game();

        game.AddPlayer("Player 1");
        game.AddPlayer("Player 2");
        game.Play();

        IPlayer player = game.GetCurrentPlayer();
        int cardsBefore = game.GetPlayerCard(player).Count;

        game.DrawCard(player, true);
        int cardsAfter = game.GetPlayerCard(player).Count;

        Assert.That(cardsAfter, Is.EqualTo(cardsBefore + 1));
    }

    [Test]
    public void DrawCard_ReturnsDrawnCard()
    {
        // Arrange
        Game game = new Game();

        game.AddPlayer("Player 1");
        game.AddPlayer("Player 2");
        game.Play();

        IPlayer player = game.GetCurrentPlayer();

        ICard drawnCard = game.DrawCard(player);

        Assert.That(drawnCard, Is.Not.Null);
    }
}
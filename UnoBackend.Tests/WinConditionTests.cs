using UnoBackend.Services;
using UnoBackend.Interfaces;
using UnoBackend.Models;
using UnoBackend.Models.Enum;


namespace UnoBackend.Tests;

public class WinConditionTest
{
    [Test]
    public void UnoCalled_PlayerIsUno_NoDraw()
    {
        Game game = new Game();
        game.AddPlayer("Player 1");
        game.AddPlayer("Player 2");
        game.Play();

        IPlayer player = game.GetCurrentPlayer();

        game._cardInHand[player].Clear();
        game._cardInHand[player].Add(new Card(Color.Red, CardValue.Five));

        int cardsBefore = game._cardInHand[player].Count;

        game.CallUno(player);
        game.PlayerTurn(player);

        int cardsAfter = game._cardInHand[player].Count;

        Assert.That(cardsAfter, Is.EqualTo(cardsBefore));
    }
    [Test]
    public void UnoCalled_PlayerIsNotUno_Draw()
    {
        Game game = new Game();
        game.AddPlayer("Player 1");
        game.AddPlayer("Player 2");
        game.Play();

        IPlayer player = game.GetCurrentPlayer();
        int cardsBefore = game.GetPlayerCard(player).Count;
        game.CallUno(player);

        game.PlayerTurn(player);
        int cardsAfter = game.GetPlayerCard(player).Count;

        Assert.That(cardsAfter, Is.EqualTo(cardsBefore + 1));
    }

       [Test]
    public void UnoNotCalled_PlayerIsUno_Draw()
    {
        Game game = new Game();
        game.AddPlayer("Player 1");
        game.AddPlayer("Player 2");
        game.Play();

        IPlayer player = game.GetCurrentPlayer();
        
        game._cardInHand[player].Clear();
        game._cardInHand[player].Add(new Card(Color.Red, CardValue.Five));
        int cardsBefore = game.GetPlayerCard(player).Count;

        game.PlayerTurn(player);

        int cardsAfter = game.GetPlayerCard(player).Count;

        Assert.That(cardsAfter, Is.EqualTo(cardsBefore + 1));
    }



    [Test]
public void GetWinner_NoWinner_ReturnsNull()
{
    // Arrange
    Game game = new Game();

    // Act
    IPlayer? winner = game.GetWinner();

    // Assert
    Assert.That(winner, Is.Null);
}


[Test]
public void SetWinnerCallback_CallbackIsCalled_WhenWinnerIsTriggered()
{
    // Arrange
    Game game = new Game();

    game.AddPlayer("Player 1");
    game.AddPlayer("Player 2");
    game.Play();

    IPlayer player = game.GetCurrentPlayer();

    bool callbackCalled = false;

    game.SetWinnerCallback(winner =>
    {
        callbackCalled = true;
    });

    // Give player exactly one card
    game._cardInHand[player].Clear();

    ICard card = new Card(Color.Red, CardValue.Five);
    game._cardInHand[player].Add(card);

    // Make the card playable by putting a matching card
    // into the discard pile.
    game.DiscardCard(new Card(Color.Red, CardValue.Three));

    // Act
    game.PlayCard(card);

    // Assert
    Assert.That(callbackCalled, Is.True);
}


[Test]
public void WinnerCallback_SetsWinner_WhenPlayerHasNoCards()
{
    // Arrange
    Game game = new Game();

    game.AddPlayer("Player 1");
    game.AddPlayer("Player 2");
    game.Play();

    IPlayer player = game.GetCurrentPlayer();

    // Give player exactly one playable card
    game._cardInHand[player].Clear();

    ICard card = new Card(Color.Red, CardValue.Five);
    game._cardInHand[player].Add(card);

    // Put a matching card on the discard pile
    game.DiscardCard(new Card(Color.Red, CardValue.Three));

    // Act
    game.PlayCard(card);

    // Assert
    Assert.That(game.GetWinner().Name, Is.EqualTo(player.Name));
}





}
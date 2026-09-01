using NUnit.Framework;
using UnoBackend.Services;
using UnoBackend.Interfaces;
using UnoBackend.Models.Enum;
using UnoBackend.Models;

namespace UnoBackend.Tests;

public class GameStateTests
{
    #region RestartGame Tests

    [Test]
    public void RestartGame_KeepsPlayers()
    {
        // Arrange
        Game game = new Game();

        game.AddPlayer("Player 1");
        game.AddPlayer("Player 2");

        IPlayer playerBefore = game.GetCurrentPlayer();

        // Act
        game.RestartGame();

        // Assert
        Assert.That(game.GetCurrentPlayer(), Is.EqualTo(playerBefore));
        Assert.That(game._players, Has.Count.EqualTo(2));
    }


    [Test]
    public void RestartGame_ClearsPlayerHands()
    {
        // Arrange
        Game game = new Game();

        game.AddPlayer("Player 1");
        game.AddPlayer("Player 2");

        // Manually give Player 1 a card
        IPlayer player = game.GetCurrentPlayer();

        game._cardInHand[player] = new List<ICard>
        {
            new Card(Color.Red, CardValue.Five)
        };

        // Act
        game.RestartGame();

        // Assert
        Assert.That(game._cardInHand, Is.Empty);
    }


    [Test]
    public void RestartGame_CanAddNewPlayersWithoutRemovingExistingPlayers()
    {
        // Arrange
        Game game = new Game();

        game.AddPlayer("Player 1");
        game.AddPlayer("Player 2");

        // Act
        game.RestartGame();

        game.AddPlayer("Player 3");

        // Assert
        Assert.That(game._players, Has.Count.EqualTo(3));
    }

    #endregion


    #region ResetGame Tests

    [Test]
    public void ResetGame_RemovesAllPlayers()
    {
        // Arrange
        Game game = new Game();

        game.AddPlayer("Player 1");
        game.AddPlayer("Player 2");

        // Act
        game.ResetGame();

        // Assert
        Assert.That(game._players, Is.Empty);
    }


    [Test]
    public void ResetGame_ClearsPlayerHands()
    {
        // Arrange
        Game game = new Game();

        game.AddPlayer("Player 1");
        game.AddPlayer("Player 2");

        IPlayer player = game.GetCurrentPlayer();

        game._cardInHand[player] = new List<ICard>
        {
            new Card(Color.Red, CardValue.Five)
        };

        // Act
        game.ResetGame();

        // Assert
        Assert.That(game._cardInHand, Is.Empty);
    }


    [Test]
    public void ResetGame_CanAddPlayersAgain()
    {
        // Arrange
        Game game = new Game();

        game.AddPlayer("Player 1");
        game.AddPlayer("Player 2");

        // Act
        game.ResetGame();

        game.AddPlayer("Player 3");
        game.AddPlayer("Player 4");

        // Assert
        Assert.That(game._players, Has.Count.EqualTo(2));
    }

    #endregion
}
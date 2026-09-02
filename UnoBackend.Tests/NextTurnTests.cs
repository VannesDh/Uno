using UnoBackend.Services;
using UnoBackend.Interfaces;
using Microsoft.Extensions.Logging.Abstractions;


public class NextTurnTests
{
    

[Test]
public void NextPlayer_NotSkipped_MovesToNextPlayer()
{
    // Arrange
    Game game = new Game(NullLogger<Game>.Instance);

    game.AddPlayer("Player 1");
    game.AddPlayer("Player 2");
    game.AddPlayer("Player 3");
    game.Play();

    IPlayer firstPlayer = game.GetCurrentPlayer();

    // Act
    game.NextPlayer(false);

    // Assert
    IPlayer nextPlayer = game.GetCurrentPlayer();

    Assert.That(nextPlayer, Is.Not.EqualTo(firstPlayer));
}


[Test]
public void NextPlayer_Skipped_SkipsNextPlayer()
{
    // Arrange
    Game game = new Game(NullLogger<Game>.Instance);

    game.AddPlayer("Player 1");
    game.AddPlayer("Player 2");
    game.AddPlayer("Player 3");
    game.Play();

    IPlayer firstPlayer = game.GetCurrentPlayer();

    // Act
    game.NextPlayer(true);

    // Assert
    IPlayer currentPlayer = game.GetCurrentPlayer();

    Assert.That(currentPlayer, Is.Not.EqualTo(firstPlayer));
}


[Test]
public void NextPlayer_WhenAtLastPlayer_WrapsAround()
{
    // Arrange
    Game game = new Game(NullLogger<Game>.Instance);

    game.AddPlayer("Player 1");
    game.AddPlayer("Player 2");
    game.AddPlayer("Player 3");
    game.Play();

    IPlayer firstPlayer = game.GetCurrentPlayer();

    // Move to Player 2
    game.NextPlayer(false);

    // Move to Player 3
    game.NextPlayer(false);

  
    game.NextPlayer(false);

    IPlayer currentPlayer = game.GetCurrentPlayer();

    Assert.That(currentPlayer, Is.EqualTo(firstPlayer));
}
}

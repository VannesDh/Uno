using NUnit.Framework;
using UnoBackend.Services;
using UnoBackend.Interfaces;
using UnoBackend.Models;
using UnoBackend.Models.Enum;

public class ChooseColorTests
{
    [Test]
    public void ChooseColor_WhenWaitingForColor_SetsChosenColor()
    {
        // Arrange
        Game game = new Game();

        game.AddPlayer("Player 1");
        game.AddPlayer("Player 2");

        IPlayer player = game.GetCurrentPlayer();

        game._cardInHand[player] = new List<ICard>();

        ICard wildCard = new Card(Color.Wild, CardValue.Wild);

        game._cardInHand[player].Add(wildCard);
        game.DiscardCard(new Card(Color.Red, CardValue.Five));

        game.PlayCard(wildCard);

        // Act
        game.ChooseColor(Color.Blue);

        // Assert
        Assert.That(game._chosenColor, Is.EqualTo(Color.Blue));
        Assert.That(game.WaitingForColor(), Is.False);
    }


    [Test]
    public void ChooseColor_WhenNotWaitingForColor_DoesNothing()
    {
        // Arrange
        Game game = new Game();

        game.AddPlayer("Player 1");
        game.AddPlayer("Player 2");

        // Act
        game.ChooseColor(Color.Blue);

        // Assert
        Assert.That(game._chosenColor, Is.Null);
        Assert.That(game.WaitingForColor(), Is.False);
    }


    [Test]
    public void ChooseColor_WhenWaitingForColor_CanChooseWild()
    {
        // Arrange
        Game game = new Game();

        game.AddPlayer("Player 1");
        game.AddPlayer("Player 2");

        IPlayer player = game.GetCurrentPlayer();

        game._cardInHand[player] = new List<ICard>();

        ICard wildCard = new Card(Color.Wild, CardValue.Wild);

        game._cardInHand[player].Add(wildCard);
        game.DiscardCard(new Card(Color.Red, CardValue.Five));

        game.PlayCard(wildCard);

        // Act
        game.ChooseColor(Color.Wild);

        // Assert
        Assert.That(game._chosenColor, Is.EqualTo(Color.Wild));
        Assert.That(game.WaitingForColor(), Is.False);
    }
}
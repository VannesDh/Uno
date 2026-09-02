using UnoBackend.Services;
using UnoBackend.Interfaces;
using UnoBackend.Models.Enum;
using UnoBackend.Models;
using Microsoft.Extensions.Logging.Abstractions;

public class PlayCardTests
{
    #region CheckPlayableCard Tests

    [Test]
    public void CheckPlayableCard_PlayerHasPlayableCards_ReturnsPlayableCardIds()
    {
        // Arrange
        Game game = new Game(NullLogger<Game>.Instance);

        game.AddPlayer("Player 1");
        game.AddPlayer("Player 2");
        game.Play();

        IPlayer player = game.GetCurrentPlayer();

        game._cardInHand[player].Clear();

        ICard playableCard = new Card(Color.Red, CardValue.Five);
        ICard unplayableCard = new Card(Color.Blue, CardValue.Seven);

        game._cardInHand[player].Add(playableCard);
        game._cardInHand[player].Add(unplayableCard);

        game.DiscardCard(new Card(Color.Red, CardValue.Three));

        // Act
        List<Guid> playableCards = game.CheckPlayableCard(player);

        // Assert
        Assert.That(playableCards, Has.Count.EqualTo(1));
        Assert.That(playableCards, Does.Contain(playableCard.Id));
    }


    [Test]
    public void CheckPlayableCard_NoPlayableCards_ReturnsEmptyList()
    {
        // Arrange
        Game game = new Game(NullLogger<Game>.Instance);

        game.AddPlayer("Player 1");
        game.AddPlayer("Player 2");
        game.Play();

        IPlayer player = game.GetCurrentPlayer();

        game._cardInHand[player].Clear();

        ICard card1 = new Card(Color.Blue, CardValue.Five);
        ICard card2 = new Card(Color.Green, CardValue.Seven);

        game._cardInHand[player].Add(card1);
        game._cardInHand[player].Add(card2);

        game.DiscardCard(new Card(Color.Red, CardValue.Three));

        // Act
        List<Guid> playableCards = game.CheckPlayableCard(player);

        // Assert
        Assert.That(playableCards, Is.Empty);
    }


    [Test]
    public void CheckPlayableCard_PlayerHasNoCards_ReturnsEmptyList()
    {
        // Arrange
        Game game = new Game(NullLogger<Game>.Instance);

        game.AddPlayer("Player 1");
        game.AddPlayer("Player 2");
        game.Play();

        IPlayer player = game.GetCurrentPlayer();

        game._cardInHand[player].Clear();

        // Act
        List<Guid> playableCards = game.CheckPlayableCard(player);

        // Assert
        Assert.That(playableCards, Is.Empty);
    }

    #endregion

    [Test]
    public void PlayCard_Reverse_ChangesDirection()
    {
        // Arrange
        Game game = new Game(NullLogger<Game>.Instance);

        game.AddPlayer("Player 1");
        game.AddPlayer("Player 2");

        IPlayer player = game.GetCurrentPlayer();

        game._cardInHand[player] = new List<ICard>();

        ICard reverseCard = new Card(Color.Red, CardValue.Reverse);

        game._cardInHand[player].Add(reverseCard);

        game.DiscardCard(new Card(Color.Red, CardValue.Five));

        // Act
        game.PlayCard(reverseCard);

        // Assert
        Assert.That(
            game._gameDirection,
            Is.EqualTo(GameDirection.CounterClockwise)
        );
    }


    [Test]
    public void PlayCard_Reverse_WhenCounterClockwise_ChangesToClockwise()
    {
        // Arrangez
        Game game = new Game(NullLogger<Game>.Instance);

        game.AddPlayer("Player 1");
        game.AddPlayer("Player 2");

        IPlayer player = game.GetCurrentPlayer();

        game._cardInHand[player] = new List<ICard>();

        ICard firstReverse = new Card(Color.Red, CardValue.Reverse);

        game._cardInHand[player].Add(firstReverse);
        game.DiscardCard(new Card(Color.Blue, CardValue.Five));

        // Act
        game.PlayCard(firstReverse);

        // Add another Reverse
        ICard secondReverse = new Card(Color.Green, CardValue.Reverse);

        game._cardInHand[player].Add(secondReverse);

        game.PlayCard(secondReverse);

        // Assert
        Assert.That(
            game._gameDirection,
            Is.EqualTo(GameDirection.Clockwise)
        );
    }





    [Test]
    public void PlayCard_Skip_SetsTurnSkipped()
    {
        // Arrange
        Game game = new Game(NullLogger<Game>.Instance);

        game.AddPlayer("Player 1");
        game.AddPlayer("Player 2");

        IPlayer player = game.GetCurrentPlayer();

        game._cardInHand[player] = new List<ICard>();

        ICard skipCard = new Card(Color.Red, CardValue.Skip);

        game._cardInHand[player].Add(skipCard);

        game.DiscardCard(new Card(Color.Red, CardValue.Five));

        // Act
        game.PlayCard(skipCard);

        // Assert
        Assert.That(game._turnSkipped, Is.True);
    }

    [Test]
    public void PlayCard_PlusTwo_AddsTwoToPendingDraw()
    {
        // Arrange
        Game game = new Game(NullLogger<Game>.Instance);

        game.AddPlayer("Player 1");
        game.AddPlayer("Player 2");

        IPlayer player = game.GetCurrentPlayer();

        game._cardInHand[player] = new List<ICard>();

        ICard plusTwo = new Card(Color.Red, CardValue.PlusTwo);

        game._cardInHand[player].Add(plusTwo);

        game.DiscardCard(new Card(Color.Red, CardValue.Five));

        // Act
        game.PlayCard(plusTwo);

        // Assert
        Assert.That(game.GetLastDrawPenalty(), Is.EqualTo(2));
    }


    [Test]
    public void PlayCard_PlusFour_AddsFourToPendingDraw_AndWaitsForColor()
    {
        // Arrange
        Game game = new Game(NullLogger<Game>.Instance);

        game.AddPlayer("Player 1");
        game.AddPlayer("Player 2");

        IPlayer player = game.GetCurrentPlayer();

        game._cardInHand[player] = new List<ICard>();

        ICard plusFour = new Card(Color.Wild, CardValue.PlusFour);

        game._cardInHand[player].Add(plusFour);

        game.DiscardCard(new Card(Color.Red, CardValue.Five));

        // Act
        game.PlayCard(plusFour);

        // Assert
        Assert.That(game.GetLastDrawPenalty(), Is.EqualTo(4));
        Assert.That(game.WaitingForColor(), Is.True);
    }



    [Test]
    public void PlayCard_Wild_WaitsForColor()
    {
        // Arrange
        Game game = new Game(NullLogger<Game>.Instance);

        game.AddPlayer("Player 1");
        game.AddPlayer("Player 2");

        IPlayer player = game.GetCurrentPlayer();

        game._cardInHand[player] = new List<ICard>();

        ICard wildCard = new Card(Color.Wild, CardValue.Wild);

        game._cardInHand[player].Add(wildCard);

        game.DiscardCard(new Card(Color.Red, CardValue.Five));

        // Act
        game.PlayCard(wildCard);

        // Assert
        Assert.That(game.WaitingForColor(), Is.True);
    }

}
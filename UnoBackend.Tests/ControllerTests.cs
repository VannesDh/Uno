

using Microsoft.AspNetCore.Mvc;
using Moq;
using UnoBackend.Controller;
using UnoBackend.DTOs;
using UnoBackend.Interfaces;
using UnoBackend.Models.Enum;

namespace UnoBackend.Tests;

public class ControllerTests
{

    [Test]
    public void Reset_ReturnsOK()
    {
        Mock<IGame> game = new Mock<IGame>();
        GameController controller = new GameController(game.Object);

        IActionResult result = controller.Reset();

        Assert.That(result, Is.TypeOf<OkResult>());
        game.Verify(g => g.ResetGame(), Times.Once);
    }


    [Test]
public void AddPlayer_ReturnsOk()
{
    Mock<IGame> game = new Mock<IGame>();
    GameController controller = new GameController(game.Object);

    PlayerDto player = new PlayerDto
    {
        PlayerName = "Player 1"
    };

    IActionResult result = controller.AddPlayer(player);

    Assert.That(result, Is.TypeOf<OkResult>());
    game.Verify(g => g.AddPlayer("Player 1"), Times.Once);
}


[Test]
public void ChooseColor_InvalidColor_ReturnsBadRequest()
{
    Mock<IGame> game = new Mock<IGame>();
    GameController controller = new GameController(game.Object);

    IActionResult result = controller.ChooseColor("Banana");

    Assert.That(result, Is.TypeOf<BadRequestObjectResult>());

    game.Verify(
        g => g.ChooseColor(It.IsAny<Color>()),
        Times.Never
    );
}

[Test]
public void ChooseColor_ValidColor_ReturnsOk()
{
    Mock<IGame> game = new Mock<IGame>();
    GameController controller = new GameController(game.Object);

    IActionResult result = controller.ChooseColor("Red");

    Assert.That(result, Is.TypeOf<OkResult>());

    game.Verify(
        g => g.ChooseColor(Color.Red),
        Times.Once
    );
}

[Test]
public void Draw_ReturnsDrawnCard()
{
    Mock<IGame> game = new Mock<IGame>();
    GameController controller = new GameController(game.Object);

    Mock<IPlayer> player = new Mock<IPlayer>();
    Mock<ICard> card = new Mock<ICard>();

    Guid cardId = Guid.NewGuid();

    player.SetupGet(p => p.Name).Returns("Player 1");

    card.SetupGet(c => c.Id).Returns(cardId);
    card.SetupGet(c => c.Color).Returns(Color.Red);
    card.SetupGet(c => c.CardValue).Returns(CardValue.Five);

    game.Setup(g => g.GetCurrentPlayer())
        .Returns(player.Object);

    game.Setup(g => g.DrawCard(player.Object, false))
        .Returns(card.Object);

    game.Setup(g => g.GetDeckCount())
        .Returns(50);

    IActionResult result = controller.Draw();

    Assert.That(result, Is.TypeOf<OkObjectResult>());

    OkObjectResult okResult = (OkObjectResult)result;

    DrawResponseDto response = (DrawResponseDto)okResult.Value!;

    Assert.That(response.Card.Id, Is.EqualTo(cardId));
    Assert.That(response.Card.Color, Is.EqualTo("Red"));
    Assert.That(response.Card.Value, Is.EqualTo("Five"));
    Assert.That(response.DeckCount, Is.EqualTo(50));

    game.Verify(g => g.DrawCard(player.Object, false), Times.Once);
}

[Test]
public void PlayCard_CardNotInHand_ReturnsBadRequest()
{
    Mock<IGame> game = new Mock<IGame>();
    GameController controller = new GameController(game.Object);

    Mock<IPlayer> player = new Mock<IPlayer>();

    game.Setup(g => g.GetCurrentPlayer())
        .Returns(player.Object);

    game.Setup(g => g.GetPlayerCard(player.Object))
        .Returns(new List<ICard>());

    CardDto card = new CardDto
    {
        Id = Guid.NewGuid()
    };

    IActionResult result = controller.PlayCard(card);

    Assert.That(result, Is.TypeOf<BadRequestObjectResult>());

    game.Verify(
        g => g.PlayCard(It.IsAny<ICard>()),
        Times.Never
    );
}
[Test]
public void PlayCard_UnplayableCard_ReturnsBadRequest()
{
    Mock<IGame> game = new Mock<IGame>();
    GameController controller = new GameController(game.Object);

    Mock<IPlayer> player = new Mock<IPlayer>();
    Mock<ICard> card = new Mock<ICard>();

    Guid cardId = Guid.NewGuid();

    card.SetupGet(c => c.Id).Returns(cardId);

    game.Setup(g => g.GetCurrentPlayer())
        .Returns(player.Object);

    game.Setup(g => g.GetPlayerCard(player.Object))
        .Returns(new List<ICard> { card.Object });

    game.Setup(g => g.CheckCardPlayability(card.Object))
        .Returns(false);

    CardDto request = new CardDto
    {
        Id = cardId
    };

    IActionResult result = controller.PlayCard(request);

    Assert.That(result, Is.TypeOf<BadRequestObjectResult>());

    game.Verify(
        g => g.PlayCard(It.IsAny<ICard>()),
        Times.Never
    );
}
}
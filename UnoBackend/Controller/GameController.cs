using Microsoft.AspNetCore.Mvc;
using UnoBackend.DTOs;
using UnoBackend.Interfaces;
using UnoBackend.Models.Enum;
using UnoBackend.Services;
using Color = UnoBackend.Models.Enum.Color;

namespace UnoBackend.Controller;

[ApiController]
[Route("api/game")]
public class GameController : ControllerBase
{
    private readonly Game _game;

    public GameController(Game game)
    {
        _game = game;
    }


    [HttpGet("play")]
    public IActionResult Play()
    {
        _game.Play();

        DeckDto deck = new()
        {
            CardCount = _game.GetDeckCount()
        };

        IPlayer player = _game.GetCurrentPlayer();

        PlayerDto playerDto = new()
        {
            PlayerName = player.Name
        };

        List<CardDto> playerCards = _game.GetPlayerCard(player)
            .Select(card => new CardDto
            {
                Id = card.Id,
                Color = card.Color.ToString(),
                Value = card.CardValue.ToString()
            })
            .ToList();

        HandDto playerHand = new()
        {
            Cards = playerCards
        };

        ICard card = _game.GetCurrentTopPile();

        DiscardPileDto discardPile = new()
        {
            LastCardInDiscardPile = new CardDto
            {
                Id = card.Id,
                Color = card.Color.ToString(),
                Value = card.CardValue.ToString()
            }
        };

        InitialDataDto initialData = new()
        {
            DeckCount = deck,
            Hand = playerHand,
            DiscardPile = discardPile,
            Player = playerDto,
            WaitingForColor = _game.WaitingForColor()
        };

        return Ok(initialData);
    }


    [HttpPost("draw")]
    public IActionResult Draw()
    {
        IPlayer player = _game.GetCurrentPlayer();

        ICard card = _game.DrawCard(player);

        CardDto drawnCard = new()
        {
            Id = card.Id,
            Color = card.Color.ToString(),
            Value = card.CardValue.ToString()
        };

        return Ok(new DrawResponseDto
        {
            Card = drawnCard,
            DeckCount = _game.GetDeckCount()
        });
    }


    [HttpGet("checkPlayability")]
    public IActionResult CheckPlayerCardPlayability()
    {
        IPlayer player = _game.GetCurrentPlayer();

        List<Guid> listOfIds = _game.CheckPlayableCard(player);

        return Ok(listOfIds);
    }


    [HttpPost("playCard")]
    public IActionResult PlayCard([FromBody] CardDto card)
    {
        IPlayer player = _game.GetCurrentPlayer();

        ICard? actualCard = _game.GetPlayerCard(player)
            .FirstOrDefault(c => c.Id == card.Id);

        if (actualCard == null)
        {
            return BadRequest("Card is not in player's hand.");
        }

        if (!_game.CheckCardPlayability(actualCard))
        {
            return BadRequest("Card cannot be played.");
        }

        // PlayCard now uses the winner callback
        _game.PlayCard(actualCard);

        // Get winner from the Game
        IPlayer? winner = _game.GetWinner();

        Console.WriteLine(
            winner != null
                ? $"The winner is {winner.Name}"
                : "There is no winner yet"
        );

        List<CardDto> playerCards = _game.GetPlayerCard(player)
            .Select(card => new CardDto
            {
                Id = card.Id,
                Color = card.Color.ToString(),
                Value = card.CardValue.ToString()
            })
            .ToList();

        ICard topCard = _game.GetCurrentTopPile();

        return Ok(new PlayCardResponseDto
        {
            GameWinner = winner != null,

            Hand = new HandDto
            {
                Cards = playerCards
            },

            DiscardPile = new DiscardPileDto
            {
                LastCardInDiscardPile = new CardDto
                {
                    Id = topCard.Id,
                    Color = topCard.Color.ToString(),
                    Value = topCard.CardValue.ToString()
                }
            }
        });
    }


    [HttpPost("chooseColor")]
    public IActionResult ChooseColor([FromBody] string color)
    {
        if (!Enum.TryParse<Color>(color, true, out Color chosenColor))
        {
            return BadRequest("Invalid color.");
        }

        _game.ChooseColor(chosenColor);

        return Ok();
    }


    [HttpPost("addPlayer")]
    public IActionResult AddPlayer([FromBody] PlayerDto player)
    {
        _game.AddPlayer(player.PlayerName);

        return Ok();
    }


    [HttpPost("restart")]
    public IActionResult RestartGame()
    {
        _game.ResetGame();

        _game.Play();

        DeckDto deck = new()
        {
            CardCount = _game.GetDeckCount()
        };

        IPlayer player = _game.GetCurrentPlayer();

        PlayerDto playerDto = new()
        {
            PlayerName = player.Name
        };

        List<CardDto> playerCards = _game.GetPlayerCard(player)
            .Select(card => new CardDto
            {
                Id = card.Id,
                Color = card.Color.ToString(),
                Value = card.CardValue.ToString()
            })
            .ToList();

        HandDto playerHand = new()
        {
            Cards = playerCards
        };

        ICard card = _game.GetCurrentTopPile();

        DiscardPileDto discardPile = new()
        {
            LastCardInDiscardPile = new CardDto
            {
                Id = card.Id,
                Color = card.Color.ToString(),
                Value = card.CardValue.ToString()
            }
        };

        InitialDataDto initialData = new()
        {
            DeckCount = deck,
            Hand = playerHand,
            DiscardPile = discardPile,
            Player = playerDto,
            WaitingForColor = _game.WaitingForColor()
        };

        return Ok(initialData);
    }


    [HttpPost("endTurn")]
    public IActionResult EndTurn()
    {
        _game.EndTurn();

        IPlayer player = _game.GetCurrentPlayer();

        PlayerDto current = new()
        {
            PlayerName = player.Name
        };

        List<CardDto> playerCards = _game.GetPlayerCard(player)
            .Select(card => new CardDto
            {
                Id = card.Id,
                Color = card.Color.ToString(),
                Value = card.CardValue.ToString()
            })
            .ToList();

        HandDto handDto = new()
        {
            Cards = playerCards
        };

        return Ok(new EndDto
        {
            Player = current,
            Hand = handDto
        });
    }


    [HttpPost("callUno")]
    public IActionResult CallUno()
    {
        IPlayer player = _game.GetCurrentPlayer();

        _game.CallUno(player);

        return Ok();
    }
}
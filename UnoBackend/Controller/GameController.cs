using System.Drawing;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Mvc;
using UnoBackend.DTOs;
using UnoBackend.Interfaces;
using UnoBackend.Models;
using UnoBackend.Services;

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
        PlayerDto playerDto = new();
        playerDto.PlayerName = player.Name;

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
            LastCardInDiscardPile = new()
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
            Player = playerDto
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

        _game.PlayCard(actualCard);

        IPlayer currentPlayer = _game.GetCurrentPlayer();

        List<CardDto> playerCards = _game.GetPlayerCard(currentPlayer)
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

}
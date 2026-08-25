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
            Color = card.Color.ToString(),
            Value = card.CardValue.ToString()
        })
        .ToList();

        HandDto playerHand = new()
        {
            Cards = playerCards
        };

        ICard card =  _game.GetCurrentTopPile();

        DiscardPileDto discardPile = new()
        {
          LastCardInDiscardPile = new()
          {
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

    public IActionResult Draw()
    {

        
        return Ok();
    }
}
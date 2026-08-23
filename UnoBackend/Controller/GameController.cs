using Microsoft.AspNetCore.Mvc;
using UnoBackend.DTOs;
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
            Cards = _game.GetDeck()
                .Select(card => new CardDto
                {
                    Color = card.Color.ToString(),
                    Value = card.CardValue.ToString()
                })
                .ToList()
        };
        
        return Ok(deck);
    }

}
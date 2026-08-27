using System.Drawing;
using UnoBackend.Models;
using UnoBackend.Models.Enum;

namespace UnoBackend.DTOs;


public class InitialDataDto
{
    public DeckDto DeckCount { get; set; } = new();
    public HandDto Hand { get; set; } = new();
    public PlayerDto Player { get; set; } = new();
    public DiscardPileDto DiscardPile { get; set; } = new();
    public bool WaitingForColor { get; set; }
}

public class DiscardPileDto
{
    public CardDto LastCardInDiscardPile { get; set; }
}

public class DeckDto
{
    public int CardCount { get; set; }
}

public class PlayerDto
{
    // public int PlayerId { get; set; } = default;
    public string PlayerName { get; set; } = "";
}

public class CardDto
{
    public Guid Id { get; set; }
    public string Color { get; set; }
    public string Value { get; set; }
}

public class HandDto
{
    public List<CardDto> Cards { get; set; } = [];
}

public class PlayCardResponseDto
{
    public HandDto Hand { get; set; }
    public DiscardPileDto DiscardPile { get; set; }
    public bool GameWinner{get;set;}
}

public class DrawResponseDto
{
    public CardDto Card { get; set; } = null!;
    public int DeckCount { get; set; }
}

public class EndDto
{
    public PlayerDto? Player {get; set;} = null;
    public HandDto Hand {get; set;}
}
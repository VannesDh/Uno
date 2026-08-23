namespace UnoBackend.DTOs;

public class DeckDto
{
    public List<CardDto> Cards { get; set; } = [];
}

public class CardDto
{
    public string Color { get; set; } = "";
    public string Value { get; set; } = "";
}

public class HandDto
{
    public List<CardDto> Cards { get; set; } = [];
}
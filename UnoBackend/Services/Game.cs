using UnoBackend.Interfaces;
using UnoBackend.Models;
using UnoBackend.Models.Enum;

namespace UnoBackend.Services;
public class Game{
    private List<IPlayer> _players = [];
    private IDeck _deck;
    private IDiscarded _discardedPile = new Discarded();
    private GameDirection _gameDirection = GameDirection.Clockwise; 
    private int _currentPlayerIndex;
    private bool _turnSkipped = false;
    private Dictionary<IPlayer,bool> _callUno;
    public Game()
    {
        _deck = new Deck(InitializeCards());
        Console.WriteLine(_deck);
    }
    // Helper to init cards on first time
    private Stack<ICard> InitializeCards()
    {
        Stack<ICard> _filledDeck = new();

        foreach(Color color in Enum.GetValues<Color>())
        {
            foreach(CardValue value in Enum.GetValues<CardValue>())
            {
                if ((color == Color.Wild && value != CardValue.Wild && value != CardValue.PlusFour)
                ||(color != Color.Wild && (value == CardValue.Wild || value == CardValue.PlusFour)))
                {
                    continue;
                }
                _filledDeck.Append(new Card(color,value));
            }
        }

        return _filledDeck;   
    }

    public void Play()
    {
        
    }

    public void AddPlayer(string name)
    {
        Player newPlayer = new(name);
        _players.Add(newPlayer);
        _callUno.Add(newPlayer,false);
    }

    
    
}
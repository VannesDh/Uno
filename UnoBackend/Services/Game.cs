using System.Collections;
using UnoBackend.Interfaces;
using UnoBackend.Models;
using UnoBackend.Models.Enum;

namespace UnoBackend.Services;
public class Game{

    private IPlayer _testPlayer1 = new Player("Josh");
    private IPlayer _testPlayer2 = new Player("Kanna");
    private IPlayer _testPlayer3 = new Player("Ai");
    private List<IPlayer> _players = [];
    private Dictionary<IPlayer,List<ICard>>  _cardInHand = new();
    private IDeck _deck;
    private IDiscarded _discardedPile = new Discarded();
    private GameDirection _gameDirection = GameDirection.Clockwise; 
    private int _currentPlayerIndex;
    private bool _turnSkipped = false;
    private Dictionary<IPlayer,bool> _callUno = new();
    public Game()
    {
        _deck = new Deck(InitializeCards());
        _players = [_testPlayer1, _testPlayer2, _testPlayer3];
        _callUno.Add(_testPlayer1,false);
        _callUno.Add(_testPlayer2,false);
        _callUno.Add(_testPlayer3,false);

        _currentPlayerIndex = 0;  
    }
    public void Play()
    {
        if(_players.Count < 2)
        {
            Console.WriteLine("Cannot la bro");
        }
        else
        {
            DistributeCards();

            while(_discardedPile.DiscardedCards.Count == 0)
            {
                CheckPlayedCard(_deck.DeckPiles.Pop());
            }    
        }

        while (true)
        {
            
        }

    }

    #region Discarding, Power Related, and Logic Checking

    public void CheckPlayedCard(ICard card)
    {
        if(card.CardValue == CardValue.PlusFour && _discardedPile.DiscardedCards.Count == 0)
        {
            _deck.DeckPiles.Push(card);
            Shuffle(_deck.DeckPiles.ToList());
            return;
        }
        else if(card.CardValue == CardValue.Skip
            || card.CardValue == CardValue.Reverse
            || card.CardValue == CardValue.Wild
            || card.CardValue == CardValue.PlusFour
            || card.CardValue == CardValue.PlusTwo)
        {
            SpecialCardPlayed(card);
        }
        else
        {
            _discardedPile.DiscardedCards.Push(_deck.DeckPiles.Pop());
        }
    }

    public void SpecialCardPlayed(ICard card)
    {
        if(card.CardValue == CardValue.Reverse)
        {
            if(_gameDirection == GameDirection.Clockwise)
            {
                _gameDirection = GameDirection.CounterClockwise;
                return;
            }
            _gameDirection = GameDirection.Clockwise;
            return;
        }
    }
    public ICard GetCurrentTopPile()
    {
        return _discardedPile.DiscardedCards.Peek();
    }
    #endregion

    #region Player Related

    public void AddPlayer(string name)
    {
        Player newPlayer = new(name);
        _players.Add(newPlayer);
        _callUno.Add(newPlayer,false);
    }

    #endregion

    #region Cards Related
     // Helper to init cards on first time
    private Stack<ICard> InitializeCards()
    {
        List<ICard> cards = new();

        foreach(Color color in Enum.GetValues<Color>())
        {
            foreach(CardValue value in Enum.GetValues<CardValue>())
            {
                // Color only
                if(color != Color.Wild && value != CardValue.PlusFour && value != CardValue.Wild)
                {
                    // We only have 1 Zero per color
                    if(value == CardValue.Zero)
                    {
                        cards.Add(new Card(color,value));
                    }
                    else
                    {
                        cards.Add(new Card(color,value));
                        cards.Add(new Card(color,value));
                    }
                }
                // For Wild color we need 4 of them per type
                else if(color == Color.Wild && (value == CardValue.Wild || value == CardValue.PlusFour))
                {
                    for (int i = 0; i < 4; i++)
                    {
                        cards.Add(new Card(color,value));
                    }
                }
            }
        }

        Shuffle(cards);
    
        Stack<ICard> filledDeck = new(cards);
        return filledDeck;   
    }
    private void Shuffle(List<ICard> cards)
    {
        for (int i = cards.Count - 1; i > 0; i--)
        {
            int j = Random.Shared.Next(i + 1);

            (cards[i], cards[j]) = (cards[j], cards[i]);
        }
    }

    private void DistributeCards()
    {
        int initialDraw = 7;
        for (int i = 0; i < _players.Count(); i++)
        {
            IPlayer player = _players[i]; 
            List<ICard> initialCard = []; 
            for (int j = 0; j < initialDraw ; j++)
            {
                ICard currCard = _deck.DeckPiles.Pop();
                initialCard.Add(new Card(currCard.Color,currCard.CardValue));
            }
            _cardInHand.Add(player,initialCard);
        }
    }

    
    #endregion
}
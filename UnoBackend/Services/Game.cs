using System.Collections;
using System.Security.Cryptography;
using UnoBackend.Interfaces;
using UnoBackend.Models;
using UnoBackend.Models.Enum;

namespace UnoBackend.Services;
public class Game{

    private IPlayer _testPlayer1 = new Player("Josh");
    private IPlayer _testPlayer2 = new Player("Kanna");
    private IPlayer _testPlayer3 = new Player("Ai");
    // additional for the plus  
    private int _pendingDraw = 0;
    private Color? _chosenColor = null;
    private bool _waitingForColor = false;
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
    }


    #region Discarding, Power Related, and Logic Checking

    public void PlayCard(ICard card)
    {

        if (CheckIfWinner(_players[_currentPlayerIndex]))
        {
            // Win
        }
        CheckPlayedCard(card);
        DiscardCard(card);

        NextPlayer(_turnSkipped);
        
    }

    public bool CheckIfWinner(IPlayer player)
    {
        if(_cardInHand[player].Count == 0)
        {
            return true;
        }
        return false;
    }
    public void NextPlayer(bool skipped)
    {
        if (skipped)
        {
            _currentPlayerIndex = (_currentPlayerIndex + (int)_gameDirection) % _players.Count;
            _turnSkipped = false;
            PlayerTurn(_players[_currentPlayerIndex]);
            return;
        }
        _currentPlayerIndex = (_currentPlayerIndex + (int)_gameDirection) % _players.Count;
        PlayerTurn(_players[_currentPlayerIndex]);

    }
    public List<int> CheckPlayableCard(IPlayer player)
    {
        List<ICard> cards = _cardInHand[player];
        List<int> playableCards = [];
        for (int i = 0; i < cards.Count; i++)
        {
            if (CheckCardPlayability(cards[i]))
            {
                playableCards.Add(i);
            }
        }
        return playableCards;
    }

    private bool CheckCardPlayability(ICard card)
    {
        ICard topPile = GetCurrentTopPile();
        if(_chosenColor.HasValue && card.Color == _chosenColor)
        {
            return true;
        }
        if(card.CardValue == topPile.CardValue || card.Color == topPile.Color || card.Color == Color.Wild)
        {
            return true;
        }
        return false;   
    }

    public void PlayerTurn(IPlayer player)
    {   if(IsUno(player))

        if(_pendingDraw != 0)
        {
            for (int i = 0; i < _pendingDraw; i++)
            {
                DrawCard(player);   
            }
            _pendingDraw = 0;
        }
    }
 
    public void DiscardCard(ICard card)
    {
        _discardedPile.DiscardedCards.Push(card);
    }
    private void CheckPlayedCard(ICard card)
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
        
        _discardedPile.DiscardedCards.Push(_deck.DeckPiles.Pop());
        
    }
    private void SpecialCardPlayed(ICard card)
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

        if(card.CardValue == CardValue.Skip)
        {
            _turnSkipped = true;
            return;
        }

        if(card.CardValue == CardValue.PlusTwo)
        {
            _pendingDraw = 2;
            return;
        }

        if(card.CardValue == CardValue.PlusFour)
        {  
            _pendingDraw = 4;
            _waitingForColor = true;
            
        }
        if(card.CardValue == CardValue.Wild)
        {
            _waitingForColor = true;
        }
        if (_waitingForColor)
            {
                // Later add SignalR so that we can send API to the frontend to make them choose colour
                // SendToFrontend(new
                // {
                //     type = "choose_colour",
                //     options = new[]
                //     {
                //         CardColour.Red,
                //         CardColour.Green,
                //         CardColour.Blue,
                //         CardColour.Yellow
                //     }
                // });
            }
    }
    private ICard GetCurrentTopPile()
    {
        return _discardedPile.DiscardedCards.Peek();
    }

    public bool IsUno(IPlayer player)
    {
        if (_cardInHand[player].Count == 1)
        {
            return true;
        }
        return false;
    }

    public void CallUno(IPlayer player)
    {
        _callUno[player] = true;
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
    public void RenewDeck()
    {
        List<ICard> cards = _discardedPile.DiscardedCards
                            .Take(_discardedPile.DiscardedCards.Count - 1)
                            .ToList();
                            
        Shuffle(cards);
        _discardedPile.DiscardedCards = new();
        _deck.DeckPiles = new Stack<ICard>(cards);
    }

    public void DrawCard(IPlayer player)
    {
        _cardInHand[player].Add(_deck.DeckPiles.Pop());
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
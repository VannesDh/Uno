using UnoBackend.Interfaces;
using UnoBackend.Models;
using UnoBackend.Models.Enum;

namespace UnoBackend.Services;

public class Game
{
    // Winner callback delegate
    public delegate void WinnerCallback(IPlayer player);
    private WinnerCallback? _winnerCallback;

    private int _pendingDraw = 0;
    private ICard? _drawnCard = null;
    private bool _hasStart = false;
    private Color? _chosenColor = null;
    private bool _waitingForColor = false;

    private List<IPlayer> _players = [];
    private Dictionary<IPlayer, List<ICard>> _cardInHand = new();

    private IDeck _deck;
    private IDiscarded _discardedPile = new Discarded();

    private GameDirection _gameDirection = GameDirection.Clockwise;
    private int _currentPlayerIndex;
    private bool _turnSkipped = false;

    private Dictionary<IPlayer, bool> _callUno = new();

    // Stores the winner after the callback is triggered
    private IPlayer? _winner;


    public Game()
    {
        _deck = new Deck(InitializeCards());
        _currentPlayerIndex = 0;

        SetWinnerCallback(OnWinner);
    }


    #region Game

    public void Play()
    {
        if (_players.Count < 2)
        {
            Console.WriteLine("Cannot start game");
            return;
        }

        if (!_hasStart)
        {
            DistributeCards();
            _hasStart = true;
        }

        while (_discardedPile.DiscardedCards.Count == 0)
        {
            ICard card = _deck.DeckPiles.Pop();

            // Wild Draw Four cannot be the starting card
            if (card.CardValue == CardValue.PlusFour)
            {
                _deck.DeckPiles.Push(card);

                List<ICard> cards = _deck.DeckPiles.ToList();

                Shuffle(cards);

                _deck.DeckPiles = new Stack<ICard>(cards);

                continue;
            }

            // Put the card into discard pile
            DiscardCard(card);

            // Apply starting card effect
            if (card.CardValue == CardValue.Reverse ||
                card.CardValue == CardValue.Skip ||
                card.CardValue == CardValue.PlusTwo)
            {
                SpecialCardPlayed(card);

                if (_turnSkipped)
                {
                    NextPlayer(_turnSkipped);
                }

                for (int i = 0; i < _pendingDraw; i++)
                {
                    DrawCard(_players[_currentPlayerIndex]);
                }

                _pendingDraw = 0;
            }

            // Wild → first player chooses the color
            if (card.CardValue == CardValue.Wild)
            {
                _waitingForColor = true;
                return;
            }
        }
    }


    public void RestartGame()
    {
        // Reset deck
        _deck = new Deck(InitializeCards());

        // Reset discard pile
        _discardedPile = new Discarded();

        // Reset player hands
        _cardInHand.Clear();

        // Reset UNO states
        _callUno.Clear();

        foreach (IPlayer player in _players)
        {
            _callUno[player] = false;
        }

        // Reset game state
        _pendingDraw = 0;
        _drawnCard = null;
        _hasStart = false;
        _chosenColor = null;
        _waitingForColor = false;
        _currentPlayerIndex = 0;
        _gameDirection = GameDirection.Clockwise;
        _turnSkipped = false;

        // Reset winner
        _winner = null;
    } 
    public void ResetGame()
       {
        // Reset deck
        _deck = new Deck(InitializeCards());

        // Reset discard pile
        _discardedPile = new Discarded();

        // Reset player hands
        _cardInHand.Clear();

        // Reset UNO states
        _callUno.Clear();

        _players = new();
        
        // Reset game state
        _pendingDraw = 0;
        _drawnCard = null;
        _hasStart = false;
        _chosenColor = null;
        _waitingForColor = false;
        _currentPlayerIndex = 0;
        _gameDirection = GameDirection.Clockwise;
        _turnSkipped = false;

        // Reset winner
        _winner = null;
    } 


    #endregion


    #region Winner Callback

    public void SetWinnerCallback(WinnerCallback callback)
    {
        _winnerCallback = callback;
    }


    private void OnWinner(IPlayer player)
    {
        _winner = player;

        Console.WriteLine($"WINNER: {player.Name}");
    }


    public IPlayer? GetWinner()
    {
        return _winner;
    }


    #endregion


    #region Discarding, Power Related, and Logic Checking

    public void PlayCard(ICard card)
    {
        if (!CheckCardPlayability(card))
            return;

        IPlayer player = _players[_currentPlayerIndex];

        CheckPlayedCard(card);

        _cardInHand[player].Remove(card);

        // The forced drawn card has now been played
        _drawnCard = null;

        if (!_waitingForColor)
        {
            _chosenColor = null;
        }

        // Check winner
        if (CheckIfWinner(player))
        {
            _winnerCallback?.Invoke(player);
        }
    }


    public bool CheckIfWinner(IPlayer player)
    {
        return _cardInHand[player].Count == 0;
    }


    public void NextPlayer(bool skipped)
    {
        int steps = skipped ? 2 : 1;

        int direction = (int)_gameDirection;

        _currentPlayerIndex =
            (_currentPlayerIndex + direction * steps + _players.Count)
            % _players.Count;

        _turnSkipped = false;

        PlayerTurn(_players[_currentPlayerIndex]);
    }


    public List<Guid> CheckPlayableCard(IPlayer player)
    {
        List<ICard> cards = _cardInHand[player];

        List<Guid> playableCards = [];

        foreach (ICard card in cards)
        {
            if (CheckCardPlayability(card))
            {
                playableCards.Add(card.Id);
            }
        }

        return playableCards;
    }


    public bool CheckCardPlayability(ICard card)
    {
        // If the player drew a playable card,
        // that card is the only card they can play.
        if (_drawnCard != null && card.Id != _drawnCard.Id)
        {
            return false;
        }

        ICard topPile = GetCurrentTopPile();

        if (_chosenColor.HasValue &&
            card.Color == _chosenColor)
        {
            return true;
        }

        if (card.CardValue == topPile.CardValue ||
            card.Color == topPile.Color ||
            card.Color == Color.Wild)
        {
            return true;
        }

        return false;
    }


    public void PlayerTurn(IPlayer player)
    {
        _drawnCard = null;
        if (!IsUno(player))
        {
            if (_callUno[player])
            {
                DrawCard(player);
                _callUno[player] = false;
            }
        }
        else if (IsUno(player))
        {
            if (!_callUno[player])
            {
                DrawCard(player);
                _callUno[player] = false;
            }
        }

        // Pending +2 / +4 draw
        if (_pendingDraw != 0)
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
        if (card.CardValue == CardValue.Skip
            || card.CardValue == CardValue.Reverse
            || card.CardValue == CardValue.Wild
            || card.CardValue == CardValue.PlusFour
            || card.CardValue == CardValue.PlusTwo)
        {
            SpecialCardPlayed(card);
        }

        DiscardCard(card);
    }


    private void SpecialCardPlayed(ICard card)
    {
        if (card.CardValue == CardValue.Reverse)
        {
            if (_gameDirection == GameDirection.Clockwise)
            {
                _gameDirection = GameDirection.CounterClockwise;
                return;
            }

            _gameDirection = GameDirection.Clockwise;
            return;
        }


        if (card.CardValue == CardValue.Skip)
        {
            _turnSkipped = true;
            return;
        }


        if (card.CardValue == CardValue.PlusTwo)
        {
            _pendingDraw = 2;
            return;
        }


        if (card.CardValue == CardValue.PlusFour)
        {
            _pendingDraw = 4;
            _waitingForColor = true;
        }


        if (card.CardValue == CardValue.Wild)
        {
            _waitingForColor = true;
        }
    }


    public bool WaitingForColor()
    {
        return _waitingForColor;
    }


    public void ChooseColor(Color color)
    {
        if (!_waitingForColor)
            return;

        _chosenColor = color;

        _waitingForColor = false;
    }


    public void EndTurn()
    {
        NextPlayer(_turnSkipped);
    }


    public ICard GetCurrentTopPile()
    {
        return _discardedPile.DiscardedCards.Peek();
    }


    public bool IsUno(IPlayer player)
    {
        return _cardInHand[player].Count == 1;
    }


    public void CallUno(IPlayer player)
    {
        Console.WriteLine($"Player {player.Name} called UNO");

        _callUno[player] = true;
    }


    #endregion


    #region Player Related

    public void AddPlayer(string name)
    {
        Player newPlayer = new(name);

        _players.Add(newPlayer);

        _callUno.Add(newPlayer, false);

        Console.WriteLine($"Player Added {name}");
    }


    public List<ICard> GetPlayerCard(IPlayer player)
    {
        return _cardInHand[player].ToList();
    }


    public IPlayer GetCurrentPlayer()
    {
        IPlayer player = _players[_currentPlayerIndex];

        Console.WriteLine($"Current player: {player.Name}");
        Console.WriteLine($"Player object: {player.GetHashCode()}");

        return player;
    }


    #endregion


    #region Cards Related

    private Stack<ICard> InitializeCards()
    {
        List<ICard> cards = new();

        foreach (Color color in Enum.GetValues<Color>())
        {
            foreach (CardValue value in Enum.GetValues<CardValue>())
            {
                // Colored cards
                if (color != Color.Wild &&
                    value != CardValue.PlusFour &&
                    value != CardValue.Wild)
                {
                    // Only one Zero per color
                    if (value == CardValue.Zero)
                    {
                        cards.Add(new Card(color, value));
                    }
                    else
                    {
                        cards.Add(new Card(color, value));
                        cards.Add(new Card(color, value));
                    }
                }

                // Wild cards
                else if (color == Color.Wild &&
                         (value == CardValue.Wild ||
                          value == CardValue.PlusFour))
                {
                    for (int i = 0; i < 4; i++)
                    {
                        cards.Add(new Card(color, value));
                    }
                }
            }
        }

        Shuffle(cards);

        Stack<ICard> filledDeck = new(cards);

        return filledDeck;
    }


    public int GetDeckCount()
    {
        return _deck.DeckPiles.Count;
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
        if (_discardedPile.DiscardedCards.Count <= 1)
        {
            return;
        }

        // Keep the current top card
        ICard topCard = _discardedPile.DiscardedCards.Pop();

        // Everything else goes back into deck
        List<ICard> cards = _discardedPile.DiscardedCards.ToList();

        Shuffle(cards);

        // Reset discard pile
        _discardedPile.DiscardedCards = new Stack<ICard>();

        _discardedPile.DiscardedCards.Push(topCard);

        // Create new deck
        _deck.DeckPiles = new Stack<ICard>(cards);
    }


    public ICard DrawCard(IPlayer player)
    {
        if (_deck.DeckPiles.Count == 0)
        {
            RenewDeck();
        }

        ICard card = _deck.DeckPiles.Pop();

        _cardInHand[player].Add(card);

        // If the drawn card is playable,
        // this becomes the only card the player can play.
        if (CheckCardPlayability(card))
        {
            _drawnCard = card;
        }

        return card;
    }


    private void DistributeCards()
    {
        for (int i = 0; i < _players.Count; i++)
        {
            IPlayer player = _players[i];

            List<ICard> initialCard = [];

            for (int j = 0; j < 2; j++)
            {
                initialCard.Add(_deck.DeckPiles.Pop());
            }

            _cardInHand.Add(player, initialCard);
        }
    }


    #endregion
}
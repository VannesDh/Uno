using UnoBackend.Interfaces;
using UnoBackend.Models;
using UnoBackend.Models.Enum;

namespace UnoBackend.Services;

public class Game
{
    // additional for the plus  
    private int _pendingDraw = 0;
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

    public Game()
    {
        _deck = new Deck(InitializeCards());
        _currentPlayerIndex = 0;
    }
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
            // ICard card = new Card(Color.Wild, CardValue.Wild);
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

            // Apply its starting effect
            if (card.CardValue == CardValue.Reverse ||
                card.CardValue == CardValue.Skip ||
                card.CardValue == CardValue.PlusTwo)
            {
                SpecialCardPlayed(card);
            }

            // Wild → first player chooses the color
            if (card.CardValue == CardValue.Wild)
            {
                _waitingForColor = true;
                return;
            }
        }
    }


    #region Discarding, Power Related, and Logic Checking

    public bool PlayCard(ICard card)
    {
        if (CheckCardPlayability(card))
        {
            CheckPlayedCard(card);
            _cardInHand[_players[_currentPlayerIndex]].Remove(card);

            if (!_waitingForColor)
            {
                _chosenColor = null;
            }
        }

        if (CheckIfWinner(_players[_currentPlayerIndex]))
        {
            Console.WriteLine(_players[_currentPlayerIndex]);
            return true;
        }

        return false;
    }

    public bool CheckIfWinner(IPlayer player)
    {
        if (_cardInHand[player].Count == 0)
        {
            Console.Write("nguehehe");
            return true;
        }
        return false;
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
        ICard topPile = GetCurrentTopPile();
        if (_chosenColor.HasValue && card.Color == _chosenColor)
        {
            return true;
        }
        if (card.CardValue == topPile.CardValue || card.Color == topPile.Color || card.Color == Color.Wild)
        {
            return true;
        }
        return false;
    }

    public void PlayerTurn(IPlayer player)
    {
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
        if (_cardInHand[player].Count == 1)
        {
            return true;
        }
        return false;
    }

    public void CallUno(IPlayer player)
    {
        Console.WriteLine($"Player {player} called UNO");
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
    // Helper to init cards on first time
    private Stack<ICard> InitializeCards()
    {
        List<ICard> cards = new();

        foreach (Color color in Enum.GetValues<Color>())
        {
            foreach (CardValue value in Enum.GetValues<CardValue>())
            {
                // Color only
                if (color != Color.Wild && value != CardValue.PlusFour && value != CardValue.Wild)
                {
                    // We only have 1 Zero per color
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
                // For Wild color we need 4 of them per type
                else if (color == Color.Wild && (value == CardValue.Wild || value == CardValue.PlusFour))
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

        // The current top card stays in the discard pile
        ICard topCard = _discardedPile.DiscardedCards.Pop();

        // Everything else goes back into the deck
        List<ICard> cards = _discardedPile.DiscardedCards.ToList();

        // Shuffle
        Shuffle(cards);

        // Restore the top card
        _discardedPile.DiscardedCards = new Stack<ICard>();
        _discardedPile.DiscardedCards.Push(topCard);

        // New draw deck
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
        return card;
    }
    private void DistributeCards()
    {
        for (int i = 0; i < _players.Count; i++)
        {
            IPlayer player = _players[i];
            List<ICard> initialCard = [];

            for (int j = 0; j < 7; j++)
            {
                initialCard.Add(_deck.DeckPiles.Pop());
            }

            _cardInHand.Add(player, initialCard);
        }
    }
    #endregion
}
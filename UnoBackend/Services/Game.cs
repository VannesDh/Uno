using Serilog;
using UnoBackend.Interfaces;
using UnoBackend.Models;
using UnoBackend.Models.Enum;
using Microsoft.Extensions.Logging;

namespace UnoBackend.Services;

public class Game
{
    public delegate void WinnerCallback(IPlayer player);
    private WinnerCallback? _winnerCallback;

    private int _pendingDraw = 0;
    private ICard? _drawnCard = null;
    private bool _hasStart = false;
    internal Color? _chosenColor = null;
    internal bool _waitingForColor = false;

    internal List<IPlayer> _players = [];
    internal Dictionary<IPlayer, List<ICard>> _cardInHand = new();
    internal IDeck _deck;
    internal IDiscarded _discardedPile;
    private int _maxPlayer;
    internal GameDirection _gameDirection = GameDirection.Clockwise;
    private int _currentPlayerIndex;
    internal bool _turnSkipped = false;

    private Dictionary<IPlayer, bool> _callUno = new();
    private readonly ILogger<Game> _logger;
    private IPlayer? _winner;
    private int _lastDrawPenalty;

    public Game(ILogger<Game> logger)
    {
        _discardedPile = new Discarded();
        _deck = new Deck(InitializeCards());
        _currentPlayerIndex = 0;
        _logger = logger;
        SetWinnerCallback(OnWinner);
    }


    #region Game

    public void Play()
    {
        if (_players.Count < 2)
        {
            _logger.LogCritical("Not Enough Player");
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

            if (card.CardValue == CardValue.PlusFour)
            {
                _deck.DeckPiles.Push(card);

                List<ICard> cards = _deck.DeckPiles.ToList();

                Shuffle(cards);

                _deck.DeckPiles = new Stack<ICard>(cards);

                continue;
            }

            DiscardCard(card);

            if (card.CardValue == CardValue.Reverse ||
                card.CardValue == CardValue.Skip ||
                card.CardValue == CardValue.PlusTwo)
            {
                SpecialCardPlayed(card);

                if (_turnSkipped)
                {
                    NextPlayer(false);
                }

                for (int i = 0; i < _pendingDraw; i++)
                {
                    DrawCard(_players[_currentPlayerIndex]);
                }
                _lastDrawPenalty = _pendingDraw;
                _pendingDraw = 0;
            }

            if (card.CardValue == CardValue.Wild)
            {
                _waitingForColor = true;
                return;
            }
        }
    }

    public void RestartGame()
    {
        _deck = new Deck(InitializeCards());
        _discardedPile = new Discarded();
        _cardInHand.Clear();
        _callUno.Clear();

        foreach (IPlayer player in _players)
        {
            _callUno[player] = false;
        }

        _pendingDraw = 0;
        _drawnCard = null;
        _hasStart = false;
        _chosenColor = null;
        _waitingForColor = false;
        _currentPlayerIndex = 0;
        _gameDirection = GameDirection.Clockwise;
        _turnSkipped = false;

        _winner = null;
    }
    public void ResetGame()
    {
        _deck = new Deck(InitializeCards());
        _discardedPile = new Discarded();
        _cardInHand.Clear();
        _callUno.Clear();
        _players = new();

        _pendingDraw = 0;
        _drawnCard = null;
        _hasStart = false;
        _chosenColor = null;
        _waitingForColor = false;
        _currentPlayerIndex = 0;
        _gameDirection = GameDirection.Clockwise;
        _turnSkipped = false;

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
    }


    public IPlayer? GetWinner()
    {
        return _winner;
    }


    #endregion


    #region Discarding, Power Related, and Logic Checking
    public int GetLastDrawPenalty()
    {
        return _lastDrawPenalty;
    }
    public void PlayCard(ICard card)
    {

        IPlayer player = _players[_currentPlayerIndex];
        if (!CheckCardPlayability(card))
        {
            _logger.LogWarning(
               "Player {PlayerName} attempted to play invalid card {Color} {CardValue}",
               player.Name,
               card.Color,
               card.CardValue
           );

            return;
        }


        _lastDrawPenalty = 0;
        CheckPlayedCard(card);

        _cardInHand[player].Remove(card);

        _drawnCard = null;

        if (!_waitingForColor)
        {
            _chosenColor = null;
        }

        _logger.LogInformation(
       "Player {PlayerName} played {Color} {CardValue}",
       player.Name,
       card.Color,
       card.CardValue
   );

        if (CheckIfWinner(player))
        {
            _logger.LogInformation(
            "Player {PlayerName} won the game",
            player.Name
        );
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
        IPlayer nextPlayer = _players[_currentPlayerIndex];

        _logger.LogInformation(
            "Turn changed to {PlayerName}. Skipped: {Skipped}",
            nextPlayer.Name,
            skipped
        );

        PlayerTurn(nextPlayer);
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
                _pendingDraw += 1;
                _lastDrawPenalty = _pendingDraw;
                _callUno[player] = false;
            }
        }
        else
        {
            if (!_callUno[player])
            {
                _pendingDraw += 1;
                _lastDrawPenalty = _pendingDraw;
                _callUno[player] = false;
            }
        }

        if (_pendingDraw != 0)
        {
            for (int i = 0; i < _pendingDraw; i++)
            {
                DrawCard(player, true);
            }
            _pendingDraw = 0;
        }
        else
        {
            _lastDrawPenalty = 0;
        }
        _callUno[player] = false;
    }


    public void DiscardCard(ICard card)
    {
        if (card == null)
            return;
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
        _logger.LogInformation(
        "Special card played: {Color} {CardValue}",
        card.Color,
        card.CardValue
    );
        if (card.CardValue == CardValue.Reverse)
        {
            if (_gameDirection == GameDirection.Clockwise)
            {
                _logger.LogInformation(
                "Game direction changed to CounterClockwise"
            );
                _logger.LogInformation(
                "Game direction changed to Clockwise"
            );
                _gameDirection = GameDirection.CounterClockwise;
                return;
            }

            _gameDirection = GameDirection.Clockwise;
            return;
        }


        if (card.CardValue == CardValue.Skip)
        {
            _logger.LogInformation(
            "Next player will be skipped"
        );
            _turnSkipped = true;
            return;
        }


        if (card.CardValue == CardValue.PlusTwo)
        {
            _pendingDraw += 2;
            _logger.LogInformation(
            "Pending draw penalty increased to {Penalty}",
            _pendingDraw
        );
            _lastDrawPenalty = _pendingDraw;
            return;
        }


        if (card.CardValue == CardValue.PlusFour)
        {
            _pendingDraw += 4;
            _logger.LogInformation(
           "Plus Four played. Pending draw: {Penalty}. Waiting for color.",
           _pendingDraw
       );
            _lastDrawPenalty = _pendingDraw;
            _waitingForColor = true;
        }


        if (card.CardValue == CardValue.Wild)
        {
            _logger.LogInformation(
            "Wild card played. Waiting for color."
        );
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
        {
            _logger.LogWarning(
            "Attempted to choose color {Color} when game was not waiting for a color",
            color
        );
            return;

        }


        _chosenColor = color;
        _logger.LogInformation(
              "Color chosen: {Color}",
              color
          );
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

        _callUno[player] = true;
        _logger.LogInformation(
      "Player {PlayerName} called UNO",
      player.Name
  );
    }


    #endregion


    #region Player Related

    public void AddPlayer(string name)
    {
        if (!(_players.Count() == 5))
        {
            Player newPlayer = new(name);

            _players.Add(newPlayer);

            _callUno.Add(newPlayer, false);
        }
    }


    public List<ICard> GetPlayerCard(IPlayer player)
    {
        return _cardInHand[player].ToList();
    }

    public IPlayer GetCurrentPlayer()
    {
        IPlayer player = _players[_currentPlayerIndex];
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

                if (color != Color.Wild &&
                    value != CardValue.PlusFour &&
                    value != CardValue.Wild)
                {

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

        ICard topCard = _discardedPile.DiscardedCards.Pop();
        List<ICard> cards = _discardedPile.DiscardedCards.ToList();

        Shuffle(cards);

        _discardedPile.DiscardedCards = new Stack<ICard>();
        _discardedPile.DiscardedCards.Push(topCard);
        _deck.DeckPiles = new Stack<ICard>(cards);
    }
    public ICard DrawCard(IPlayer player, bool isPenalty = false)
    {
        if (_deck.DeckPiles.Count == 0)
        {
            _logger.LogInformation(
                "Deck is empty. Renewing deck."
            );

            RenewDeck();
        }

        ICard card = _deck.DeckPiles.Pop();

        _cardInHand[player].Add(card);
        _logger.LogInformation(
                "Player {PlayerName} drew {Color} {CardValue}. Penalty: {IsPenalty}",
                player.Name,
                card.Color,
                card.CardValue,
                isPenalty
            );

        if (!isPenalty && CheckCardPlayability(card))
        {
            _logger.LogInformation(
            "Drawn card {CardId} is playable",
            card.Id
        );
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
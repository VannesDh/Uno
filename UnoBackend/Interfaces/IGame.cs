    using UnoBackend.Models;
using UnoBackend.Models.Enum;

namespace UnoBackend.Interfaces;

public interface IGame
{
    // Game
    void Play();
    void RestartGame();
    void ResetGame();

    IPlayer? GetWinner();

    int GetLastDrawPenalty();
    void PlayCard(ICard card);
    List<Guid> CheckPlayableCard(IPlayer player);
    bool CheckCardPlayability(ICard card);
    bool WaitingForColor();
    void ChooseColor(Color color);
    void EndTurn();
    ICard GetCurrentTopPile();
    void CallUno(IPlayer player);

    // Player
    void AddPlayer(string name);
    List<ICard> GetPlayerCard(IPlayer player);
    IPlayer GetCurrentPlayer();

    // Cards
    int GetDeckCount();
    ICard DrawCard(IPlayer player, bool isPenalty = false);
}
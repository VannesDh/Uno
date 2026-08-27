import { useState } from "react";
import Board from "./Components/Board";
import { CheckPlayerCardPlayability, Draw, Play, PlayCard, ChooseColor, EndTurn, CallUno, AddPlayer, RestartGame } from "./Services/GameApi";
import type { CardDto, DeckDto, DiscardPileDto, HandDto, PlayerDto } from "./Types/Game";
import Hand from "./Components/Hand";
import "./App.css";

function App() {
  const [deck, setDeck] = useState<DeckDto | null>(null);
  const [gameStarted, setGameStarted] = useState(false);
  const [topPile, setTopPile] = useState<DiscardPileDto | null>(null);
  const [currentCards, setCurrentCards] = useState<HandDto | null>(null);
  const [currentPlayer, setCurrentPlayer] = useState<PlayerDto | null>(null);
  const [hasDrawn, setHasDrawn] = useState(false);
  const [hasPlayed, setHasPlayed] = useState(false);
  const [playableCardIds, setPlayableCardIds] = useState<string[]>([]);
  const [showColorPicker, setShowColorPicker] = useState(false);
  const [GameWinner, setGameWinner] = useState(false);
  const [players, setPlayers] = useState<string[]>([]);
  const [playerName, setPlayerName] = useState("")
  const [mustPlayDrawnCard, setMustPlayDrawnCard] = useState(false);

  // Handling play button
  const handlePlay = async () => {
    try {
      const data = await Play();
      setDeck(data.deckCount);
      setGameStarted(true);
      setCurrentCards(data.hand);
      setTopPile(data.discardPile)
      setCurrentPlayer(data.player)
      if (data.waitingForColor) {
        setShowColorPicker(true);
      }
      const playableIds = await CheckPlayerCardPlayability();
      setPlayableCardIds(playableIds);

    } catch (error) {
      console.error(error);
    }
  };

  const handleAddPlayer = async () => {
    if (!playerName.trim()) return;

    try {
      const player: PlayerDto = {
        playerName: playerName
      }
      await AddPlayer(player);

      setPlayers(prev => [...prev, playerName]);
      setPlayerName("");
    } catch (error) {
      console.error(error);
    }
  };

  if (!gameStarted) {
    return (
      <div>
        <div className="player-menu">
          <h1>UNO</h1>

          <div className="player-name">
            <h2>Players</h2>

            {players.map((player, index) => (
              <div className="player-text" key={index}>
                {player}
              </div>
            ))}
          </div>

          <input
            className="player-name-input"
            value={playerName}
            onChange={(e) => setPlayerName(e.target.value)}
            placeholder="Player name"
          />

          <button onClick={handleAddPlayer}>
            Add Player
          </button>

          <button
            disabled={players.length < 2}
            onClick={handlePlay}
          >
            Start Game
          </button>
        </div>
      </div>
    )
  }

  // handling drawing card
const handleDraw = async () => {
  try {
    if (!hasDrawn && !hasPlayed) {
      const data = await Draw();

      setHasDrawn(true);

      setDeck(prev => ({
        ...prev!,
        cardCount: data.deckCount
      }));

      setCurrentCards(prev => ({
        ...prev!,
        cards: [...prev!.cards, data.card]
      }));

      const playableIds = await CheckPlayerCardPlayability();

      if (playableIds.includes(data.card.id)) {
        // Drawn card is playable → MUST play it
        setPlayableCardIds([data.card.id]);
        setMustPlayDrawnCard(true);
      } else {
        // Drawn card isn't playable → can end turn
        setPlayableCardIds([]);
        setMustPlayDrawnCard(false);
      }
    }
  } catch (error) {
    console.error(error);
  }
};

const handlePlayAgain = async () => {
  setShowColorPicker(false);
  try {
    const data = await RestartGame();

    setGameWinner(false);
    setHasDrawn(false);
    setHasPlayed(false);
    setPlayableCardIds([]);
    setDeck(data.deckCount);
    setCurrentCards(data.hand);
    setTopPile(data.discardPile);
    setCurrentPlayer(data.player);

    if (data.waitingForColor) {
      setShowColorPicker(true);
    }
    const playableIds = await CheckPlayerCardPlayability();
    setPlayableCardIds(playableIds);

  } catch (error) {
    console.error(error);
  }
};

  const handleChooseColor = async (color: string) => {
    try {
      await ChooseColor(color);

      setShowColorPicker(false);

      const playableIds = await CheckPlayerCardPlayability();
      setPlayableCardIds(playableIds);

    } catch (error) {
      console.error(error);
    }
  };


  // handling  endturn
  const handleEndTurn = async () => {
    try {
      if (hasPlayed || hasDrawn) {

        const data = await EndTurn();
        setCurrentPlayer(data.player);
        setCurrentCards(data.hand)

        const playableIds = await CheckPlayerCardPlayability();

        setPlayableCardIds(playableIds);

        setHasDrawn(false);
        setHasPlayed(false);
      }

    } catch (error) {
      console.error(error);
    }
  };

  // Handling card drop to the discard pile
  const handlePlayCard = async (card: CardDto) => {
    try {
      if (!hasPlayed) {
        const data = await PlayCard(card);
        setCurrentCards(data.hand);
        setTopPile(data.discardPile);
        setHasPlayed(true);
        setMustPlayDrawnCard(false);
        setPlayableCardIds([]);
        setGameWinner(data.gameWinner);

        if (card.value === "Wild" || card.value === "PlusFour") {
          setShowColorPicker(true);
        }
      }
    } catch (error) {
      console.error(error);
    }
  };

  // Handle UNO called
  const handleCallUno = async () => {
    try {
      await CallUno()
    } catch (error) {
      console.error(error);
    }
  }

  return (
    <div>
      <div className="game-header">
        <div className="current-player">
          <span className="current-player-label">CURRENT PLAYER</span>
          <span className="current-player-name">
            {currentPlayer?.playerName}
          </span>
        </div>

        <button
          className="end-turn-btn"
          onClick={handleEndTurn}
          disabled={mustPlayDrawnCard}
        >
          END TURN
        </button>
      </div>
      <Board
        deck={deck!}
        discardPile={topPile!}
        onDraw={handleDraw}
        hasDrawn={hasDrawn}
        onPlayCard={handlePlayCard}
      />

      <Hand
        cards={currentCards!}
        playableCardIds={playableCardIds}
      />

     <button className="uno-btn" onClick={handleCallUno}>
  UNO!
</button>
      {showColorPicker && (
        <div className="color-picker-overlay">
          <div className="color-picker">
            <h2>Choose a Color</h2>

            <button onClick={() => handleChooseColor("Red")}>
              Red
            </button>

            <button onClick={() => handleChooseColor("Blue")}>
              Blue
            </button>

            <button onClick={() => handleChooseColor("Green")}>
              Green
            </button>

            <button onClick={() => handleChooseColor("Yellow")}>
              Yellow
            </button>
          </div>
        </div>
      )}

      <div>
        {GameWinner && (
          <div className="winner-page">
            <div className="winner-content">

              <div className="winner-card">
                <div className="winner-card-inner">
                  <span>UNO!</span>
                </div>
              </div>

              <p className="winner-label">GAME OVER</p>

              <h1>
                <span>{currentPlayer?.playerName}</span> WINS!
              </h1>

              <p className="winner-subtitle">
                Congratulations! You played your last card.
              </p>

              <div className="winner-buttons">
                <button className="play-again-btn" onClick={handlePlayAgain}>
                  PLAY AGAIN
                </button>

                <button className="menu-btn">
                  MAIN MENU
                </button>
              </div>

            </div>
          </div>
        )}
      </div>
    </div>



  );
}

export default App;
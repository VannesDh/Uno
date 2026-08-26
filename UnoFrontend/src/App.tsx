import { useState } from "react";
import Board from "./Components/Board";
import { CheckPlayerCardPlayability, Draw, Play, PlayCard, ChooseColor, EndTurn } from "./Services/GameApi";
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




  // Handling play button
  const handlePlay = async () => {
    try {
      const data = await Play();
      setDeck(data.deckCount);
      setGameStarted(true);
      setCurrentCards(data.hand);
      setTopPile(data.discardPile)
      setCurrentPlayer(data.currentPlayer)

      const playableIds = await CheckPlayerCardPlayability();
      setPlayableCardIds(playableIds);

    } catch (error) {
      console.error(error);
    }
  };

  if (!gameStarted) {
    return <button onClick={handlePlay}>Play</button>;
  }

  // handling drawing card
  const handleDraw = async () => {
    try {
      if (deck?.cardCount == 0) {

      }

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
      }
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

        const playableIds = await CheckPlayerCardPlayability();
        setPlayableCardIds(playableIds);
        setHasPlayed(true);

        if (card.value === "Wild" || card.value === "PlusFour") {
          setShowColorPicker(true);
        }
      }
    } catch (error) {
      console.error(error);
    }
  };

  return (
    <div>
      <div>
        current Player : {currentPlayer?.playerName}
      </div>

      <button onClick={handleEndTurn}>
        End Turn
      </button>
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
    </div>
  );
}

export default App;
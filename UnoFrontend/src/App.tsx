import { useState } from "react";
import Board from "./Components/Board";
import { Play } from "./Services/GameApi";
import type { DeckDto, DiscardPileDto, HandDto, PlayerDto } from "./Types/Game";
import Hand from "./Components/Hand";

function App() {
  const [deck, setDeck] = useState<DeckDto | null>(null);
  const [gameStarted, setGameStarted] = useState(false);
  const [topPile, setTopPile] = useState<DiscardPileDto | null>(null);
  const [currentCards, setCurrentCards] = useState<HandDto|null>(null);
  const [currentPlayer, setCurrentPlayer] = useState<PlayerDto | null>(null);
  const handlePlay = async () => {
    try {
      const data = await Play();

      setDeck(data.deckCount);
      setGameStarted(true);
      setCurrentCards(data.hand);
      setTopPile(data.discardPile)
      setCurrentPlayer(data.currentPlayer)
    } catch (error) {
      console.error(error);
    }
  };

  if (!gameStarted) {
    return <button onClick={handlePlay}>Play</button>;
  }

  const handleDraw = async () => {
    // call your draw API here
    console.log("Drawing card...");
  };

  return (
    <div>
      <Board
          deck={deck!}
          discardPile={topPile!}
          onDraw={handleDraw}
        />
      <Hand cards={currentCards!} />
    </div>

  );

    

}

export default App;
import { useState } from "react";
import Board from "./Components/Board";
import { Play } from "./Services/GameApi";
import type { DeckDto, HandDto } from "./Types/Game";
import Hand from "./Components/Hand";

function App() {
  const [deck, setDeck] = useState<DeckDto | null>(null);
  const [gameStarted, setGameStarted] = useState(false);
  const [currentCards, setCurrentCards] = useState<HandDto|null>(null);
  const handlePlay = async () => {
    try {
      const data = await Play();

      setDeck(data.deckCount);
      setGameStarted(true);
      setCurrentCards(data.hand);
    } catch (error) {
      console.error(error);
    }
  };

  if (!gameStarted) {
    return <button onClick={handlePlay}>Play</button>;
  }

  return (
    <div>
      <Board deck={deck!} />
      <Hand cards={currentCards!} />
    </div>

  );

    

}

export default App;
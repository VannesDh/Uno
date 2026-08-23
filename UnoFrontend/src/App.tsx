import { useState } from "react";
import Board from "./Components/Board";
import { Play } from "./Services/GameApi";
import type { DeckDto } from "./Types/Game";

function App() {
  const [deck, setDeck] = useState<DeckDto | null>(null);
  const [gameStarted, setGameStarted] = useState(false);

  const handlePlay = async () => {
    try {
      const data = await Play();

      setDeck(data);
      setGameStarted(true);
    } catch (error) {
      console.error(error);
    }
  };

  if (!gameStarted) {
    return <button onClick={handlePlay}>Play</button>;
  }

  return <Board deck={deck!} />;
}

export default App;
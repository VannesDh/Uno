import { useState } from "react";
import Board from "./Components/Board";
import { CheckPlayerCardPlayability, Draw, Play, PlayCard } from "./Services/GameApi";
import type { CardDto, DeckDto, DiscardPileDto, HandDto, PlayerDto } from "./Types/Game";
import Hand from "./Components/Hand";

function App() {
  const [deck, setDeck] = useState<DeckDto | null>(null);
  const [gameStarted, setGameStarted] = useState(false);
  const [topPile, setTopPile] = useState<DiscardPileDto | null>(null);
  const [currentCards, setCurrentCards] = useState<HandDto | null>(null);
  const [currentPlayer, setCurrentPlayer] = useState<PlayerDto | null>(null);
  const [hasDrawn, setHasDrawn] = useState(false);
  const [hasPlayed, setHasPlayed] = useState(false);
  const [playableCardIds, setPlayableCardIds] = useState<string[]>([]);




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
      console.log(playableCardIds)
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
    } catch (error) {
      console.error(error);
    }
  };

  // Handling card drop to the discard pile
  const handlePlayCard = async (card: CardDto) => {
    try {
      const data = await PlayCard(card);
      setCurrentCards(data.hand);
      setTopPile(data.discardPile);

      const playableIds = await CheckPlayerCardPlayability();
      setPlayableCardIds(playableIds);
    } catch (error) {
      console.error(error);
    }
  };

  return (
    <div>
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
    </div>

  );



}

export default App;
import type { HandDto } from "../Types/Game";
import Card from "./Card";
import "./Hand.css";

interface HandProps {
  cards : HandDto
  playableCardIds : string[]
  isPlayerHidden : boolean
}

function Hand({ cards, playableCardIds, isPlayerHidden}: HandProps) {
  return (
    <div className="hand">
      {cards.cards.map((card) => (
        <Card
          key={card.id}
          id = {card.id}
          color={card.color}
          value={card.value}
          playable={playableCardIds.includes(card.id)}
          isPlayerHidden={isPlayerHidden}
        />
      ))}
    </div>
  );
}

export default Hand;
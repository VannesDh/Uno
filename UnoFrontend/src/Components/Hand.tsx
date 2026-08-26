import type { HandDto } from "../Types/Game";
import Card from "./Card";
import "./Hand.css";

interface HandProps {
  cards : HandDto
  playableCardIds : string[]
}

function Hand({ cards, playableCardIds }: HandProps) {
  return (
    <div className="hand">
      {cards.cards.map((card) => (
        <Card
          key={card.id}
          id = {card.id}
          color={card.color}
          value={card.value}
          playable={playableCardIds.includes(card.id)}
        />
      ))}
    </div>
  );
}

export default Hand;
import type { HandDto } from "../Types/Game";
import Card from "./Card";
import "./Hand.css";

interface HandProps {
  cards : HandDto
}

function Hand({ cards }: HandProps) {
  return (
    <div className="hand">
      {cards.cards.map((card,index) => (
        <Card
          key={index}
          color={card.color}
          value={card.value}
        />
      ))}
    </div>
  );
}

export default Hand;
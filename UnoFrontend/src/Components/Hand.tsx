import Card from "./Card";
import "./Hand.css";

interface HandProps {
  cards: {
    color: string;
    value: string;
  }[];
}

function Hand({ cards }: HandProps) {
  return (
    <div className="hand">
      {cards.map((card, index) => (
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
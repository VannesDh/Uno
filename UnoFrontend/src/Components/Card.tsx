import "./Card.css";
import cardBack from "../Assets/PixelArtAssets-main/UnoCards/Card_Back.png";

const cardImages = import.meta.glob(
  "../Assets/PixelArtAssets-main/UnoCards/*.png",
  {
    eager: true,
    query: "?url",
    import: "default",
  }
);

interface CardProps {
  id: string;
  color: string;
  value: string;
  playable: boolean;
  isPlayerHidden: boolean;
}

function Card({
  id,
  color,
  value,
  playable,
  isPlayerHidden
}: CardProps) {
  const fileName = `${color}_${value}.png`;
  const imagePath = `../Assets/PixelArtAssets-main/UnoCards/${fileName}`;

  const image = cardImages[imagePath];

  const handleDragStart = (event: React.DragEvent<HTMLDivElement>) => {
    event.dataTransfer.effectAllowed = "move";

    event.dataTransfer.setData(
      "card",
      JSON.stringify({
        id,
        color,
        value
      })
    );
  };

  return (
    <div
      className={`card ${
        playable && !isPlayerHidden ? "playable" : "notPlayable"
      }`}
      draggable={playable && !isPlayerHidden}
      onDragStart={handleDragStart}
    >
      <img
        src={isPlayerHidden ? cardBack : image}
        alt={`${color} ${value}`}
        draggable={false}
      />
    </div>
  );
}

export default Card;
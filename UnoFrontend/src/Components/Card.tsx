import "./Card.css";

const cardImages = import.meta.glob(
  "../Assets/PixelArtAssets-main/UnoCards/*.png",
  {
    eager: true,
    query: "?url",
    import: "default",
  }
);

interface CardProps {
  color: string;
  value: string;
}

function Card({ color, value }: CardProps) {
  const fileName = `${color}_${value}.png`;
  const imagePath = `../Assets/PixelArtAssets-main/UnoCards/${fileName}`;

  const image = cardImages[imagePath];

  return (
    <div
      className="card"
      draggable
      onDragStart={(event) => {
        event.dataTransfer.setData(
          "card",
          JSON.stringify({ color, value })
        );
      }}
    >
      <img src={image} alt={`${color} ${value}`} />
    </div>
  );
}

export default Card;
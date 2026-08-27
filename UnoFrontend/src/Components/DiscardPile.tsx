import "./DiscardPile.css";

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

function DiscardPile({ color, value }: CardProps) {
  const fileName = `${color}_${value}.png`;
  const imagePath = `../Assets/PixelArtAssets-main/UnoCards/${fileName}`;
  const image = cardImages[imagePath];

  return (
    <div className="discard-pile">
      <img
        className="discard-card"
        src={image}
        alt={`${color} ${value}`}
      />
    </div>
  );
}

export default DiscardPile;
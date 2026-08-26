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
  id : string
  color: string;
  value: string;
  playable: boolean;

}

function Card({id, color, value, playable }: CardProps) {
  const fileName = `${color}_${value}.png`;
  const imagePath = `../Assets/PixelArtAssets-main/UnoCards/${fileName}`;

  const image = cardImages[imagePath];

  return (
    <div
      className={`card ${playable ? "playable" : "notPlayable"}`}
      draggable
      onDragStart={(event) => {
        event.dataTransfer.setData(
        "card",
        JSON.stringify({
          id,color,value
        })
      );
      }}
    >
      <img src={image} alt={`${color} ${value}`} />
    </div>
  );
}

export default Card;
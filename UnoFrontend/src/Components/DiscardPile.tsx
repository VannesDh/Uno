import { useEffect, useState } from "react";
import "./DiscardPile.css";
import "./CardAnimation.css"

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
  const [animation, setAnimation] = useState("");

  const fileName = `${color}_${value}.png`;
  const imagePath = `../Assets/PixelArtAssets-main/UnoCards/${fileName}`;
  const image = cardImages[imagePath];

  useEffect(() => {
    switch (value) {
      case "PlusTwo":
        setAnimation("animate-plus-two");
        break;

      case "PlusFour":
        setAnimation("animate-plus-four");
        break;

      case "Reverse":
        setAnimation("animate-reverse");
        break;

      case "Skip":
        setAnimation("animate-skip");
        break;

      default:
        setAnimation("");
        break;
    }

    const timer = setTimeout(() => {
      setAnimation("");
    }, 700);

    return () => clearTimeout(timer);
  }, [color, value]);

  return (
    <div className={"discard-pile"}>
      <img
        className={`discard-card ${animation}`}
        src={image}
        alt={`${color} ${value}`}
      />
    </div>
  );
}

export default DiscardPile;
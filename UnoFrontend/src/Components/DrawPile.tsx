import { useEffect, useState } from "react";
import type { DeckDto } from "../Types/Game";
import "./DrawPile.css";
import "./CardAnimation.css";

const cardImages = import.meta.glob(
  "../Assets/PixelArtAssets-main/UnoCards/*.png",
  {
    eager: true,
    query: "?url",
    import: "default",
  }
);

interface CardProps {
  deck: DeckDto;
  onDraw: () => void;
  hasDrawn: boolean;
}

function DrawPile({ deck, onDraw, hasDrawn }: CardProps) {
  const [isDrawing, setIsDrawing] = useState(false);

  const fileName = "Card_Back.png";
  const imagePath = `../Assets/PixelArtAssets-main/UnoCards/${fileName}`;
  const image = cardImages[imagePath];

  useEffect(() => {
    if (hasDrawn) {
      setIsDrawing(true);

      const timer = setTimeout(() => {
        setIsDrawing(false);
      }, 400);

      return () => clearTimeout(timer);
    }
  }, [hasDrawn]);

  return (
    <div className="draw-pile">
      <div className={`deck ${isDrawing ? "draw-animation" : ""}`}>
        <img
          className="deck-card"
          src={image}
          alt="Draw pile"
          onClick={onDraw}
        />

        <div className="deck-count">
          {deck.cardCount}
        </div>
      </div>
    </div>
  );
}

export default DrawPile;
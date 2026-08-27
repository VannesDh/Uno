import type { DeckDto } from "../Types/Game";
import "./DrawPile.css";

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
  const fileName = "Card_Back.png";
  const imagePath = `../Assets/PixelArtAssets-main/UnoCards/${fileName}`;
  const image = cardImages[imagePath];

  return (
    <div className="draw-pile">

      <div className="deck">
        <img
          className="deck-card"
          src={image}
          alt="Draw pile"
        />

        <div className="deck-count">
          {deck.cardCount}
        </div>
      </div>

      <button
        className="draw-button"
        onClick={onDraw}
        disabled={hasDrawn}
      >
        DRAW
      </button>

    </div>
  );
}

export default DrawPile;
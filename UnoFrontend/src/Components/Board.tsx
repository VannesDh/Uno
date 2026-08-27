import type {
  CardDto,
  DeckDto,
  DiscardPileDto
} from "../Types/Game";

import DiscardPile from "./DiscardPile";
import DrawPile from "./DrawPile";

import "./Board.css";

interface BoardProps {
  deck: DeckDto;
  discardPile: DiscardPileDto;
  onDraw: () => void;
  hasDrawn: boolean;
  onPlayCard: (card: CardDto) => void;
}

function Board({
  deck,
  discardPile,
  onDraw,
  hasDrawn,
  onPlayCard
}: BoardProps) {

  const handleDragOver = (
    event: React.DragEvent<HTMLDivElement>
  ) => {
    event.preventDefault();
  };

  const handleDrop = (
    event: React.DragEvent<HTMLDivElement>
  ) => {
    event.preventDefault();

    const cardData = event.dataTransfer.getData("card");

    if (!cardData) return;

    try {
      const card: CardDto = JSON.parse(cardData);

      onPlayCard(card);
    } catch (error) {
      console.error("Invalid card data:", error);
    }
  };

  return (
    <div className="board">

      <div className="pile-container">

        {/* =========================
            DRAW PILE
           ========================= */}

        <div className="pile">

          <h3 className="pile-title">
            DRAW PILE
          </h3>

          <DrawPile
            deck={deck}
            onDraw={onDraw}
            hasDrawn={hasDrawn}
          />

        </div>


        {/* =========================
            DISCARD PILE
           ========================= */}

        <div className="pile">

          <h3 className="pile-title">
            DISCARD PILE
          </h3>

          <div
            className="discard-drop-zone"
            onDragOver={handleDragOver}
            onDrop={handleDrop}
          >

            <DiscardPile
              color={
                discardPile.lastCardInDiscardPile.color
              }
              value={
                discardPile.lastCardInDiscardPile.value
              }
            />

          </div>

        </div>

      </div>

    </div>
  );
}

export default Board;
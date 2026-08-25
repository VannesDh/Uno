import type { DeckDto, DiscardPileDto } from "../Types/Game";
import DiscardPile from "./DiscardPile";
import DrawPile from "./DrawPile";
import "./Board.css"
interface BoardProps {
  deck: DeckDto;
  discardPile: DiscardPileDto;
  onDraw: () => void;
}

function Board({ deck, discardPile, onDraw }: BoardProps) {
  return (
    <div className="board">
      <div className="pile-container">

        <div className="pile">
          <h3>Draw Pile</h3>

          <div className="draw-pile">
            <DrawPile 
              deck={deck}
              onDraw={onDraw}
            />
          </div>
        </div>

        <div className="pile">
          <h3>Discard Pile</h3>

          <div className="discard-pile">

            <DiscardPile
              color={discardPile.lastCardInDiscardPile.color}
              value={discardPile.lastCardInDiscardPile.value}
            />
          </div>
        </div>

      </div>
    </div>
  );
}

export default Board;
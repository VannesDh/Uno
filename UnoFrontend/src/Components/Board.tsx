import type { CardDto, DeckDto, DiscardPileDto } from "../Types/Game";
import DiscardPile from "./DiscardPile";
import DrawPile from "./DrawPile";
import "./Board.css"
interface BoardProps {
  deck: DeckDto;
  discardPile: DiscardPileDto;
  onDraw: () => void;
  hasDrawn: boolean;
  onPlayCard: (card: CardDto) => void;
}




function Board({ deck, discardPile, onDraw, hasDrawn, onPlayCard }: BoardProps) {

  const handleDragOver = (event: React.DragEvent<HTMLDivElement>) => {
    event.preventDefault();
  };

  const handleDrop = (event: React.DragEvent<HTMLDivElement>) => {
    event.preventDefault();

    const cardData = event.dataTransfer.getData("card");

    if (!cardData) return;

    const card = JSON.parse(cardData);

    onPlayCard(card);
  };



  return (
    <div className="board">
      <div className="pile-container">

        <div className="pile">
          <h3>Draw Pile</h3>

          <div className="draw-pile">
            <DrawPile
              deck={deck}
              onDraw={onDraw}
              hasDrawn={hasDrawn}
            />
          </div>
        </div>

        <div className="pile">
          <h3>Discard Pile</h3>

          <div className="discard-pile"  
            onDragOver={handleDragOver}
            onDrop={handleDrop}>

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
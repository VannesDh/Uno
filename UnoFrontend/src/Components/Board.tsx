import type { DeckDto } from "../Types/Game";
import Hand from "./Hand";

interface BoardProps {
  deck: DeckDto;
}

function Board({ deck }: BoardProps) {
  return (
    <div>
      <div>
        Cards remaining: {deck.cardCount}
      </div>

    </div>

    
  );
}

export default Board;
import type { DeckDto } from "../Types/Game";

interface BoardProps {
  deck: DeckDto;
}

function Board({ deck }: BoardProps) {
  return (
    <div>
      Cards remaining: {deck.cards.length}
    </div>
  );
}

export default Board;
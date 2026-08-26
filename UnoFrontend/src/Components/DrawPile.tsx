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
    const fileName = `${"Card"}_${"Back"}.png`;
    const imagePath = `../Assets/PixelArtAssets-main/UnoCards/${fileName}`;
    const image = cardImages[imagePath];

    return (
        <div className="container">
            <img src={image} />
            <h2 id="deckCount">
                {deck.cardCount}
            </h2>
            <div id="drawBtn">
                <button
                    onClick={onDraw}
                    disabled = {hasDrawn}
                    >
                    DRAW
                </button>
            </div>
        </div>
    )
}

export default DrawPile;
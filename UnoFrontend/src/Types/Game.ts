export interface CardDto {
  id :string;
  color: string;
  value: string;
}

export interface DeckDto {
  cardCount: number;
}

export interface HandDto {
  cards: CardDto[];
}

export interface PlayerDto {
  playerId: number;
  playerName: string;
}

export interface DiscardPileDto {
  lastCardInDiscardPile: CardDto;
}

export interface InitialDataDto {
  deck: DeckDto;
  hand: HandDto;
  player: PlayerDto;
  discardPile: DiscardPileDto;
}
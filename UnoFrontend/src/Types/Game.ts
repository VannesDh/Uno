
export interface CardDto {
  color: string;
  value: string;
}

export interface DeckDto {
  cards: CardDto[];
}
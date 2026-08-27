import type { CardDto, PlayerDto} from "../Types/Game";

export async function Play() {
  const response = await fetch("http://localhost:5172/api/game/play");

  if (!response.ok) {
    throw new Error("Failed to fetch deck");
  }

  return response.json();
}

export async function CheckPlayerCardPlayability() {
  const response = await fetch("http://localhost:5172/api/game/checkPlayability");

  if (!response.ok) {
    throw new Error("Failed to fetch deck");
  }

  return response.json();
}

export async function Draw() {
  const response = await fetch("http://localhost:5172/api/game/draw", {
    method: "POST",
  });

  if (!response.ok) {
    throw new Error("Failed to draw card");
  }

  return response.json();
}

export async function AddPlayer(player: PlayerDto) {
  const response = await fetch("http://localhost:5172/api/game/addPlayer", {
    method: "POST",
    headers: {
        "Content-Type": "application/json",
      },
    body: JSON.stringify(player),
  });

  if (!response.ok) {
    throw new Error("Cant Add Player");
  }

  return;
}

export async function PlayCard(card: CardDto) {
  const response = await fetch(
    "http://localhost:5172/api/game/playCard",
    {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(card),
    }
  );

  if (!response.ok) {
    const error = await response.text();
    console.log("Backend:", error);
    throw new Error("Failed to play card");
  }
  console.log(response)
  return response.json();
}

export async function ChooseColor(color: string) {
  const response = await fetch(
    "http://localhost:5172/api/game/chooseColor",
    {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(color),
    }
  );

  if (!response.ok) {
    throw new Error("Failed to choose color");
  }
}

export async function EndTurn() {
  const response = await fetch(
    "http://localhost:5172/api/game/endTurn",
    {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
    }
  );

  if (!response.ok) {
    throw new Error("Failed to end turn");
  }

  return response.json();
}

export async function CallUno(){
  const response = await fetch(
    "http://localhost:5172/api/game/callUno",
    {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
    }
  );
   if (!response.ok) {
    throw new Error("Failed to end turn");
  }
  return;
}
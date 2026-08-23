export async function Play() {
  const response = await fetch("http://localhost:5172/api/game/play");

  if (!response.ok) {
    throw new Error("Failed to fetch deck");
  }

  return response.json();
}
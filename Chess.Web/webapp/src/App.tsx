import { useState } from 'react'
import GamePage from "./pages/GamePage.tsx";
import HomePage from "./pages/HomePage.tsx";
import type {GameConfig} from "./game/types.ts";

const gameConfigStorageKey = "chess.gameConfig";
const gameIdStorageKey = "chess.gameId";

export default function App() {
  const [gameConfig, setGameConfig] = useState<GameConfig | undefined>(() => {
    const savedConfig = sessionStorage.getItem(gameConfigStorageKey);
    return savedConfig ? JSON.parse(savedConfig) as GameConfig : undefined;
  });
  
  function startGame(config: GameConfig) {
    if (config === null) {
      return;
    }
    
    sessionStorage.setItem(gameConfigStorageKey, JSON.stringify(config));
    sessionStorage.setItem(gameIdStorageKey, crypto.randomUUID());
    setGameConfig(config);
  }

  function exitGame() {
    sessionStorage.removeItem(gameConfigStorageKey);
    sessionStorage.removeItem(gameIdStorageKey);
    setGameConfig(undefined);
  }
  
  return gameConfig
      ? <GamePage config={gameConfig} gameId={sessionStorage.getItem(gameIdStorageKey) ?? crypto.randomUUID()} onExit={exitGame}/>
      : <HomePage onStartGame={startGame}/>
}


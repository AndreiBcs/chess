import { useState } from 'react'
import GamePage from "./pages/GamePage.tsx";
import HomePage from "./pages/HomePage.tsx";
import type {GameConfig} from "./game/types.ts";

export default function App() {
  
  const [gameStarted, setGameStarted] = useState(false);
  
  function startGame(config: GameConfig) {
    setGameStarted(true);
    
    console.log(config);
  }
  
  return <>
    {
      gameStarted
          ? <GamePage/>
          : <HomePage onStartGame={startGame}/>
    }
  </>
}


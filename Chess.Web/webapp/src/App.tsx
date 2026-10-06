import {useCallback, useState} from 'react';
import GamePage from "./pages/GamePage.tsx";
import HomePage from "./pages/HomePage.tsx";
import {readStoredGameSession, type StoredGameSession, clearStoredGameSession} from "./game/session.ts";
import type {GameConfig} from "./game/types.ts";

export default function App() {
  const [activeGame, setActiveGame] = useState<StoredGameSession | {config: GameConfig} | undefined>();
  const [hasSavedSession, setHasSavedSession] = useState(() => Boolean(readStoredGameSession()));
  const [gameKey, setGameKey] = useState(0);
  
  const startGame = useCallback((config: GameConfig) => {
    clearStoredGameSession();
    setHasSavedSession(false);
    setGameKey(key => key + 1);
    setActiveGame({config});
  }, []);

  const reconnect = useCallback(() => {
    const savedSession = readStoredGameSession();
    if (savedSession) {
      setGameKey(key => key + 1);
      setActiveGame(savedSession);
    }
  }, []);

  const exitGame = useCallback(() => {
    clearStoredGameSession();
    setHasSavedSession(false);
    setActiveGame(undefined);
  }, []);

  const sessionSaved = useCallback(() => setHasSavedSession(true), []);
  
  return activeGame
      ? <GamePage
          key={gameKey}
          config={activeGame.config}
          initialToken={'token' in activeGame ? activeGame.token : undefined}
          initialPlayerColor={'playerColor' in activeGame ? activeGame.playerColor : undefined}
          onSessionSaved={sessionSaved}
          onExit={exitGame}
          onNewGame={() => startGame(activeGame.config)}
        />
      : <HomePage onStartGame={startGame} onReconnect={reconnect} hasSavedSession={hasSavedSession}/>;
}


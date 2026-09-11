import { useEffect, useState } from 'react'

const socket = new WebSocket("ws://localhost:5204/ws");
export default function App() {

  useEffect(() => {

    socket.onopen = () => {
      console.log("Connection opened");

      socket.send(JSON.stringify({
        type: "StartOptions",
        data: {
          playerColor: "white",
          engineType: "stockfish",
          elo: 1500
        }
      }))
    }

    socket.onmessage = (e) => {
      const message = JSON.parse(e.data);

      console.log("Received message", message);
    }

    socket.onclose = () => {
      console.log("Connection closed");
    }

    socket.onerror = (error) => {
      console.log("Connection failed", error);
    }
    
    return () => socket.close()
  }, []);
  
  return (
    <>
      <h1>Chess</h1>
      <button onClick={() => {
        socket.send(JSON.stringify({
          type: "Move",
          data: {
            from: { row: 6, column: 4 },
            to: { row: 4, column: 4 },
            promotion: null
          }
        }))
      }}
      >Make Move</button>
    </>
  )
}


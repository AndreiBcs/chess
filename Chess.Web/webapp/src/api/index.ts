import type Move from '..//game/types.ts'

const socket = new WebSocket("ws://localhost:5204/ws");

socket.onopen = () => {
    console.log("Connection opened");
    
    socket.send(JSON.stringify({
        type: "startOptions",
        data: {
            color: "white",
            engine: "stockfish",
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

export function sendMove(move: Move) {
    socket.send(JSON.stringify({
        type: "move",
        data: {
            from: { row: move.from.row, col: move.from.col },
            to: { row: move.to.row, col: move.to.col },
            promotion: move.promotion
        }
    }));
}
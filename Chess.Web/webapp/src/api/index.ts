import { HubConnectionBuilder, LogLevel, type HubConnection } from "@microsoft/signalr";
import type { Move, StartGameOptions } from "../game/types.ts";

const hubUrl = import.meta.env.VITE_SIGNALR_URL || "/gamehub";

export function createConnection() {
    return new HubConnectionBuilder()
        .withUrl(hubUrl)
        .withAutomaticReconnect()
        .configureLogging(import.meta.env.DEV ? LogLevel.Warning : LogLevel.Error)
        .build();
}

export function startGame(connection: HubConnection, options: StartGameOptions) {
    return connection.invoke<string | null>("StartGame", options);
}

export function findMatch(connection: HubConnection, options: StartGameOptions) {
    return connection.invoke("FindMatch", options);
}

export function rejoinGame(connection: HubConnection, token: string) {
    return connection.invoke("Rejoin", token);
}

export function cancelMatch(connection: HubConnection) {
    return connection.invoke("CancelMatch");
}

export function resignGame(connection: HubConnection) {
    return connection.invoke("ResignGame");
}

export function submitMove(connection: HubConnection, move: Move) {
    return connection.invoke("SubmitMove", {
        from: {row: move.from.row, column: move.from.col},
        to: {row: move.to.row, column: move.to.col},
        promotion: move.promotion ?? null
    });
}
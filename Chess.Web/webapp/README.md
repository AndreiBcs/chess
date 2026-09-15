# Chess Web

The React client for the chess API. Local development uses Vite's `/gamehub` proxy to connect to the ASP.NET SignalR hub.

## Development

```bash
npm install
npm run dev
```

Set `VITE_API_URL` in `.env` when the API is not running at `http://localhost:5204`. Set `VITE_SIGNALR_URL` only when the browser should connect to a public hub URL directly.

## Tests

```bash
npm run test:e2e
```

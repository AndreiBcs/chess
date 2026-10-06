type EngineSettingsDialogProps = {
    open: boolean,
    depth: number,
    moveTime: number,
    nodes: number,
    depthValid: boolean,
    moveTimeValid: boolean,
    nodesValid: boolean,
    engineSettingsValid: boolean,
    onDepthChange: (value: number) => void,
    onMoveTimeChange: (value: number) => void,
    onNodesChange: (value: number) => void,
    onClose: () => void
}

export default function EngineSettingsDialog({
    open,
    depth,
    moveTime,
    nodes,
    depthValid,
    moveTimeValid,
    nodesValid,
    engineSettingsValid,
    onDepthChange,
    onMoveTimeChange,
    onNodesChange,
    onClose
}: EngineSettingsDialogProps) {
    if (!open) return null;

    return <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/70 p-4"
        role="dialog" aria-modal="true" aria-labelledby="engine-settings-title">
        <fieldset className="w-full max-w-md border border-[#9b6048] bg-[#252525] p-5 text-[#f1dfc1] shadow-2xl sm:p-6">
            <legend id="engine-settings-title" className="px-2 text-lg font-semibold">Engine settings</legend>
            <header className="mb-4 flex justify-end">
                <button className="border border-[#9b6048] px-3 py-1.5 text-sm hover:bg-[#3a322c]"
                    type="button" onClick={onClose}>Close</button>
            </header>
            <div className="space-y-4">
                <fieldset className="border border-[#9b6048] p-3">
                    <legend className="px-2 text-sm">Depth <span className="text-xs text-[#d5c2a5]">(1-50)</span></legend>
                    <input className="mt-1.5 w-full border border-[#9b6048] bg-[#f1dfc1] px-3 py-2 text-[#202020] outline-none focus:border-white"
                        type="number" value={depth} onChange={event => onDepthChange(Number(event.target.value))}
                        min={1} max={50} step={1} required aria-invalid={!depthValid}
                        aria-describedby={!depthValid ? "engine-depth-error" : undefined} />
                    {!depthValid && <span id="engine-depth-error" className="mt-1 block text-xs text-[#f0a58e]" role="alert">
                        Enter a whole number from 1 to 50.
                    </span>}
                </fieldset>
                <fieldset className="border border-[#9b6048] p-3">
                    <legend className="px-2 text-sm">Move time <span className="text-xs text-[#d5c2a5]">(1-600,000 ms)</span></legend>
                    <input className="mt-1.5 w-full border border-[#9b6048] bg-[#f1dfc1] px-3 py-2 text-[#202020] outline-none focus:border-white"
                        type="number" value={moveTime} onChange={event => onMoveTimeChange(Number(event.target.value))}
                        min={1} max={600000} step={1} required aria-invalid={!moveTimeValid}
                        aria-describedby={!moveTimeValid ? "engine-move-time-error" : undefined} />
                    {!moveTimeValid && <span id="engine-move-time-error" className="mt-1 block text-xs text-[#f0a58e]" role="alert">
                        Enter a whole number from 1 to 600,000 ms.
                    </span>}
                </fieldset>
                <fieldset className="border border-[#9b6048] p-3">
                    <legend className="px-2 text-sm">Nodes <span className="text-xs text-[#d5c2a5]">(1-1,000,000,000)</span></legend>
                    <input className="mt-1.5 w-full border border-[#9b6048] bg-[#f1dfc1] px-3 py-2 text-[#202020] outline-none focus:border-white"
                        type="number" value={nodes} onChange={event => onNodesChange(Number(event.target.value))}
                        min={1} max={1000000000} step={1} required aria-invalid={!nodesValid}
                        aria-describedby={!nodesValid ? "engine-nodes-error" : undefined} />
                    {!nodesValid && <span id="engine-nodes-error" className="mt-1 block text-xs text-[#f0a58e]" role="alert">
                        Enter a whole number from 1 to 1,000,000,000.
                    </span>}
                </fieldset>
            </div>
            <button className="mt-5 w-full border border-[#f1dfc1] bg-[#f1dfc1] px-4 py-2 font-semibold text-[#3d2b24] hover:bg-[#9b6048] hover:text-[#f1dfc1] disabled:cursor-not-allowed disabled:opacity-45"
                type="button" disabled={!engineSettingsValid}
                title={!engineSettingsValid ? "Correct all engine settings before continuing." : undefined}
                onClick={onClose}>Done</button>
        </fieldset>
    </div>;
}
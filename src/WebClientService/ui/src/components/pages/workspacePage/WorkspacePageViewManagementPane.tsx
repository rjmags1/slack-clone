import { useState } from 'react'
import { useMove } from 'react-aria'

const bodySize = () => document.body.getBoundingClientRect().width

const MAX_WIDTH_RATIO = 0.35
const SEPARATOR_WIDTH = 4 //px

function WorkspacePageViewManagementPane() {
    const [separatorX, setSeparatorX] = useState<number>(bodySize() * 0.33)
    const { moveProps } = useMove({
        onMove: (e) => {
            setSeparatorX((prevX) => {
                let x = prevX + e.deltaX
                const ratio = x / bodySize()
                if (ratio > MAX_WIDTH_RATIO) {
                    x = bodySize() * MAX_WIDTH_RATIO
                }
                document.getElementById(
                    'workspace-page-view-management-pane'
                )!.style.width = `${Math.max(SEPARATOR_WIDTH, x)}px`
                return x
            })
        },
    })

    return (
        <div
            id="workspace-page-view-management-pane"
            {...moveProps}
            className="flex h-full"
            tabIndex={0}
            style={{ width: separatorX }}
        >
            <div className="shrink-1 h-full grow bg-black"></div>
            <div
                className="h-full min-w-[4px] max-w-[4px] shrink-0 
                    bg-zinc-500 hover:cursor-ew-resize"
            ></div>
        </div>
    )
}

export default WorkspacePageViewManagementPane

import WorkspacePageViewManagementPaneHeader from './WorkspaceViewManagementPaneHeader'
import MessageTypesInteractionsSidebar from './MessageTypesInteractionsSidebarSection'
import { useEffect, useState } from 'react'
import { useMove } from 'react-aria'
import ChannelsSidebarSection from './ChannelsSidebarSection'
import DirectMessagesSidebarSection from './DirectMessagesSidebarSection'
import StarredSidebarSection from './StarredSidebarSection'

const bodySize = () => document.body.getBoundingClientRect().width

const MAX_WIDTH_RATIO = 0.35

function WorkspacePageViewManagementPane() {
    const [separatorX, setSeparatorX] = useState<number>()
    const { moveProps } = useMove({
        onMove: (e) => {
            setSeparatorX((prevX) => {
                if (prevX === undefined) return undefined
                let x = prevX + e.deltaX
                const ratio = x / bodySize()
                if (ratio > MAX_WIDTH_RATIO) {
                    x = bodySize() * MAX_WIDTH_RATIO
                }
                document.getElementById(
                    'workspace-page-view-management-pane'
                )!.style.width = `${Math.max(5, x)}px`
                return x
            })
        },
    })

    useEffect(() => {
        if (separatorX === undefined) {
            setSeparatorX(
                document
                    .getElementById('workspace-page-view-management-pane')!
                    .getBoundingClientRect().width
            )
        }
    }, [separatorX])

    return (
        <div
            id="workspace-page-view-management-pane"
            className="flex h-full w-max text-white"
            tabIndex={0}
            style={{ width: separatorX }}
        >
            <div className="flex h-full w-max grow flex-col justify-start">
                <WorkspacePageViewManagementPaneHeader />
                <MessageTypesInteractionsSidebar />
                <div className="flex h-full w-full grow flex-col overflow-hidden">
                    <ChannelsSidebarSection />
                    <DirectMessagesSidebarSection />
                    <StarredSidebarSection />
                </div>
            </div>
            <div
                {...moveProps}
                className="flex h-full min-w-[5px] max-w-[5px] 
                    shrink-0 justify-center hover:cursor-ew-resize"
            >
                <div className="h-full min-w-[1px] max-w-[1px] bg-zinc-500" />
            </div>
        </div>
    )
}

export default WorkspacePageViewManagementPane

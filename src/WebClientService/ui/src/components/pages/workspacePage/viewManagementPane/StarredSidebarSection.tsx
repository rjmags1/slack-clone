import { useState } from 'react'
import Button from '../../../lib/Button'
import WorkspaceSidebarStarredList from './WorkspaceSidebarStarredList'

function StarredSidebarSection() {
    const [expand, setExpand] = useState(false)

    const open = () => {
        // TODO
        setExpand(true)
    }
    const close = () => {
        setExpand(false)
        // TODO
    }
    return (
        <div className="flex w-full flex-col overflow-hidden border-b border-zinc-500 p-2">
            <div
                className="min-w-max justify-between
                    gap-x-2 text-xs"
            >
                <Button onClick={expand ? close : open} className="min-w-max">
                    {`${expand ? '⌃' : '⌄'} Starred`}
                </Button>
            </div>
            {expand && <WorkspaceSidebarStarredList />}
        </div>
    )
}

export default StarredSidebarSection

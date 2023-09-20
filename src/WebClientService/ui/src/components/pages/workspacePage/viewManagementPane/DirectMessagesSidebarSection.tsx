import { useState } from 'react'
import Button from '../../../lib/Button'
import WorkspaceSidebarDirectMessagesList from './WorkspaceSidebarDirectMessagesList'

function DirectMessagesSidebarSection() {
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
                className="flex w-full min-w-max 
                    justify-between gap-x-2 text-xs"
            >
                <Button onClick={expand ? close : open}>
                    {`${expand ? '⌃' : '⌄'} Direct messages`}
                </Button>
                <Button
                    className="text-xxs h-fit rounded-md bg-sky-800 
                        px-1 py-[1.5px] hover:bg-sky-950"
                    onClick={() => alert('not implemented')}
                >
                    + New DM
                </Button>
            </div>
            {expand && <WorkspaceSidebarDirectMessagesList />}
        </div>
    )
}

export default DirectMessagesSidebarSection

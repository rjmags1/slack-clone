import { useState } from 'react'
import Button from '../../../lib/Button'
import WorkspaceSidebarChannelsList from './WorkspaceSidebarChannelsList'
import { WorkspacePageSidebarChannelsFragment$key } from '../../../../relay/fragments/__generated__/WorkspacePageSidebarChannelsFragment.graphql'

type ChannelsSidebarSectionProps = {
    channels: WorkspacePageSidebarChannelsFragment$key
}

function ChannelsSidebarSection({ channels }: ChannelsSidebarSectionProps) {
    const [expand, setExpand] = useState(true)

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
                className="flex w-full min-w-max shrink-0
                    justify-between gap-x-2 text-xs"
            >
                <Button
                    onClick={expand ? close : open}
                    className="min-w-max shrink-0"
                >
                    {`${expand ? '⌃' : '⌄'} Channels`}
                </Button>
                <Button
                    className="text-xxs h-fit min-w-max shrink-0 
                        rounded-md bg-sky-800 px-1 py-[1.5px] hover:bg-sky-950"
                    onClick={() => alert('not implemented')}
                >
                    + New channel
                </Button>
            </div>
            {expand && <WorkspaceSidebarChannelsList channels={channels} />}
        </div>
    )
}

export default ChannelsSidebarSection

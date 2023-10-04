import Button from '../../../../lib/Button'
import type { ChannelViewPaneContentHeaderFragment$key } from '../../../../../relay/fragments/__generated__/ChannelViewPaneContentHeaderFragment.graphql'
import ChannelViewPaneContentHeaderFragment from '../../../../../relay/fragments/ChannelViewPaneContentHeader'
import { useFragment } from 'react-relay'

type ChannelViewPaneContentHeaderProps = {
    headerInfo: ChannelViewPaneContentHeaderFragment$key
}

function ChannelViewPaneContentHeader({
    headerInfo,
}: ChannelViewPaneContentHeaderProps) {
    const data = useFragment(ChannelViewPaneContentHeaderFragment, headerInfo)

    return (
        <div
            className="flex h-max w-full shrink-0 items-center justify-start 
                gap-x-1 truncate border-b border-b-zinc-500 p-2 
                font-medium text-white"
        >
            {data.name}
            <Button onClick={() => alert('not implemented')}>⌄</Button>
            {data.private && (
                <span
                    className="text-xxs ml-2 h-fit rounded-md bg-rose-800 
                        px-1 py-[1px] font-light"
                >
                    Private
                </span>
            )}
        </div>
    )
}

export default ChannelViewPaneContentHeader

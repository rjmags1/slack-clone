import { usePaginationFragment } from 'react-relay'
import ChannelMessagesFragment from '../../../../../relay/fragments/ChannelMessages'
import { ChannelMessagesFragment$key } from '../../../../../relay/fragments/__generated__/ChannelMessagesFragment.graphql'
import List from '../../../../lib/List'
import ChannelMessage from './ChannelMessage'
import { Item } from 'react-stately'
import { useEffect, useRef } from 'react'

type ChannelViewPaneMessageListProps = {
    messages: ChannelMessagesFragment$key
}

function ChannelViewPaneMessageList({
    messages,
}: ChannelViewPaneMessageListProps) {
    const { data, loadNext, hasNext, isLoadingNext } = usePaginationFragment(
        ChannelMessagesFragment,
        messages
    )
    const lastMessageId =
        data.messages.edges[data.messages.edges.length - 1].node.id
    const observerRef = useRef<IntersectionObserver | null>(null)

    useEffect(() => {
        const firstMessageId = data.messages.edges[0]?.node.id
        document.getElementById(firstMessageId)!.scrollIntoView({
            behavior: 'instant',
            block: 'end',
            inline: 'nearest',
        } as any)
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [])

    useEffect(() => {
        if (!hasNext || isLoadingNext) return

        if (observerRef.current) observerRef.current.disconnect()

        observerRef.current = new IntersectionObserver((entries) => {
            for (const entry of entries) {
                if (
                    entry.target.id === lastMessageId &&
                    entry.intersectionRatio > 0
                ) {
                    loadNext(10)
                }
            }
        })
        observerRef.current.observe(document.getElementById(lastMessageId)!)

        return () => observerRef.current!.disconnect()
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [hasNext, isLoadingNext, lastMessageId])

    return (
        <List
            items={(data.messages.edges as any).toReversed()}
            className="w-full grow overflow-hidden overflow-y-auto"
        >
            {(item) => (
                <Item key={(item as any).node.id}>
                    <ChannelMessage
                        id={(item as any).node.id}
                        message={(item as any).node}
                    />
                </Item>
            )}
        </List>
    )
}

export default ChannelViewPaneMessageList

import { usePaginationFragment } from 'react-relay'
import ChannelMessagesFragment from '../../../../../relay/fragments/ChannelMessages'
import { ChannelMessagesFragment$key } from '../../../../../relay/fragments/__generated__/ChannelMessagesFragment.graphql'
import List from '../../../../lib/List'
import ChannelMessage from './ChannelMessage'
import { Item } from 'react-stately'
import useScrollIntoView from '../../../../../hooks/useScrollIntoView'
import useIntersectionObserver from '../../../../../hooks/useIntersectionObserver'

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
    const firstMessageId = data.messages.edges[0]?.node.id
    const lastMessageId =
        data.messages.edges[data.messages.edges.length - 1].node.id
    useScrollIntoView(
        firstMessageId,
        {
            behavior: 'instant' as ScrollBehavior,
            block: 'end' as ScrollLogicalPosition,
            inline: 'nearest' as ScrollLogicalPosition,
        },
        []
    )
    useIntersectionObserver(
        lastMessageId,
        [hasNext, isLoadingNext, lastMessageId],
        [!hasNext, isLoadingNext],
        () => loadNext(10)
    )

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

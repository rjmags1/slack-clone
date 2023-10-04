import { usePaginationFragment } from 'react-relay'
import { DirectMessagesFragment$key } from '../../../../../relay/fragments/__generated__/DirectMessagesFragment.graphql'
import DirectMessagesFragment from '../../../../../relay/fragments/DirectMessages'
import useScrollIntoView from '../../../../../hooks/useScrollIntoView'
import useIntersectionObserver from '../../../../../hooks/useIntersectionObserver'
import List from '../../../../lib/List'
import { Item } from 'react-stately'
import DirectMessage from './DirectMessage'

type DmGroupViewPaneMessageListProps = {
    messages: DirectMessagesFragment$key
}

function DmGroupViewPaneMessageList({
    messages,
}: DmGroupViewPaneMessageListProps) {
    const { data, hasNext, loadNext, isLoadingNext } = usePaginationFragment(
        DirectMessagesFragment,
        messages
    )
    const firstMessageId = data.messages.edges[0]?.node.id
    const lastMessageId =
        data.messages.edges[data.messages.edges.length - 1].node.id
    //useScrollIntoView(
    //firstMessageId,
    //{
    //behavior: 'instant' as ScrollBehavior,
    //block: 'end' as ScrollLogicalPosition,
    //inline: 'nearest' as ScrollLogicalPosition,
    //},
    //[]
    //)
    //useIntersectionObserver(
    //lastMessageId,
    //[hasNext, isLoadingNext, lastMessageId],
    //[!hasNext, isLoadingNext],
    //() => loadNext(10)
    //)

    return (
        <List
            items={(data.messages.edges as any).toReversed()}
            className="w-full grow overflow-hidden overflow-y-auto"
        >
            {(item) => (
                <Item key={(item as any).node.id}>
                    <DirectMessage
                        id={(item as any).node.id}
                        message={(item as any).node}
                    />
                </Item>
            )}
        </List>
    )
}

export default DmGroupViewPaneMessageList

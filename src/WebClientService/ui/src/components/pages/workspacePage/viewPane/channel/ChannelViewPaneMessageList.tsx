import { useFragment } from 'react-relay'
import ChannelMessagesFragment from '../../../../../relay/fragments/ChannelMessages'
import { ChannelMessagesFragment$key } from '../../../../../relay/fragments/__generated__/ChannelMessagesFragment.graphql'
import List from '../../../../lib/List'
import ChannelMessage from './ChannelMessage'
import { Item } from 'react-stately'

type ChannelViewPaneMessageListProps = {
    messages: ChannelMessagesFragment$key
}

function ChannelViewPaneMessageList({
    messages,
}: ChannelViewPaneMessageListProps) {
    const data = useFragment(ChannelMessagesFragment, messages)
    return (
        <List
            items={data.messages.edges}
            className="h-full w-full overflow-y-auto"
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

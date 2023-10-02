import graphql from 'babel-plugin-relay/macro'

const ChannelQuery = graphql`
    query ChannelMessagesQuery(
        $userId: ID!
        $channelId: ID!
        $messagesFilter: MessagesFilter!
    ) {
        viewChannel(userId: $userId, channelId: $channelId) {
            id
            allowThreads
            allowedPostersMask
            ...ChannelViewPaneContentHeaderFragment
            ...ChannelMessagesFragment @arguments(filter: $messagesFilter)
        }
    }
`

export default ChannelQuery

import graphql from 'babel-plugin-relay/macro'

const ChannelMessagesFragment = graphql`
    fragment ChannelMessagesFragment on Channel
    @refetchable(queryName: "ChannelMessagesListPaginationQuery")
    @argumentDefinitions(
        first: { type: "Int", defaultValue: 50 }
        after: { type: "ID" }
        filter: { type: "MessagesFilter!" }
    ) {
        messages(filter: $filter, first: $first, after: $after)
            @connection(key: "ChannelMessagesFragment_messages") {
            totalEdges
            pageInfo {
                startCursor
                endCursor
                hasNextPage
                hasPreviousPage
            }
            edges {
                cursor
                node {
                    id
                    ...ChannelMessageFragment
                }
            }
        }
    }
`

export default ChannelMessagesFragment

import graphql from 'babel-plugin-relay/macro'

const DirectMessagesFragment = graphql`
    fragment DirectMessagesFragment on DirectMessageGroup
    @refetchable(queryName: "DirectMessagesListPaginationQuery")
    @argumentDefinitions(
        first: { type: "Int", defaultValue: 10 }
        after: { type: "ID" }
    ) {
        messages(first: $first, after: $after) {
            totalEdges
            pageInfo {
                startCursor
                endCursor
                hasNextPage
                hasPreviousPage
            }
            edges {
                node {
                    ...DirectMessageFragment
                }
            }
        }
    }
`

export default DirectMessagesFragment

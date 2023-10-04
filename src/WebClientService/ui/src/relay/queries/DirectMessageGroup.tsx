import graphql from 'babel-plugin-relay/macro'

const DirectMessageGroupQuery = graphql`
    query DirectMessageGroupQuery($userId: ID!, $directMessageGroupId: ID!) {
        viewDirectMessageGroup(
            userId: $userId
            directMessageGroupId: $directMessageGroupId
        ) {
            id
            ...DirectMessageViewPaneContentHeaderFragment
            ...DirectMessagesFragment
        }
    }
`

export default DirectMessageGroupQuery

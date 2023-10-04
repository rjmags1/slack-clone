import graphql from 'babel-plugin-relay/macro'

const DirectMessageFragment = graphql`
    fragment DirectMessageFragment on Message {
        id
        user {
            id
            username
            avatar {
                id
                storeKey
            }
        }
        content
        sentAtUTC
        lastEditUTC
        files {
            id
            storeKey
        }
        laterFlag {
            id
        }
        mentions {
            mentioned {
                id
            }
        }
        reactions {
            id
            count
            emoji
            userReactionId
        }
        replyToId
    }
`

export default DirectMessageFragment

import graphql from 'babel-plugin-relay/macro'

const DirectMessageFragment = graphql`
    fragment DirectMessageFragment on Message {
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
            mentionedId
        }
        reactions {
            id
            emoji
            user {
                id
            }
        }
        replyToId
    }
`

export default DirectMessageFragment

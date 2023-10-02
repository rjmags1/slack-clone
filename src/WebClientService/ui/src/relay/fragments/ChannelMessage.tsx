import graphql from 'babel-plugin-relay/macro'

const ChannelMessageFragment = graphql`
    fragment ChannelMessageFragment on Message {
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
        threadId
    }
`

export default ChannelMessageFragment

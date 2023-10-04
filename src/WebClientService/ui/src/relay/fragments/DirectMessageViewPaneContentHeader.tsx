import graphql from 'babel-plugin-relay/macro'

const DirectMessageViewPaneContentHeaderFragment = graphql`
    fragment DirectMessageViewPaneContentHeaderFragment on DirectMessageGroup {
        members {
            id
            user {
                id
                username
            }
        }
    }
`

export default DirectMessageViewPaneContentHeaderFragment

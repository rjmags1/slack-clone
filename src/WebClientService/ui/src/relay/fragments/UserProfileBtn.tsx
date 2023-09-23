import graphql from 'babel-plugin-relay/macro'

const UserProfileBtnFragment = graphql`
    fragment UserProfileBtnFragment on User {
        id
        username
        onlineStatus
        avatar {
            id
            name
            storeKey
        }
    }
`

export default UserProfileBtnFragment

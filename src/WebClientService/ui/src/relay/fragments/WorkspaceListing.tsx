import graphql from 'babel-plugin-relay/macro'

const WorkspaceListingFragment = graphql`
    fragment WorkspaceListingFragment on Workspace {
        createdAt
        description
        numMembers
        avatar {
            id
            storeKey
        }
    }
`

export default WorkspaceListingFragment

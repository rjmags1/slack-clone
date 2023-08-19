import graphql from 'babel-plugin-relay/macro'

const CreateWorkspaceMutation = graphql`
    mutation CreateWorkspaceFormMutation(
        $workspace: WorkspaceInput!
        $creatorId: ID!
    ) {
        createWorkspace(workspace: $workspace, creatorId: $creatorId) {
            id
            createdAt
            description
            name
            avatar {
                id
                storeKey
            }
            numMembers
        }
    }
`

export default CreateWorkspaceMutation

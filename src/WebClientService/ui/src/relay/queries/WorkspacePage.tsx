import graphql from 'babel-plugin-relay/macro'

const WorkspacePageDataQuery = graphql`
    query WorkspacePageQuery($userId: ID!, $workspaceId: ID!) {
        workspacePageData(userId: $userId) {
            id
            user(id: $userId) {
                ...UserProfileBtnFragment
            }
            workspace(id: $workspaceId) {
                ...WorkspacePageSidebarHeaderFragment
            }
            ...WorkspacePageSidebarChannelsFragment
            ...WorkspacePageSidebarDirectMessageGroupsFragment
            ...WorkspacePageSidebarStarredsFragment
        }
    }
`

export default WorkspacePageDataQuery

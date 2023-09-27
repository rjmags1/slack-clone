import graphql from 'babel-plugin-relay/macro'

const WorkspacePageDataQuery = graphql`
    query WorkspacePageQuery(
        $userId: ID!
        $workspaceId: ID!
        $channelsFilter: ChannelsFilter!
    ) {
        workspacePageData(userId: $userId) {
            id
            user(id: $userId) {
                ...UserProfileBtnFragment
            }
            workspace(id: $workspaceId) {
                ...WorkspacePageSidebarHeaderFragment
            }
            ...WorkspacePageSidebarChannelsFragment
                @arguments(filter: $channelsFilter)
            ...WorkspacePageSidebarDirectMessageGroupsFragment
            ...WorkspacePageSidebarStarredsFragment
        }
    }
`

export default WorkspacePageDataQuery

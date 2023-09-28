import graphql from 'babel-plugin-relay/macro'

const WorkspacePageDataQuery = graphql`
    query WorkspacePageQuery(
        $userId: ID!
        $workspaceId: ID!
        $channelsFilter: ChannelsFilter!
        $directMessageGroupsFilter: DirectMessageGroupsFilter!
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
                @arguments(filter: $directMessageGroupsFilter)
            ...WorkspacePageSidebarStarredsFragment
        }
    }
`

export default WorkspacePageDataQuery

import graphql from 'babel-plugin-relay/macro'

const WorkspacePageSidebarChannelFragment = graphql`
    fragment WorkspacePageSidebarChannelFragment on Channel {
        name
    }
`

export default WorkspacePageSidebarChannelFragment

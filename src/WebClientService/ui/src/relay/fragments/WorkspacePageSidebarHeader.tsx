import graphql from 'babel-plugin-relay/macro'

const WorkspacePageSidebarHeaderFragment = graphql`
    fragment WorkspacePageSidebarHeaderFragment on Workspace {
        id
        name
    }
`

export default WorkspacePageSidebarHeaderFragment

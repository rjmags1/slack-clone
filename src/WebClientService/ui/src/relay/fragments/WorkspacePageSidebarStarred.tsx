import graphql from 'babel-plugin-relay/macro'

const WorkspacePageSidebarStarredFragment = graphql`
    fragment WorkspacePageSidebarStarredFragment on Group {
        name
        __typename
    }
`

export default WorkspacePageSidebarStarredFragment

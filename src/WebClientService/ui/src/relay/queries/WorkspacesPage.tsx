import graphql from 'babel-plugin-relay/macro'

const WorkspacesPageDataQuery = graphql`
    query WorkspacesPageQuery($userId: ID!) {
        workspacesPageData {
            id
            user(id: $userId) {
                id
                createdAt
                personalInfo {
                    email
                    userNotificationsPreferences {
                        notifSound
                    }
                }
            }
            ...WorkspacesListFragment
        }
    }
`

export default WorkspacesPageDataQuery

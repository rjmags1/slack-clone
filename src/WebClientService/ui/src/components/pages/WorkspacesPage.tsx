import graphql from 'babel-plugin-relay/macro'
import { useLazyLoadQuery } from 'react-relay'
import type { WorkspacesPageQuery as WorkspacesPageQueryType } from './__generated__/WorkspacesPageQuery.graphql'
import AuthGuard from '../session/AuthGuard'

const WorkspacesPageDataQuery = graphql`
    query WorkspacesPageQuery(
        $userId: ID!
        $workspacesFilter: WorkspacesFilter!
    ) {
        workspacesPageData(
            userId: $userId
            workspacesFilter: $workspacesFilter
        ) {
            user {
                id
                createdAt
                personalInfo {
                    email
                    userNotificationsPreferences {
                        notifSound
                    }
                }
            }
            workspaces {
                totalEdges
                pageInfo {
                    startCursor
                    endCursor
                    hasNextPage
                    hasPreviousPage
                }
                edges {
                    node {
                        id
                        createdAt
                        description
                        name
                        numMembers
                        avatar {
                            id
                            storeKey
                        }
                    }
                }
            }
        }
    }
`

function WorkspacesPage() {
    const data = useLazyLoadQuery<WorkspacesPageQueryType>(
        WorkspacesPageDataQuery,
        {
            userId: 'e6560874-6d15-4d94-bc53-f2240ab364e0',
            workspacesFilter: {
                cursor: {
                    first: 2,
                },
                userId: 'e6560874-6d15-4d94-bc53-f2240ab364e0',
            },
        }
    )

    console.log(data)

    return (
        <AuthGuard>
            <div>workspaces page</div>
        </AuthGuard>
    )
}

export default WorkspacesPage

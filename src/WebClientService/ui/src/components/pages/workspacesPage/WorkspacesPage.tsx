import graphql from 'babel-plugin-relay/macro'
import { useLazyLoadQuery } from 'react-relay'
import type { WorkspacesPageQuery as WorkspacesPageQueryType } from './__generated__/WorkspacesPageQuery.graphql'
import WorkspacesPageNavbar from './WorkspacesPageNavbar'
import WorkspacesList from './WorkspacesList'
import WorkspacesSearchbar from './WorkspacesSearchbar'

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

    return (
        <div className="h-full w-full">
            <WorkspacesPageNavbar />
            <div
                className="flex h-[calc(100%_-_2.5rem)] flex-col 
                    items-center justify-center p-4"
            >
                <header className="my-3">
                    <h2 className="text-3xl font-bold text-white">
                        Welcome back!
                    </h2>
                </header>
                <WorkspacesList />
            </div>
        </div>
    )
}

export default WorkspacesPage

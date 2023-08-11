import graphql from 'babel-plugin-relay/macro'
import { useLazyLoadQuery } from 'react-relay'
import type { WorkspacesPageQuery as WorkspacesPageQueryType } from './__generated__/WorkspacesPageQuery.graphql'
import WorkspacesPageNavbar from './WorkspacesPageNavbar'
import WorkspacesList from './WorkspacesList'
import { useContext, useEffect } from 'react'
import { SessionContext } from '../../session/SessionProvider'
import Loading from '../../lib/Loading'

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
                ...WorkspacesListFragment
            }
        }
    }
`

const MAX_WORKSPACES_PER_USER = 100

function WorkspacesPage() {
    const claims = useContext(SessionContext)!
    const sub = claims.filter((c) => c.type === 'sub')[0]?.value as string
    const data = useLazyLoadQuery<WorkspacesPageQueryType>(
        WorkspacesPageDataQuery,
        {
            userId: sub,
            workspacesFilter: {
                cursor: {
                    first: MAX_WORKSPACES_PER_USER,
                },
                userId: sub,
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
                {data.workspacesPageData === null ? (
                    <Loading />
                ) : (
                    <WorkspacesList
                        workspaces={data.workspacesPageData.workspaces}
                    />
                )}
            </div>
        </div>
    )
}

export default WorkspacesPage

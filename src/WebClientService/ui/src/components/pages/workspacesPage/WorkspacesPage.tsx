import { useLazyLoadQuery } from 'react-relay'
import type { WorkspacesPageQuery as WorkspacesPageQueryType } from '../../../relay/queries/__generated__/WorkspacesPageQuery.graphql'
import WorkspacesPageNavbar from './WorkspacesPageNavbar'
import WorkspacesList from './WorkspacesList'
import { useContext, createContext } from 'react'
import { SessionContext, getSubClaim } from '../../session/SessionProvider'
import WorkspacesPageDataQuery from '../../../relay/queries/WorkspacesPage'

export const WorkspacesPageIdContext = createContext<string | null>(null)

function WorkspacesPage() {
    const claims = useContext(SessionContext)!
    const sub = getSubClaim(claims)
    const data = useLazyLoadQuery<WorkspacesPageQueryType>(
        WorkspacesPageDataQuery,
        {
            userId: sub,
        }
    )

    return (
        <WorkspacesPageIdContext.Provider
            value={data?.workspacesPageData?.id || null}
        >
            <div className="h-full w-full">
                <WorkspacesPageNavbar />
                <div
                    className="flex h-[calc(100%_-_2.5rem)] flex-col 
                    items-center justify-start pb-6"
                >
                    <header className="my-10">
                        <h2 className="text-5xl font-bold text-white">
                            Welcome back!
                        </h2>
                    </header>
                    {data.workspacesPageData && (
                        <WorkspacesList workspaces={data.workspacesPageData} />
                    )}
                </div>
            </div>
        </WorkspacesPageIdContext.Provider>
    )
}

export default WorkspacesPage

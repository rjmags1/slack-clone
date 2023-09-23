import { WorkspacePageQuery$data } from '../../../../relay/queries/__generated__/WorkspacePageQuery.graphql'
import UserProfileBtn from './UserProfileBtn'
import WorkspaceSearchBar from './WorkspaceSearchBar'

type WorkspacePageNavbarProps = {
    data: WorkspacePageQuery$data
}

function WorkspacePageNavbar({ data }: WorkspacePageNavbarProps) {
    return (
        <nav
            className="sticky top-0 flex h-10 w-full items-center 
                justify-center bg-sky-950 px-8 text-white"
        >
            <WorkspaceSearchBar />
            <UserProfileBtn
                user={data.workspacePageData!.user!}
                className="absolute right-0 mr-8 
                    rounded-full outline-none"
            />
        </nav>
    )
}

export default WorkspacePageNavbar

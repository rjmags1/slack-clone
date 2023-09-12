import UserProfileBtn from './UserProfileBtn'
import WorkspaceSearchBar from './WorkspaceSearchBar'

function WorkspacePageNavbar() {
    return (
        <nav
            className="sticky top-0 flex h-10 w-full items-center 
                justify-center bg-sky-950 px-8 text-white"
        >
            <WorkspaceSearchBar />
            <UserProfileBtn className="absolute right-0 mr-8" />
        </nav>
    )
}

export default WorkspacePageNavbar

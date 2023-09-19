import WorkspacePageNavbar from './navbar/WorkspacePageNavbar'
import WorkspacePageViewManagementPane from './viewManagementPane/WorkspacePageViewManagementPane'
import WorkspacePageViewPane from './viewPane/WorkspacePageViewPane'

function WorkspacePage() {
    return (
        <div className="h-full w-full">
            <WorkspacePageNavbar />
            <div className="flex h-[calc(100%_-_2.5rem)] w-full">
                <WorkspacePageViewManagementPane />
                <WorkspacePageViewPane />
            </div>
        </div>
    )
}

export default WorkspacePage

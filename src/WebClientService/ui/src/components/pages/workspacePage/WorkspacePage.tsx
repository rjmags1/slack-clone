import WorkspacePageNavbar from './WorkspacePageNavbar'
import WorkspacePageViewManagementPane from './WorkspacePageViewManagementPane'
import WorkspacePageViewPane from './WorkspacePageViewPane'

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

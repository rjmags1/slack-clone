import { useParams } from 'react-router-dom'
import WorkspacePageNavbar from './WorkspacePageNavbar'

function WorkspacePage() {
    const { workspaceId } = useParams()
    return (
        <div className="h-full w-full">
            <WorkspacePageNavbar />
            <div className="h-[calc(100%_-_2.5rem)] w-full"></div>
        </div>
    )
}

export default WorkspacePage

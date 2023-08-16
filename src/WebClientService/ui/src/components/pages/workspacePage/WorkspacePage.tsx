import { useParams } from 'react-router-dom'

function WorkspacePage() {
    const { workspaceId } = useParams()
    return (
        <div>
            <h1>Workspace Id: {workspaceId}</h1>
        </div>
    )
}

export default WorkspacePage

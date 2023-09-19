import Button from '../../../lib/Button'
import WorkspaceAddMemberBtn from './WorkspaceAddMemberBtn'
import WorkspaceSettingsBtn from './WorkspaceSettingsBtn'

function WorkspacePageViewManagementPaneHeader() {
    return (
        <div className="flex w-full flex-col gap-y-1 border-b border-zinc-500 p-2">
            <div className="flex w-full justify-start gap-x-2">
                <h3 className="w-max truncate">workspace-name</h3>
                <Button onClick={() => alert('not implemented')}>⌄</Button>
            </div>
            <div className="flex w-full justify-start gap-x-2">
                <WorkspaceAddMemberBtn />
                <WorkspaceSettingsBtn />
            </div>
        </div>
    )
}

export default WorkspacePageViewManagementPaneHeader

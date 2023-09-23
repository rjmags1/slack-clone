import { useFragment } from 'react-relay'
import { WorkspacePageSidebarHeaderFragment$key } from '../../../../relay/fragments/__generated__/WorkspacePageSidebarHeaderFragment.graphql'
import Button from '../../../lib/Button'
import WorkspaceAddMemberBtn from './WorkspaceAddMemberBtn'
import WorkspaceSettingsBtn from './WorkspaceSettingsBtn'
import WorkspacePageSidebarHeaderFragment from '../../../../relay/fragments/WorkspacePageSidebarHeader'

type WorkspaceViewManagementPaneHeaderProps = {
    workspace: WorkspacePageSidebarHeaderFragment$key
}

function WorkspacePageViewManagementPaneHeader({
    workspace,
}: WorkspaceViewManagementPaneHeaderProps) {
    const data = useFragment(WorkspacePageSidebarHeaderFragment, workspace)

    return (
        <div
            className="flex w-full shrink-0 grow-0 flex-col gap-y-1 
                border-b border-zinc-500 p-2"
        >
            <div className="flex w-full justify-start gap-x-2">
                <h3 className="w-[min(220px,_30vw)] truncate">{data.name}</h3>
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

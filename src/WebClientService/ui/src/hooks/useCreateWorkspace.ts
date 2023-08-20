import { ConnectionHandler, useMutation } from 'react-relay'
import CreateWorkspaceMutation from '../relay/mutations/CreateWorkspace'
import { useContext } from 'react'
import { WorkspacesPageIdContext } from '../components/pages/workspacesPage/WorkspacesPage'
import { CreateWorkspaceFormMutation$data } from '../relay/mutations/__generated__/CreateWorkspaceFormMutation.graphql'

type CreateWorkspaceArgs = {
    sub: string
    name: string
    description: string
    avatarId: string | null
    invitedEmails: string[]
    closeModal: () => void
}

function useCreateWorkspace() {
    const [commitWorkspaceMutation] = useMutation(CreateWorkspaceMutation)
    const pageId = useContext(WorkspacesPageIdContext)

    const createWorkspace = ({
        sub,
        name,
        description,
        avatarId,
        invitedEmails,
        closeModal,
    }: CreateWorkspaceArgs) =>
        commitWorkspaceMutation({
            variables: {
                creatorId: sub,
                workspace: {
                    name,
                    description,
                    avatarId: avatarId,
                    invitedUserEmails: invitedEmails,
                },
            },
            onCompleted: (_response, errors) => {
                if (errors === null) {
                    closeModal()
                } else {
                    for (const e of errors) {
                        console.error(e)
                    }
                }
            },
            updater: (store, data) => {
                const { createWorkspace: newWorkspace } =
                    data as CreateWorkspaceFormMutation$data
                if (!pageId) return
                const pageRelayRecord = store.get(pageId)!
                const newWorkspaceRelayRecord = store.get(newWorkspace!.id)!
                const workspacesConnectionRelayRecord =
                    ConnectionHandler.getConnection(
                        pageRelayRecord,
                        'WorkspacesListFragment_workspaces'
                    )!
                const edge = ConnectionHandler.createEdge(
                    store,
                    workspacesConnectionRelayRecord,
                    newWorkspaceRelayRecord,
                    'WorkspacesConnectionEdge'
                )
                ConnectionHandler.insertEdgeBefore(
                    workspacesConnectionRelayRecord,
                    edge
                )
            },
        })

    return { createWorkspace }
}

export default useCreateWorkspace

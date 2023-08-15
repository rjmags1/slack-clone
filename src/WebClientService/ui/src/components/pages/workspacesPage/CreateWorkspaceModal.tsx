import Dialog from '../../lib/Dialog'
import CreateWorkspaceForm from './CreateWorkspaceForm'

type CreateWorkspaceModalProps = {
    close: () => void
}

function CreateWorkspaceModal({ close }: CreateWorkspaceModalProps) {
    return (
        <Dialog
            title="New Workspace"
            className="flex h-fit max-h-[80vh] min-w-[35vw] max-w-[60vw] 
                flex-col justify-center gap-y-3 rounded-md border border-white 
                bg-sky-950 p-4 text-2xl text-white outline-none drop-shadow-2xl"
            close={close}
        >
            <CreateWorkspaceForm close={close} />
        </Dialog>
    )
}

export default CreateWorkspaceModal

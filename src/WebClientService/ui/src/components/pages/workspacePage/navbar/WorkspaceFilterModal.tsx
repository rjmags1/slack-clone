import Dialog from '../../../lib/Dialog'
import WorkspaceSearchFilterForm from './WorkspaceSearchFilterForm'

type WorkspaceFilterModalProps = {
    close: () => void
}

function WorkspaceSearchFilterModal({ close }: WorkspaceFilterModalProps) {
    return (
        <Dialog
            close={close}
            title="Filter search by..."
            className="flex h-fit max-h-[80vh] min-w-[35vw] max-w-[60vw] 
                flex-col justify-center gap-y-3 rounded-md bg-sky-950
                p-4 text-2xl text-white outline-none drop-shadow-2xl"
        >
            <WorkspaceSearchFilterForm />
        </Dialog>
    )
}

export default WorkspaceSearchFilterModal

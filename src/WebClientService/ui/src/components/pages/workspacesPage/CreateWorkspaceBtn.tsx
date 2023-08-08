import ModalTriggerButton from '../../lib/ModalTriggerButton'
import CreateWorkspaceModal from './CreateWorkspaceModal'

function CreateWorkspaceBtn() {
    return (
        <ModalTriggerButton
            label="+ New Workspace"
            className="rounded-md border border-white bg-sky-700 px-2 py-1
                text-xs outline-none hover:opacity-50"
        >
            {(close) => <CreateWorkspaceModal close={close as () => void} />}
        </ModalTriggerButton>
    )
}

export default CreateWorkspaceBtn

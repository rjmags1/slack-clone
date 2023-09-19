import { OverlayTriggerState } from 'react-stately'
import UpdateUserStatusForm from './UpdateUserStatusForm'
import Modal from '../../../lib/Modal'
import Dialog from '../../../lib/Dialog'

type UpdateUserStatusModalProps = {
    close: () => void
    state: OverlayTriggerState
    overlayProps: object
}

function UpdateUserStatusModal({
    close,
    state,
    overlayProps,
}: UpdateUserStatusModalProps) {
    return (
        <Modal state={state}>
            <Dialog
                {...overlayProps}
                title="Update your status..."
                className="min-w-max rounded-md bg-sky-950 p-5
                    text-white shadow-2xl outline-none"
                close={close}
            >
                <UpdateUserStatusForm />
            </Dialog>
        </Modal>
    )
}

export default UpdateUserStatusModal

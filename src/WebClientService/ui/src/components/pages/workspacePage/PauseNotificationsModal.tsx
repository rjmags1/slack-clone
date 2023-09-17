import { OverlayTriggerState } from 'react-stately'
import Modal from '../../lib/Modal'
import Dialog from '../../lib/Dialog'
import PauseNotificationsForm from './PauseNotificationsForm'

type PauseNotificationsModalProps = {
    close: () => void
    state: OverlayTriggerState
    overlayProps: object
}

function PauseNotificationsModal({
    close,
    state,
    overlayProps,
}: PauseNotificationsModalProps) {
    return (
        <Modal state={state}>
            <Dialog
                {...overlayProps}
                title="Pause notifications until..."
                className="min-w-max rounded-md bg-sky-950 p-5
                text-white shadow-2xl outline-none"
                close={close}
            >
                <PauseNotificationsForm />
            </Dialog>
        </Modal>
    )
}

export default PauseNotificationsModal

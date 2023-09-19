import ModalTriggerButton from '../../../lib/ModalTriggerButton'
import WorkspaceSearchFilterModal from './WorkspaceFilterModal'

function WorkspaceSearchFilterBtn() {
    return (
        <div
            className="relative flex h-6 w-6 items-center justify-center 
                rounded-lg bg-zinc-300 p-[0.3rem] outline-none 
                hover:bg-zinc-400"
        >
            <img src="/filter.png" alt="" />
            <ModalTriggerButton
                className="absolute left-0 top-0 z-10 h-full w-full 
                    outline-none"
            >
                {(close) => (
                    <WorkspaceSearchFilterModal close={close as () => void} />
                )}
            </ModalTriggerButton>
        </div>
    )
}

export default WorkspaceSearchFilterBtn

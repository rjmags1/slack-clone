import Button from '../../lib/Button'

type WorkspaceSearchHistoryBtnProps = {
    toggleDropdown: () => void
}

function WorkspaceSearchHistoryBtn({
    toggleDropdown,
}: WorkspaceSearchHistoryBtnProps) {
    return (
        <Button
            onClick={toggleDropdown}
            className="h-6 w-6 rounded-lg bg-zinc-300 p-[0.3rem]
                outline-none hover:bg-zinc-400"
        >
            <img src="/history.png" alt="history" />
        </Button>
    )
}

export default WorkspaceSearchHistoryBtn

import Dropdown from '../../lib/Dropdown'
import WorkspaceSearchHistoryListing from './WorkspaceSearchHistoryListing'

type WorkspaceSearchHistoryDropdownProps = {
    searchText: string
    searchHistory: string[]
    close: () => void
}

function WorkspaceSearchHistoryDropdown({
    searchText,
    searchHistory,
    close,
}: WorkspaceSearchHistoryDropdownProps) {
    return (
        <Dropdown
            close={close}
            className="absolute top-6 z-10 mt-1 h-fit max-h-[50vh] 
                w-full overflow-y-auto rounded-md bg-zinc-300 outline-none"
            selectedClassName="bg-zinc-500"
            items={searchHistory
                .filter((s) => s.startsWith(searchText))
                .map((s) => (
                    <WorkspaceSearchHistoryListing value={s} />
                ))}
            noItemsListing={
                <div className="px-2 py-1 text-sm">No matching searches</div>
            }
        />
    )
}

export default WorkspaceSearchHistoryDropdown

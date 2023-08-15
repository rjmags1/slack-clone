import SearchField from '../../lib/SearchField'

type WorkspacesSearchBarProps = {
    searchText: string
    setSearchText: React.Dispatch<React.SetStateAction<string>>
}

function WorkspacesSearchbar({
    searchText,
    setSearchText,
}: WorkspacesSearchBarProps) {
    return (
        <div className="sticky top-0 bg-sky-950 px-6 py-2">
            <SearchField
                onChange={setSearchText}
                value={searchText}
                inputClassName="text-white text-sm font-normal w-full 
                    bg-inherit outline-none py-1"
                clearButtonClassName="text-white"
                inputContainerClassName="flex items-center justify-between"
                placeholder="Search your workspaces by name..."
            />
        </div>
    )
}

export default WorkspacesSearchbar

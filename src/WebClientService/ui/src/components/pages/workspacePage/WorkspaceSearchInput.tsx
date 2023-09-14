import SearchField from '../../lib/SearchField'

type WorkspaceSearchInputProps = {
    searchText: string
    setSearchText: React.Dispatch<React.SetStateAction<string>>
    onSubmit: () => void
}

function WorkspaceSearchInput({
    searchText,
    setSearchText,
    onSubmit,
}: WorkspaceSearchInputProps) {
    return (
        <SearchField
            onChange={setSearchText}
            value={searchText}
            containerClassName="h-full flex items-center justify-center w-full"
            inputClassName="text-sm font-normal w-full bg-inherit 
                    outline-none p-1"
            inputContainerClassName="flex items-center justify-between h-full w-full"
            placeholder="Search..."
            onSubmit={onSubmit}
        />
    )
}

export default WorkspaceSearchInput

import SearchField from '../../lib/SearchField'

type WorkspacesSearchBarProps = {
    workspaces: any[]
}

function WorkspacesSearchbar({ workspaces }: WorkspacesSearchBarProps) {
    return (
        <div className="bg-sky-950 px-6 py-2">
            <SearchField
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

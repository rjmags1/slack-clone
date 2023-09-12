import { useState } from 'react'
import SearchField from '../../lib/SearchField'
import WorkspaceSearchHistoryDropdown from './WorkspaceSearchHistoryDropdown'

type WorkspaceSearchInputProps = {
    searchText: string
    setSearchText: React.Dispatch<React.SetStateAction<string>>
}

function WorkspaceSearchInput({
    searchText,
    setSearchText,
}: WorkspaceSearchInputProps) {
    const [submittedText, setSubmittedText] = useState('')
    const onSubmit = () => {
        // TODO
    }

    return (
        <div className="h-6 w-[40%] min-w-[300px] rounded-md bg-zinc-300 text-black">
            <SearchField
                onChange={setSearchText}
                value={searchText}
                containerClassName="h-full flex items-center justify-center"
                inputClassName="text-sm font-normal w-full bg-inherit 
                    outline-none p-1"
                inputContainerClassName="flex items-center justify-between h-full w-full"
                placeholder="Search..."
                onSubmit={onSubmit}
            />
            {submittedText && submittedText === searchText && (
                <WorkspaceSearchHistoryDropdown />
            )}
        </div>
    )
}

export default WorkspaceSearchInput

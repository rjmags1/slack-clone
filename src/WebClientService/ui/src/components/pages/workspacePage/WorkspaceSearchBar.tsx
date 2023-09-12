import { useState } from 'react'
import WorkspaceSearchHistoryBtn from './WorkspaceSearchHistoryBtn'
import WorkspaceSearchFilterBtn from './WorkspaceSearchFilterBtn'
import WorkspaceSearchInput from './WorkspaceSearchInput'

function WorkspaceSearchBar() {
    const [searchText, setSearchText] = useState('')

    return (
        <div className="flex items-center justify-center gap-x-3">
            <WorkspaceSearchHistoryBtn />
            <WorkspaceSearchInput
                searchText={searchText}
                setSearchText={setSearchText}
            />
            <WorkspaceSearchFilterBtn />
        </div>
    )
}

export default WorkspaceSearchBar

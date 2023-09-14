import { useEffect, useState } from 'react'
import WorkspaceSearchHistoryBtn from './WorkspaceSearchHistoryBtn'
import WorkspaceSearchFilterBtn from './WorkspaceSearchFilterBtn'
import WorkspaceSearchInput from './WorkspaceSearchInput'
import WorkspaceSearchHistoryDropdown from './WorkspaceSearchHistoryDropdown'
import { useParams } from 'react-router-dom'
import useWorkspaceSearchHistory from '../../../hooks/useWorkspaceSearchHistory'

function WorkspaceSearchBar() {
    const [searchText, setSearchText] = useState('')
    const [renderHistoryDropdown, setRenderHistoryDropdown] = useState(false)
    const { workspaceId } = useParams()
    const { searchHistory, addSearchEntry } = useWorkspaceSearchHistory(
        workspaceId!
    )

    useEffect(() => {
        if (!renderHistoryDropdown && searchText.length > 0) {
            setRenderHistoryDropdown(true)
        }
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [searchText])

    const toggleDropdown = () => setRenderHistoryDropdown((prev) => !prev)

    const onSubmit = () => {
        addSearchEntry(searchText)
        setRenderHistoryDropdown(false)
        // TODO: redirect to search results page
    }

    useEffect(() => {
        const handleEscape = (e: KeyboardEvent) => {
            if (e.key === 'Escape') {
                setRenderHistoryDropdown(false)
            }
        }
        document.addEventListener('keyup', handleEscape)
        return () => document.removeEventListener('keyup', handleEscape)
    }, [])

    return (
        <div
            className="flex w-[40%] min-w-[300px] max-w-[600px] items-center 
                justify-center gap-x-3 overflow-visible"
        >
            <WorkspaceSearchHistoryBtn toggleDropdown={toggleDropdown} />
            <div
                className="relative flex h-6 w-full flex-col items-center 
                    justify-start overflow-visible rounded-md bg-zinc-300 text-black"
            >
                <WorkspaceSearchInput
                    searchText={searchText}
                    setSearchText={setSearchText}
                    onSubmit={onSubmit}
                />
                {renderHistoryDropdown && (
                    <WorkspaceSearchHistoryDropdown
                        close={() => setRenderHistoryDropdown(false)}
                        searchText={searchText}
                        searchHistory={searchHistory}
                    />
                )}
            </div>
            <WorkspaceSearchFilterBtn />
        </div>
    )
}

export default WorkspaceSearchBar

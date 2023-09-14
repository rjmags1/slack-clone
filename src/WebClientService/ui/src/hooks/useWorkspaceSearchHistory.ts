import { useEffect, useState } from 'react'

const HISTORY_SUFFIX = '__SEARCH_HISTORY'

function useWorkspaceSearchHistory(workspaceId: string) {
    const [searchHistory, setSearchHistory] = useState<string[]>([])
    const localStorageKey = workspaceId + HISTORY_SUFFIX

    useEffect(() => {
        if (window.localStorage.getItem(localStorageKey) === null) {
            window.localStorage.setItem(localStorageKey, '')
            setSearchHistory([])
        } else {
            const storedHistory = window.localStorage
                .getItem(localStorageKey)!
                .split(',')
            setSearchHistory(storedHistory)
        }
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [])

    const addSearchEntry = (entry: string) => {
        const updated = [entry, ...searchHistory]
        window.localStorage.setItem(localStorageKey, updated.join(','))
        setSearchHistory(updated.slice(0, 50))
    }

    return { searchHistory, addSearchEntry }
}

export default useWorkspaceSearchHistory

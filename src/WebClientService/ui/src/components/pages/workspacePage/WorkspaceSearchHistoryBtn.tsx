import { useState } from 'react'
import Button from '../../lib/Button'
import WorkspaceSearchHistoryDropdown from './WorkspaceSearchHistoryDropdown'

function WorkspaceSearchHistoryBtn() {
    const [renderDropdown, setRenderDropdown] = useState(false)
    const onClick = () => {
        // TODO
    }

    return (
        <Button
            onClick={onClick}
            className="h-6 w-6 rounded-lg bg-zinc-300 p-[0.3rem]
                hover:bg-zinc-400"
        >
            <img src="/history.png" alt="history" />
            {renderDropdown && <WorkspaceSearchHistoryDropdown />}
        </Button>
    )
}

export default WorkspaceSearchHistoryBtn

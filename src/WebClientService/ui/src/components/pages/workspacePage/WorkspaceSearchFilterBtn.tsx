import { useState } from 'react'
import Button from '../../lib/Button'
import WorkspaceSearchFilterForm from './WorkspaceSearchFilterForm'

function WorkspaceSearchFilterBtn() {
    const [renderFilterForm, setRenderFilterForm] = useState(false)
    const onClick = () => {
        // TODO
    }

    return (
        <Button
            onClick={onClick}
            className="h-6 w-6 rounded-lg bg-zinc-300 p-[0.3rem]
                hover:bg-zinc-400"
        >
            <img src="/filter.png" alt="filter" />
            {renderFilterForm && <WorkspaceSearchFilterForm />}
        </Button>
    )
}

export default WorkspaceSearchFilterBtn

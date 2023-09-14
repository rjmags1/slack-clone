type WorkspaceSearchHistoryListingProps = {
    value: string
}

function WorkspaceSearchHistoryListing({
    value,
}: WorkspaceSearchHistoryListingProps) {
    return <div className="px-2 py-1 text-sm">{value}</div>
}

export default WorkspaceSearchHistoryListing

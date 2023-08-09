import Button from '../../lib/Button'

type WorkspaceListingProps = {
    workspace: any
}

function WorkspaceListing({ workspace }: WorkspaceListingProps) {
    const onWorkspaceOpen = () => {
        // TODO
    }

    return (
        <div
            className="flex w-full items-center gap-x-2 rounded-md
                px-4 py-2 text-white hover:bg-zinc-600"
        >
            <img
                src="/default-avatar.png"
                alt="avatar"
                style={{
                    border: '1px',
                    borderColor: 'white',
                    height: '2rem',
                    borderRadius: '9999px',
                    backgroundColor: 'black',
                }}
            />
            <div className="flex flex-col">
                <h5 className="text-sm">{workspace.name}</h5>
                <p className="text-[0.6rem]">{workspace.numMembers} members</p>
            </div>
            <Button
                onClick={onWorkspaceOpen}
                className="ml-[8rem] rounded-md bg-sky-700 px-4 py-2
                    text-xs hover:bg-sky-900"
            >
                Open Workspace
            </Button>
        </div>
    )
}

export default WorkspaceListing

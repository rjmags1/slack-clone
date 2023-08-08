import CreateWorkspaceBtn from './CreateWorkspaceBtn'

function WorkspacesPageNavbar() {
    return (
        <nav
            className="sticky top-0 flex h-10 w-full items-center 
                justify-between bg-sky-950 px-8 text-white"
        >
            <h1>slack-clone</h1>
            <CreateWorkspaceBtn />
        </nav>
    )
}

export default WorkspacesPageNavbar

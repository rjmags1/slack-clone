import WorkspaceSidebarAllChannelsBtn from './WorkspaceSidebarAllChannelsBtn'
import WorkspaceSidebarDirectMessagesBtn from './WorkspaceSidebarDirectMessagesBtn'
import WorkspaceSidebarDraftsBtn from './WorkspaceSidebarDraftsBtn'
import WorkspaceSidebarLaterBtn from './WorkspaceSidebarLaterBtn'
import WorkspaceSidebarMentionsReactionsBtn from './WorkspaceSidebarMentionsReactionsBtn'
import WorkspaceSidebarThreadsBtn from './WorkspaceSidebarThreadsBtn'
import WorkspaceSidebarUnreadsBtn from './WorkspaceSidebarUnreadsBtn'

function MessageTypesInteractionsSidebarSection() {
    return (
        <div
            className="flex w-full shrink-0 grow-0 flex-col
                border-b border-zinc-500 p-2 text-xs font-light"
        >
            <WorkspaceSidebarUnreadsBtn />
            <WorkspaceSidebarThreadsBtn />
            <WorkspaceSidebarDirectMessagesBtn />
            <WorkspaceSidebarMentionsReactionsBtn />
            <WorkspaceSidebarAllChannelsBtn />
            <WorkspaceSidebarLaterBtn />
            <WorkspaceSidebarDraftsBtn />
        </div>
    )
}

export default MessageTypesInteractionsSidebarSection

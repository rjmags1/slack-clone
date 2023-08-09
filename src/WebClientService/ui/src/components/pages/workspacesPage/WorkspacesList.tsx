import { Item } from 'react-stately'
import List from '../../lib/List'
import WorkspacesSearchbar from './WorkspacesSearchbar'
import WorkspaceListing from './WorkspaceListing'

function WorkspacesList() {
    const workspaces: any[] = Array(10).fill({
        id: 'alksdjfhaksdjfhaksjdfh',
        createdAt: 'asdfasdfasdf',
        description: 'asdfasdfasdfasdf',
        name: 'asdfasdfasdfasdf',
        numMembers: 234,
        avatar: {
            id: 'askjdnvcaksdnvas',
            storeKey: 'asdjvfoiqweqjwe',
        },
    })

    return (
        <div
            className="no-scrollbar max-h-[80%] min-h-max w-max min-w-max
                overflow-y-auto rounded-md border-0 bg-zinc-500 font-light"
        >
            <WorkspacesSearchbar workspaces={workspaces} />
            <List className="flex flex-col gap-y-2 px-4 py-2">
                {workspaces.map((w) => (
                    <Item>
                        <WorkspaceListing workspace={w} />
                    </Item>
                ))}
            </List>
        </div>
    )
}

export default WorkspacesList

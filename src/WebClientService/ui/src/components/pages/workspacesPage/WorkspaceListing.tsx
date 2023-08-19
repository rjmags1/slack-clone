import Button from '../../lib/Button'
import graphql from 'babel-plugin-relay/macro'
import type { WorkspaceListingFragment$key } from './__generated__/WorkspaceListingFragment.graphql'
import { useFragment } from 'react-relay'
import { Link } from 'react-router-dom'
import Avatar from '../../lib/Avatar'

const WorkspaceListingFragment = graphql`
    fragment WorkspaceListingFragment on Workspace {
        createdAt
        description
        numMembers
        avatar {
            id
            storeKey
        }
    }
`

type WorkspaceListingProps = {
    id: string
    workspace: WorkspaceListingFragment$key
    name: string
}

function WorkspaceListing({ workspace, name, id }: WorkspaceListingProps) {
    const data = useFragment(WorkspaceListingFragment, workspace)

    return (
        <div
            className="flex h-12 w-full items-center justify-between
                rounded-md px-4 py-2 text-white hover:bg-zinc-600"
        >
            <div className="flex flex-row items-center gap-x-2">
                <Avatar
                    src="/default-avatar.png"
                    alt="avatar"
                    className="h-[2rem] bg-black"
                />
                <div className="flex flex-col">
                    <h5 className="text-sm">{name}</h5>
                    <p className="text-[0.6rem]">{data.numMembers} members</p>
                </div>
            </div>
            <Link to={`/workspace/${id}`}>
                <Button
                    className="rounded-md bg-sky-700 px-4 py-2
                    text-xs hover:bg-sky-900"
                >
                    Open Workspace
                </Button>
            </Link>
        </div>
    )
}

export default WorkspaceListing

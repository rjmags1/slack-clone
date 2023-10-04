import { useFragment } from 'react-relay'
import DirectMessageViewPaneContentHeaderFragment from '../../../../../relay/fragments/DirectMessageViewPaneContentHeader'
import { DirectMessageViewPaneContentHeaderFragment$key } from '../../../../../relay/fragments/__generated__/DirectMessageViewPaneContentHeaderFragment.graphql'
import {
    SessionContext,
    getSubClaim,
} from '../../../../session/SessionProvider'
import { useContext } from 'react'

type DmGroupViewPaneContentHeaderProps = {
    headerInfo: DirectMessageViewPaneContentHeaderFragment$key
}

function DmGroupViewPaneContentHeader({
    headerInfo,
}: DmGroupViewPaneContentHeaderProps) {
    const claims = useContext(SessionContext)!
    const sub = getSubClaim(claims)
    const { members } = useFragment(
        DirectMessageViewPaneContentHeaderFragment,
        headerInfo
    )
    const nonUserNames = members
        .filter((m) => m.user.id !== sub)
        .map((m) => m.user.username)
    const convoTitle = nonUserNames.join(', ')
    return (
        <div
            className="flex h-max w-full shrink-0 items-center justify-start 
                gap-x-1 truncate border-b border-b-zinc-500 p-2 
                font-medium text-white"
        >
            <h3 className="w-full truncate">{convoTitle}</h3>
        </div>
    )
}

export default DmGroupViewPaneContentHeader

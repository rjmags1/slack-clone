import { useState } from 'react'
import Avatar from '../../../lib/Avatar'
import Button from '../../../lib/Button'
import UserProfileDropdown from './UserProfileDropdown'
import UpdateUserStatusModal from './UpdateUserStatusModal'
import { useOverlayTriggerState } from 'react-stately'
import { useOverlayTrigger } from 'react-aria'
import PauseNotificationsModal from './PauseNotificationsModal'
import { useFragment } from 'react-relay'
import UserProfileBtnFragment from '../../../../relay/fragments/UserProfileBtn'
import type { UserProfileBtnFragment$key } from '../../../../relay/fragments/__generated__/UserProfileBtnFragment.graphql'

type UserProfileBtnProps = {
    className?: string
    user: UserProfileBtnFragment$key
}

function UserProfileBtn({ className, user, ...props }: UserProfileBtnProps) {
    const [renderDropdown, setRenderDropdown] = useState(false)
    const userStatusModalState = useOverlayTriggerState(props)
    const pauseNotifsModalState = useOverlayTriggerState(props)
    const {
        triggerProps: userStatusModalTriggerProps,
        overlayProps: userStatusModalOverlayProps,
    } = useOverlayTrigger({ type: 'dialog' }, userStatusModalState)
    const {
        triggerProps: pauseNotifsModalTriggerProps,
        overlayProps: pauseNotifsModalOverlayProps,
    } = useOverlayTrigger({ type: 'dialog' }, pauseNotifsModalState)
    const data = useFragment(UserProfileBtnFragment, user)

    return (
        <>
            <Button
                onClick={() => setRenderDropdown((prev) => !prev)}
                className={className}
            >
                <Avatar
                    src="/default-avatar.png"
                    alt="profile"
                    className="h-[1.6rem] border border-solid border-zinc-300 
                        bg-zinc-300"
                />
                {renderDropdown && (
                    <UserProfileDropdown
                        user={data}
                        close={() => setRenderDropdown(false)}
                        renderStatusModal={() =>
                            userStatusModalState.setOpen(true)
                        }
                        userStatusTriggerProps={userStatusModalTriggerProps}
                        renderPauseNotifsModal={() =>
                            pauseNotifsModalState.setOpen(true)
                        }
                        pauseNotifsTriggerProps={pauseNotifsModalTriggerProps}
                    />
                )}
            </Button>
            {userStatusModalState.isOpen && (
                <UpdateUserStatusModal
                    overlayProps={userStatusModalOverlayProps}
                    state={userStatusModalState}
                    close={() => {}}
                />
            )}
            {pauseNotifsModalState.isOpen && (
                <PauseNotificationsModal
                    overlayProps={pauseNotifsModalOverlayProps}
                    state={pauseNotifsModalState}
                    close={() => {}}
                />
            )}
        </>
    )
}

export default UserProfileBtn

import { AriaButtonProps } from 'react-aria'
import Dropdown from '../../../lib/Dropdown'
import PauseNotificationsOption from './PauseNotificationsOption'
import PreferencesOption from './PreferencesOption'
import ProfileOption from './ProfileOption'
import SignoutOption from './SignoutOption'
import UpdateUserStatusOption from './UpdateUserStatusOption'
import { UserProfileBtnFragment$data } from '../../../../relay/fragments/__generated__/UserProfileBtnFragment.graphql'
import UserBanner from './UserBanner'

type UserProfileDropdownProps = {
    user: UserProfileBtnFragment$data
    close: () => void
    renderStatusModal: () => void
    userStatusTriggerProps: AriaButtonProps<'button'>
    renderPauseNotifsModal: () => void
    pauseNotifsTriggerProps: AriaButtonProps<'button'>
}

function UserProfileDropdown({
    close,
    renderStatusModal,
    userStatusTriggerProps,
    renderPauseNotifsModal,
    pauseNotifsTriggerProps,
    user,
}: UserProfileDropdownProps) {
    return (
        <Dropdown
            close={close}
            className="absolute right-0 top-0 mt-9 min-w-fit
                rounded-md bg-sky-950 shadow-2xl outline-none"
            selectedClassName="bg-sky-900"
            items={[
                <UserBanner user={user} />,
                <UpdateUserStatusOption
                    onClick={renderStatusModal}
                    triggerProps={userStatusTriggerProps}
                />,
                <PauseNotificationsOption
                    onClick={renderPauseNotifsModal}
                    triggerProps={pauseNotifsTriggerProps}
                />,
                <ProfileOption />,
                <PreferencesOption />,
                <SignoutOption />,
            ]}
        />
    )
}

export default UserProfileDropdown

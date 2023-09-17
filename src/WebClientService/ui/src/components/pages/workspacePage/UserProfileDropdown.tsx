import { AriaButtonProps } from 'react-aria'
import Dropdown from '../../lib/Dropdown'
import PauseNotificationsOption from './PauseNotificationsOption'
import PreferencesOption from './PreferencesOption'
import ProfileOption from './ProfileOption'
import SignoutOption from './SignoutOption'
import UpdateUserStatusOption from './UpdateUserStatusOption'

type UserProfileDropdownProps = {
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
}: UserProfileDropdownProps) {
    return (
        <Dropdown
            close={close}
            className="absolute right-0 top-0 mt-9 min-w-fit
                rounded-md bg-sky-950 shadow-2xl outline-none"
            selectedClassName="bg-sky-900"
            items={[
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

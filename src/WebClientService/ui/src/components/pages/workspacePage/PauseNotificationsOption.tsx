import { AriaButtonProps } from 'react-aria'
import Button from '../../lib/Button'

type PauseNotificationsOptionProps = {
    onClick: () => void
    triggerProps: AriaButtonProps<'button'>
}

function PauseNotificationsOption({
    onClick,
    triggerProps,
}: PauseNotificationsOptionProps) {
    return (
        <Button
            {...triggerProps}
            onClick={onClick}
            className="w-full min-w-max px-2 py-1 text-xs font-light 
                text-white hover:underline"
        >
            Pause notifications
        </Button>
    )
}

export default PauseNotificationsOption

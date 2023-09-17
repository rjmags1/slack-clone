import { AriaButtonProps } from 'react-aria'
import Button from '../../lib/Button'

type UpdateUserStatusOptionProps = {
    onClick: () => void
    triggerProps: AriaButtonProps<'button'>
}

function UpdateUserStatusOption({
    onClick,
    triggerProps,
}: UpdateUserStatusOptionProps) {
    return (
        <Button
            {...triggerProps}
            onClick={onClick}
            className="relative w-full min-w-max px-2 py-1 text-xs
                 font-light text-white hover:underline"
        >
            Update status
        </Button>
    )
}

export default UpdateUserStatusOption

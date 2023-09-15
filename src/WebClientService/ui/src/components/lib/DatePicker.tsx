import { AriaDatePickerProps, DateValue, useDatePicker } from 'react-aria'
import { useDatePickerState } from 'react-stately'
import Popover from './Popover'
import Dialog from './Dialog'
import Calendar from './Calendar'
import Button from './Button'
import DateField from './DateField'
import { useRef } from 'react'

function DatePicker(props: AriaDatePickerProps<DateValue>) {
    const state = useDatePickerState(props)
    const ref = useRef(null)
    const {
        groupProps,
        labelProps,
        fieldProps,
        buttonProps,
        dialogProps,
        calendarProps,
    } = useDatePicker(props, state, ref)

    return (
        <div className="flex min-w-max gap-x-2 font-extralight">
            <div {...labelProps}>{props.label}</div>
            <div
                {...groupProps}
                ref={ref}
                className="mr-2 flex gap-x-1 text-xs"
            >
                <DateField {...fieldProps} />
                <Button onClick={() => state.setOpen(true)} {...buttonProps}>
                    🗓
                </Button>
            </div>
            {state.isOpen && (
                <Popover
                    state={state}
                    triggerRef={ref}
                    placement="bottom start"
                >
                    <Dialog {...dialogProps}>
                        <Calendar {...calendarProps} />
                    </Dialog>
                </Popover>
            )}
        </div>
    )
}

export default DatePicker

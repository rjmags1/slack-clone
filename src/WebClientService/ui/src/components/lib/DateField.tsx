import { useDateField, useDateSegment, useLocale } from 'react-aria'
import { DateFieldState, useDateFieldState } from 'react-stately'
import { createCalendar } from '@internationalized/date'
import { useRef } from 'react'
import { AriaDateFieldOptions, DateValue } from '@react-aria/datepicker'

function DateField(props: AriaDateFieldOptions<DateValue>) {
    let { locale } = useLocale()
    let state = useDateFieldState({
        ...props,
        locale,
        createCalendar,
    })

    let ref = useRef(null)
    let { labelProps, fieldProps } = useDateField(props, state, ref)

    return (
        <div className="h-full flex-col items-center justify-center">
            {props.label && <span {...labelProps}>{props.label}</span>}
            <div
                {...fieldProps}
                ref={ref}
                className="flex h-full min-w-max items-center gap-x-[2px] 
                    border-b border-white"
            >
                {state.segments.map((segment, i) => (
                    <DateSegmentComponent
                        key={i}
                        segment={segment}
                        state={state}
                    />
                ))}
                {state.validationState === 'invalid' && (
                    <span aria-hidden="true">🚫</span>
                )}
            </div>
        </div>
    )
}

export function DateSegmentComponent({
    segment,
    state,
}: {
    segment: any
    state: DateFieldState
}) {
    let ref = useRef(null)
    let { segmentProps } = useDateSegment(segment, state, ref)

    return (
        <div {...segmentProps} ref={ref}>
            {segment.text}
        </div>
    )
}

export default DateField

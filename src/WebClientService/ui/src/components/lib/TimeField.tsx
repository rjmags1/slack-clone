import { useRef } from 'react'
import {
    AriaTimeFieldProps,
    TimeValue,
    useLocale,
    useTimeField,
} from 'react-aria'
import { useTimeFieldState } from 'react-stately'
import { DateSegmentComponent } from './DateField'

type TimeFieldProps = AriaTimeFieldProps<TimeValue> & {
    containerClassName?: string
    labelClassName?: string
}

function TimeField({
    containerClassName,
    labelClassName,
    ...props
}: TimeFieldProps) {
    let { locale } = useLocale()
    let state = useTimeFieldState({
        ...props,
        locale,
    })

    let ref = useRef(null)
    let { labelProps, fieldProps } = useTimeField(props, state, ref)

    return (
        <div
            className={
                containerClassName ||
                'flex w-full min-w-max justify-start gap-x-1 text-sm font-extralight'
            }
        >
            <span className={labelClassName || 'text-sm'} {...labelProps}>
                {props.label}
            </span>
            <div
                {...fieldProps}
                ref={ref}
                className="flex justify-start gap-x-[2px] underline"
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

export default TimeField

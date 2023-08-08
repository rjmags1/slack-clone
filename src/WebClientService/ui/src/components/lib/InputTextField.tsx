import { useRef } from 'react'
import type { AriaTextFieldProps } from 'react-aria'
import { useTextField } from 'react-aria'

type InputTextFieldProps = AriaTextFieldProps & {
    containerClassName?: string
    inputClassName?: string
    labelClassName?: string
    errorMessageClassName?: string
    descriptionClassName?: string
}

function InputTextField(props: InputTextFieldProps) {
    const {
        label,
        inputClassName,
        containerClassName,
        labelClassName,
        descriptionClassName,
        errorMessageClassName,
    } = props
    const ref = useRef(null)
    const { labelProps, inputProps, descriptionProps, errorMessageProps } =
        useTextField(props, ref)

    return (
        <div
            className={
                containerClassName ||
                'flex h-fit w-full flex-col gap-y-[2px] font-extralight'
            }
        >
            <label {...labelProps} className={labelClassName}>
                {label}
            </label>
            <input
                className={
                    inputClassName ||
                    'border-b border-b-white bg-inherit p-[2px] text-xs text-white outline-none'
                }
                {...inputProps}
                ref={ref}
            />
            {props.description && !props.errorMessage && (
                <div
                    {...descriptionProps}
                    className={descriptionClassName || 'text-xs text-rose-600'}
                >
                    {props.description}
                </div>
            )}
            {props.errorMessage && (
                <div {...errorMessageProps} className={errorMessageClassName}>
                    {props.errorMessage}
                </div>
            )}
        </div>
    )
}

export default InputTextField

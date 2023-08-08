import { SearchFieldProps, useSearchFieldState } from 'react-stately'
import Button from './Button'
import { useRef } from 'react'
import { useSearchField } from 'react-aria'

type CustomSearchFieldProps = SearchFieldProps & {
    containerClassName?: string
    inputClassName?: string
    labelClassName?: string
    clearButtonClassName?: string
}

function SearchField(props: CustomSearchFieldProps) {
    const {
        label,
        containerClassName,
        inputClassName,
        labelClassName,
        clearButtonClassName,
    } = props
    const state = useSearchFieldState(props)
    const ref = useRef(null)
    const { labelProps, inputProps, clearButtonProps } = useSearchField(
        props,
        state,
        ref
    )

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
            <div className="flex items-center justify-between border-b border-b-white text-xs text-white">
                <input
                    {...inputProps}
                    className={
                        inputClassName ||
                        'w-full bg-inherit p-[2px] outline-none'
                    }
                    ref={ref}
                />
                {state.value !== '' && (
                    <Button
                        className={clearButtonClassName + ' mr-2'}
                        {...clearButtonProps}
                    >
                        ✕
                    </Button>
                )}
            </div>
        </div>
    )
}

export default SearchField
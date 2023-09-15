import { useToggleState } from 'react-stately'
import { AriaCheckboxProps, useCheckbox } from 'react-aria'
import { useRef } from 'react'

function Checkbox(props: AriaCheckboxProps) {
    const { children } = props
    const state = useToggleState(props)
    const ref = useRef<HTMLInputElement>(null)
    const { inputProps } = useCheckbox(props, state, ref)

    return (
        <label
            className="flex items-center justify-center gap-x-1 
            font-extralight"
        >
            <input
                {...inputProps}
                ref={ref}
                className="mt-[1px] outline-none"
            />
            {children}
        </label>
    )
}

export default Checkbox

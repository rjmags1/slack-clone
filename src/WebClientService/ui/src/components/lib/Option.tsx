import { useRef } from 'react'
import { mergeProps, useFocusRing, useOption } from 'react-aria'
import { ListState } from 'react-stately'

function Option({ item, state }: { item: any; state: ListState<any> }) {
    let ref = useRef(null)
    let { optionProps, isSelected, isDisabled } = useOption(
        { key: item.key },
        state,
        ref
    )

    let { isFocusVisible, focusProps } = useFocusRing()

    return (
        <li {...mergeProps(optionProps, focusProps)} ref={ref}>
            {item.rendered}
        </li>
    )
}

export default Option

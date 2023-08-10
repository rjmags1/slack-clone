import { useRef } from 'react'
import {
    AriaGridListItemOptions,
    mergeProps,
    useFocusRing,
    useGridListItem,
} from 'react-aria'
import { ListState } from 'react-stately'
import { Node } from 'react-stately'

function ListItem({
    item,
    state,
}: {
    item: Node<object>
    state: ListState<object>
}) {
    let ref = useRef(null)
    let { rowProps, gridCellProps, isPressed } = useGridListItem(
        { node: item },
        state,
        ref
    )

    let { isFocusVisible, focusProps } = useFocusRing()

    return (
        <li
            {...mergeProps(rowProps, focusProps)}
            ref={ref}
            className={`${isPressed ? 'pressed' : ''} ${
                isFocusVisible ? 'focus-visible' : ''
            }`}
        >
            <div {...gridCellProps}>{item.rendered}</div>
        </li>
    )
}

export default ListItem

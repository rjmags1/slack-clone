import { useRef } from 'react'
import { useGridListItem } from 'react-aria'
import { ListState } from 'react-stately'
import { Node } from 'react-stately'

function ListItem({
    item,
    state,
}: {
    item: Node<object>
    state: ListState<object>
}) {
    const ref = useRef<HTMLLIElement>(null)
    const { rowProps, gridCellProps } = useGridListItem(
        { node: item },
        state,
        ref
    )

    return (
        <li {...rowProps} ref={ref}>
            <div {...gridCellProps}>{item.rendered}</div>
        </li>
    )
}

export default ListItem

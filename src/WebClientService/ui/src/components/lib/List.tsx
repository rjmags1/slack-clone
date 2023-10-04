import { useRef } from 'react'
import { useGridList } from 'react-aria'
import { ListProps, useListState } from 'react-stately'
import ListItem from './ListItem'

type CustomListProps = ListProps<object> & {
    className?: string
}

function List(props: CustomListProps) {
    const state = useListState(props)
    const ref = useRef<HTMLUListElement>(null)
    const { gridProps } = useGridList(props, state, ref)

    return (
        <ul {...gridProps} ref={ref} className={props.className}>
            {Array.from(state.collection).map((item) => (
                <ListItem key={item.key} item={item} state={state} />
            ))}
        </ul>
    )
}

export default List

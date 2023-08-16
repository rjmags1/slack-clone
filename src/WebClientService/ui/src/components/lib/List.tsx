import { useRef } from 'react'
import { useGridList } from 'react-aria'
import { ListProps, useListState } from 'react-stately'
import ListItem from './ListItem'

type CustomListProps = ListProps<object> & {
    className?: string
}

function List(props: CustomListProps) {
    let state = useListState(props)
    let ref = useRef<HTMLUListElement>(null)
    let { gridProps } = useGridList(props, state, ref)

    return (
        <ul {...gridProps} ref={ref} className={props.className}>
            {Array.from(state.collection).map((item) => (
                <ListItem key={item.key} item={item} state={state} />
            ))}
        </ul>
    )
}

export default List

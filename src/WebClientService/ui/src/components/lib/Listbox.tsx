import type { AriaListBoxProps } from 'react-aria'
import { useListState } from 'react-stately'
import { useListBox } from 'react-aria'
import { useRef } from 'react'
import Option from './Option'

type ListBoxProps<T> = AriaListBoxProps<T> & {
    listClassName?: string
    labelClassName?: string
}

function ListBox<T extends object>({
    listClassName,
    labelClassName,
    ...props
}: ListBoxProps<T>) {
    const state = useListState(props)

    const ref = useRef(null)
    const { listBoxProps, labelProps } = useListBox(props, state, ref)

    return (
        <>
            <div className={labelClassName} {...labelProps}>
                {props.label}
            </div>
            <ul {...listBoxProps} ref={ref} className={listClassName}>
                {Array.from(state.collection).map((item) => (
                    <Option key={item.key} item={item} state={state} />
                ))}
            </ul>
        </>
    )
}

export default ListBox

import type { ListState } from 'react-stately'
import type { AriaTagProps } from 'react-aria'
import { useFocusRing, useTag } from 'react-aria'
import Button from './Button'
import { useRef } from 'react'

export type TagClassNames = {
    containerClassName?: string
    gridCellClassName?: string
    removeButtonClassName?: string
}

type TagProps<T> = AriaTagProps<T> &
    TagClassNames & {
        state: ListState<T>
        setTags: React.Dispatch<React.SetStateAction<string[]>>
    }

function Tag<T>(props: TagProps<T>) {
    const {
        item,
        state,
        containerClassName,
        gridCellClassName,
        removeButtonClassName,
        setTags,
    } = props
    const ref = useRef<HTMLDivElement>(null)
    const { focusProps, isFocusVisible } = useFocusRing({ within: true })
    const { rowProps, gridCellProps, removeButtonProps } = useTag<T>(
        props,
        state,
        ref
    )

    return (
        <div
            ref={ref}
            {...rowProps}
            {...focusProps}
            data-focus-visible={isFocusVisible}
            className={containerClassName}
        >
            <div {...gridCellProps} className={gridCellClassName}>
                {item.rendered}
                <Button
                    className={removeButtonClassName}
                    {...removeButtonProps}
                    onClick={() =>
                        setTags((prevTags) =>
                            prevTags.filter((t) => t !== item.rendered)
                        )
                    }
                >
                    ⨯
                </Button>
            </div>
        </div>
    )
}

export default Tag

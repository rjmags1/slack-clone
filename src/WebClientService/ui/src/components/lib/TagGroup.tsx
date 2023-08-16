import type { AriaTagGroupProps } from 'react-aria'
import { useListState } from 'react-stately'
import { useTagGroup } from 'react-aria'
import { useRef } from 'react'
import Tag, { TagClassNames } from './Tag'

type TagGroupProps<T> = AriaTagGroupProps<T> & {
    containerClassName?: string
    labelClassName?: string
    gridClassName?: string
    descriptionClassName?: string
    errorMessageClassName?: string
    setTags: React.Dispatch<React.SetStateAction<string[]>>
    tagClassNames?: TagClassNames
}

function TagGroup<T extends object>(props: TagGroupProps<T>) {
    const {
        label,
        description,
        errorMessage,
        containerClassName,
        labelClassName,
        gridClassName,
        descriptionClassName,
        errorMessageClassName,
        tagClassNames,
        setTags,
    } = props
    const ref = useRef<HTMLDivElement>(null)

    const state = useListState(props)
    const { gridProps, labelProps, descriptionProps, errorMessageProps } =
        useTagGroup<T>(props, state, ref)

    return (
        <div className={containerClassName}>
            <div {...labelProps} className={labelClassName}>
                {label}
            </div>
            <div {...gridProps} className={gridClassName} ref={ref}>
                {Array.from(state.collection).map((item) => (
                    <Tag
                        setTags={setTags}
                        key={item.key}
                        item={item}
                        state={state}
                        {...tagClassNames}
                    />
                ))}
            </div>
            {description && (
                <div {...descriptionProps} className={descriptionClassName}>
                    {description}
                </div>
            )}
            {errorMessage && (
                <div {...errorMessageProps} className={errorMessageClassName}>
                    {errorMessage}
                </div>
            )}
        </div>
    )
}

export default TagGroup

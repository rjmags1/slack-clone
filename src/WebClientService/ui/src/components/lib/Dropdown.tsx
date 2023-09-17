import { Item } from 'react-stately'
import List from './List'
import { ReactNode, useEffect, useRef, useState } from 'react'
import { useHover } from 'react-aria'
import useDetectOutsideClick from '../../hooks/useDetectOutsideClick'
import { generateRandomString } from '../../utils'

type DropdownProps = {
    className?: string
    selectedClassName?: string
    items: ReactNode[]
    noItemsListing?: ReactNode
    close: () => void
}

function Dropdown({
    items,
    className,
    noItemsListing,
    selectedClassName,
    close,
}: DropdownProps) {
    const ref = useRef<HTMLDivElement>(null)
    const [dropdownItemIdPrefix] = useState(generateRandomString(5))
    const [selected, setSelected] = useState(0)
    const { hoverProps } = useHover({
        onHoverStart: (e) => {
            setSelected(getItemIdxFromItemContainerId(e.target.id))
        },
    })
    useDetectOutsideClick(ref, close)

    useEffect(() => {
        const handleKeyup = (e: KeyboardEvent) => {
            if (e.key === 'ArrowDown') {
                setSelected((prev) => (prev + 1) % items.length)
            }
            if (e.key === 'ArrowUp') {
                setSelected((prev) =>
                    prev === 0 ? items.length - 1 : prev - 1
                )
            }
        }
        document.addEventListener('keyup', handleKeyup)
        return () => document.removeEventListener('keyup', handleKeyup)
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [items])

    const getItemIdxFromItemContainerId = (id: string): number =>
        parseInt(id.slice(dropdownItemIdPrefix.length))

    return (
        <div
            ref={ref}
            className={className || 'absolute z-10 mt-1 w-full rounded-md'}
        >
            <List>
                {items.length === 0 ? (
                    <Item>{noItemsListing}</Item>
                ) : (
                    items.map((item, i) => (
                        <Item>
                            <div
                                {...hoverProps}
                                id={`${dropdownItemIdPrefix}${i}`}
                                className={
                                    (i === selected ? selectedClassName : '') +
                                    `${
                                        i === 0
                                            ? ' rounded-t-md'
                                            : i === items.length - 1
                                            ? ' rounded-b-md'
                                            : ''
                                    }`
                                }
                            >
                                {item}
                            </div>
                        </Item>
                    ))
                )}
            </List>
        </div>
    )
}

export default Dropdown

import { AriaButtonProps, useButton } from 'react-aria'
import { ReactNode, useRef } from 'react'

type ButtonProps = AriaButtonProps<'button'> & {
    children: ReactNode
    className?: string
    onClick?: React.MouseEventHandler<HTMLButtonElement>
}

function Button(props: ButtonProps) {
    const ref: any = useRef()
    const { buttonProps } = useButton({}, ref)
    const { children, className, onClick } = props

    return (
        <button
            {...buttonProps}
            className={className}
            onClick={onClick}
            ref={ref}
        >
            {children}
        </button>
    )
}

export default Button

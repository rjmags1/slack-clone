import type { AriaDialogProps } from 'react-aria'
import { useDialog } from 'react-aria'
import { useRef } from 'react'

type DialogProps = AriaDialogProps & {
    title?: React.ReactNode
    children: React.ReactNode
    className?: string
    close?: () => void
}

function Dialog({ title, children, className, close, ...props }: DialogProps) {
    const ref = useRef<HTMLDivElement>(null)
    const { dialogProps, titleProps } = useDialog(props, ref)

    return (
        <div {...dialogProps} ref={ref} className={className}>
            {close && (
                <div className="flex h-fit w-full items-center justify-between font-thin">
                    {title && (
                        <h3
                            {...titleProps}
                            className="left-0 mt-0 font-semibold"
                        >
                            {title}
                        </h3>
                    )}
                    <button
                        className="mr-2 h-fit text-sm hover:opacity-70"
                        onClick={close}
                    >
                        ✕
                    </button>
                </div>
            )}
            {children}
        </div>
    )
}

export default Dialog

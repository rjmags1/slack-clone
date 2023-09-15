import { useRef } from 'react'
import { DismissButton, Overlay, usePopover } from 'react-aria'
import type { AriaPopoverProps } from 'react-aria'
import type { OverlayTriggerState } from 'react-stately'

interface PopoverProps extends Omit<AriaPopoverProps, 'popoverRef'> {
    children: React.ReactNode
    state: OverlayTriggerState
}

function Popover({ children, state, offset = 8, ...props }: PopoverProps) {
    let popoverRef = useRef(null)
    let { popoverProps, underlayProps, arrowProps, placement } = usePopover(
        {
            ...props,
            offset,
            popoverRef,
        },
        state
    )

    return (
        <Overlay>
            <div {...underlayProps} style={{ position: 'fixed', inset: 0 }} />
            <div
                {...popoverProps}
                style={{
                    ...popoverProps.style,
                }}
                ref={popoverRef}
                className="rounded-md bg-zinc-700 p-2 text-white shadow-2xl"
            >
                <DismissButton onDismiss={state.close} />
                {children}
                <DismissButton onDismiss={state.close} />
            </div>
        </Overlay>
    )
}

export default Popover

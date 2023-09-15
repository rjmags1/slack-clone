import { useOverlayTrigger } from 'react-aria'
import { OverlayTriggerProps, useOverlayTriggerState } from 'react-stately'
import Button from './Button'
import React, { JSXElementConstructor, ReactElement } from 'react'
import Modal from './Modal'

type ModalTriggerProps = OverlayTriggerProps & {
    label?: string
    className?: string
    children: (
        props: object
    ) => ReactElement<{ id: string }, string | JSXElementConstructor<any>>
}

function ModalTriggerButton({
    label,
    children,
    className,
    ...props
}: ModalTriggerProps) {
    const state = useOverlayTriggerState(props)
    const { triggerProps, overlayProps } = useOverlayTrigger(
        { type: 'dialog' },
        state
    )

    return (
        <>
            <Button
                {...triggerProps}
                className={className}
                onClick={() => state.setOpen(true)}
            >
                {label}
            </Button>
            {state.isOpen && (
                <Modal {...props} state={state}>
                    {React.cloneElement(children(state.close), overlayProps)}
                </Modal>
            )}
        </>
    )
}

export default ModalTriggerButton

import { ReactNode, useRef } from 'react'
import { AriaModalOverlayProps, Overlay, useModalOverlay } from 'react-aria'
import { OverlayTriggerState } from 'react-stately'

type ModalProps = {
    state: OverlayTriggerState
    children: ReactNode
    className?: string
}

function Modal({ state, children, className, ...props }: ModalProps) {
    let ref = useRef<HTMLDivElement>(null)
    let { modalProps, underlayProps } = useModalOverlay(
        props as AriaModalOverlayProps,
        state,
        ref
    )

    return (
        <Overlay>
            <div
                style={{
                    position: 'fixed',
                    zIndex: 100,
                    top: 0,
                    left: 0,
                    bottom: 0,
                    right: 0,
                    display: 'flex',
                    alignItems: 'center',
                    justifyContent: 'center',
                }}
                {...underlayProps}
            >
                <div {...modalProps} ref={ref} className={className}>
                    {children}
                </div>
            </div>
        </Overlay>
    )
}

export default Modal

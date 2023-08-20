import { useRef, useEffect } from 'react'

type ScrollTriggerProps = {
    containerRef: React.RefObject<HTMLElement>
    observerCallback: IntersectionObserverCallback
    threshold?: number
    containerMargin?: string
}

function ScrollTrigger({
    containerRef,
    observerCallback,
    threshold,
    containerMargin,
}: ScrollTriggerProps) {
    const observerRef = useRef<IntersectionObserver | null>(null)
    const observedRef = useRef<HTMLDivElement | null>(null)

    useEffect(() => {
        if (observerRef.current === null) {
            observerRef.current = new IntersectionObserver(observerCallback, {
                root: containerRef.current,
                rootMargin: containerMargin || '0px',
                threshold: typeof threshold === 'undefined' ? 1.0 : threshold,
            })
        }
        if (observerRef.current !== null && observedRef.current !== null) {
            observerRef.current.observe(observedRef.current)
        }
        //eslint-disable-next-line react-hooks/exhaustive-deps
    }, [observedRef.current, containerRef.current])

    return <div ref={observedRef} className="h-3 w-full bg-inherit" />
}

export default ScrollTrigger

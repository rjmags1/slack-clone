import { useEffect, useRef } from 'react'

function useIntersectionObserver(
    observedId: string,
    reobserveDependencies: any[],
    skipReobserve: boolean[],
    onIntersect: () => void
) {
    const observerRef = useRef<IntersectionObserver | null>(null)
    useEffect(() => {
        if (skipReobserve.some((condition) => condition)) return

        if (observerRef.current) observerRef.current.disconnect()

        observerRef.current = new IntersectionObserver((entries) => {
            for (const entry of entries) {
                if (
                    entry.target.id === observedId &&
                    entry.intersectionRatio > 0
                ) {
                    onIntersect()
                }
            }
        })
        observerRef.current.observe(document.getElementById(observedId)!)

        return () => observerRef.current!.disconnect()
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, reobserveDependencies)
}

export default useIntersectionObserver

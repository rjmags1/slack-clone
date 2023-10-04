import { useEffect } from 'react'

function useScrollIntoView(
    targetId: string,
    options: ScrollIntoViewOptions,
    rescrollDependencies: any[]
) {
    useEffect(() => {
        const firstMessageId = targetId
        document.getElementById(firstMessageId)!.scrollIntoView(options)
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, rescrollDependencies)
}

export default useScrollIntoView

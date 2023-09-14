import { RefObject, useEffect } from 'react'
import { debounce } from '../utils'

function useDetectOutsideClick(
    ref: RefObject<HTMLElement>,
    onDetect: () => void
) {
    useEffect(() => {
        function handleClickOutside(e: Event) {
            if (ref.current && !ref.current.contains(e.target as Node)) {
                onDetect()
            }
        }
        debounce(
            () => document.addEventListener('click', handleClickOutside),
            300
        )

        return () => {
            document.removeEventListener('click', handleClickOutside)
        }
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [])
}

export default useDetectOutsideClick

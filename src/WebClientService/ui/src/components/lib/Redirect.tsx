import { useEffect } from 'react'
import LoadingSpinner from './LoadingSpinner'

function Redirect({ to }: { to: string }) {
    useEffect(() => {
        window.location.href = to
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [])

    return (
        <LoadingSpinner className="flex h-screen w-screen items-center justify-center" />
    )
}

export default Redirect

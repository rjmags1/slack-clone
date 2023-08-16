import { useEffect } from 'react'

function Redirect({ to }: { to: string }) {
    useEffect(() => {
        window.location.href = to
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [])

    return <span>redirecting...</span>
}

export default Redirect

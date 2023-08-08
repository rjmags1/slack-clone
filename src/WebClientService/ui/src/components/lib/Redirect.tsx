import { useEffect } from 'react'

function Redirect({ to }: { to: string }) {
    useEffect(() => {
        window.location.href = to
    }, [])

    return <span>redirecting...</span>
}

export default Redirect

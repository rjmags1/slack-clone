import { useState, useEffect } from 'react'

const SESSION_URL = '/bff/user'

const THREE_MIN_MS = 180 * 1000

export type Claim = {
    type: string
    value: string | number
}

function useClaims(): {
    loading: boolean
    claims: Claim[] | null
} {
    const [claims, setClaims] = useState<Claim[] | null>(null)
    const [loading, setLoading] = useState<boolean>(true)
    const [lastCheckedAt, setLastCheckedAt] = useState(Date.now())

    useEffect(() => {
        const getSession = async () => {
            var req = new Request(SESSION_URL, {
                headers: new Headers({
                    'X-CSRF': '1',
                }),
            })
            try {
                const res = await fetch(req)
                if (res.ok) {
                    const data = await res.json()
                    if (claims === null) {
                        setClaims(data)
                    }
                    setLoading(false)
                } else {
                    setClaims(null)
                    setLoading(false)
                }
            } catch (error) {
                setLoading(false)
                console.log(error)
            }
        }

        const checkSession = async () => {
            if (Date.now() - lastCheckedAt < THREE_MIN_MS) return

            await getSession()
            setLastCheckedAt(Date.now())
        }

        setInterval(checkSession, THREE_MIN_MS)
        getSession()
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [])

    return { claims, loading }
}

export default useClaims

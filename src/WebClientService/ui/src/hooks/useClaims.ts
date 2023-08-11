import { useState, useEffect } from 'react'

const SESSION_URL = '/bff/user'

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

    useEffect(() => {
        const fetchData = async () => {
            var req = new Request(SESSION_URL, {
                headers: new Headers({
                    'X-CSRF': '1',
                }),
            })
            try {
                const res = await fetch(req)
                if (res.ok) {
                    const data = await res.json()
                    setClaims(data)
                    setLoading(false)
                } else {
                    setLoading(false)
                }
            } catch (error) {
                setLoading(false)
                console.log(error)
            }
        }

        fetchData()
    }, [])

    return { claims, loading }
}

export default useClaims

import { useState, useEffect } from 'react'

const SESSION_URL = '/bff/user'

function useClaims() {
    const [claims, setClaims] = useState(null)
    const [loading, setLoading] = useState(true)

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

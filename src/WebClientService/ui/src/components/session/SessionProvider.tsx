import useClaims, { Claim } from '../../hooks/useClaims'
import Loading from '../lib/Loading'
import { createContext } from 'react'

export const SessionContext = createContext<Claim[] | null>(null)

function SessionProvider({ children }: { children: React.ReactNode }) {
    const { claims, loading } = useClaims()

    return (
        <SessionContext.Provider value={claims}>
            {loading ? <Loading /> : children}
        </SessionContext.Provider>
    )
}

export default SessionProvider

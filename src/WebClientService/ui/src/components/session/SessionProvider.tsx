import useClaims, { Claim } from '../../hooks/useClaims'
import Loading from '../lib/Loading'
import { createContext } from 'react'

export const SessionContext = createContext<Claim[] | null>(null)

export const getSubClaim = (claims: Claim[]) =>
    claims.filter((c) => c.type === 'sub')[0]?.value as string

function SessionProvider({ children }: { children: React.ReactNode }) {
    const { claims, loading } = useClaims()

    return (
        <SessionContext.Provider value={claims}>
            {loading ? <Loading /> : children}
        </SessionContext.Provider>
    )
}

export default SessionProvider

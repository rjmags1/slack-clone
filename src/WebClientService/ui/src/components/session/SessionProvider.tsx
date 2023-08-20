import useClaims, { Claim } from '../../hooks/useClaims'
import LoadingSpinner from '../lib/LoadingSpinner'
import { createContext } from 'react'

export const SessionContext = createContext<Claim[] | null>(null)

export const getSubClaim = (claims: Claim[]) =>
    claims.filter((c) => c.type === 'sub')[0]?.value as string

function SessionProvider({ children }: { children: React.ReactNode }) {
    const { claims, loading } = useClaims()

    return (
        <SessionContext.Provider value={claims}>
            {loading ? (
                <LoadingSpinner className="flex h-screen w-screen items-center justify-center" />
            ) : (
                children
            )}
        </SessionContext.Provider>
    )
}

export default SessionProvider

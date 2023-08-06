import useClaims from '../../hooks/useClaims'
import Loading from '../utils/Loading'
import { createContext } from 'react'

export const SessionContext = createContext(null)

function SessionProvider({ children }: { children: React.ReactNode }) {
    const { claims, loading } = useClaims()

    return loading ? (
        <Loading />
    ) : (
        <SessionContext.Provider value={claims}>
            {children}
        </SessionContext.Provider>
    )
}

export default SessionProvider

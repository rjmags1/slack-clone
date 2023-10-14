import { createContext, useEffect, useRef } from 'react'
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr'
import useClaims from '../../hooks/useClaims'
import { getSubClaim } from './SessionProvider'

const signalRURL = 'https://localhost:6002/realtime-hub'

export const SignalRContext = createContext<HubConnection | null>(null)

function SignalRProvider({ children }: { children: React.ReactNode }) {
    const connectionRef = useRef<HubConnection | null>(null)
    const { claims } = useClaims()

    useEffect(() => {
        if (claims === null) return
        const sub = getSubClaim(claims)
        const connect = async () => {
            const builder = new HubConnectionBuilder().withUrl(
                signalRURL + `?sub=${sub}`
            )
            const connection = builder.build()
            connectionRef.current = connection
            try {
                await connection.start()
            } catch (e) {
                console.error(e)
            }
        }
        const disconnect = async () => await connectionRef.current?.stop()

        connect()

        return () => {
            if (connectionRef.current !== null) disconnect()
        }
    }, [claims])

    return (
        <SignalRContext.Provider value={connectionRef.current}>
            {children}
        </SignalRContext.Provider>
    )
}

export default SignalRProvider

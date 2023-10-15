import { createContext, useContext, useEffect, useRef } from 'react'
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr'
import { SessionContext, getSubClaim } from './SessionProvider'

const SIGNAL_R_URL = 'https://localhost:6002/realtime-hub'

export const SignalRContext = createContext<HubConnection | null>(null)

function SignalRProvider({ children }: { children: React.ReactNode }) {
    const connectionRef = useRef<HubConnection | null>(null)
    const claims = useContext(SessionContext)!

    useEffect(() => {
        const sub = getSubClaim(claims)
        const connect = async () => {
            const builder = new HubConnectionBuilder().withUrl(
                SIGNAL_R_URL + `?sub=${sub}`
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
    })

    return (
        <SignalRContext.Provider value={connectionRef.current}>
            {children}
        </SignalRContext.Provider>
    )
}

export default SignalRProvider

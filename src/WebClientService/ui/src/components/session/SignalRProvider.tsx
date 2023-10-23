import { createContext, useContext, useEffect, useRef } from 'react'
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr'
import { SessionContext, getSubClaim } from './SessionProvider'
import { useParams } from 'react-router-dom'

const SIGNAL_R_URL = 'https://localhost:6002/realtime-hub'

export const SignalRContext = createContext<HubConnection | null>(null)

function SignalRProvider({ children }: { children: React.ReactNode }) {
    const connectionRef = useRef<HubConnection | null>(null)
    const claims = useContext(SessionContext)!
    const { workspaceId } = useParams()

    useEffect(() => {
        if (!workspaceId) return

        const sub = getSubClaim(claims)
        const connect = async () => {
            const builder = new HubConnectionBuilder().withUrl(
                SIGNAL_R_URL + `?sub=${sub}&workspace=${workspaceId}`
            )
            const connection = builder.build()

            connection.on('receiveMessage', (key: string, message: string) =>
                console.log(key, message)
            )

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
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [workspaceId])

    return (
        <SignalRContext.Provider value={connectionRef.current}>
            {children}
        </SignalRContext.Provider>
    )
}

export default SignalRProvider

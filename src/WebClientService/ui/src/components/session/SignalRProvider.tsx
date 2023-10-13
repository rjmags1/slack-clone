import { createContext, useEffect, useState } from 'react'
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr'

const signalRURL = 'https://localhost:6002/realtime-hub'

export const SignalRContext = createContext<HubConnection | null>(null)

function SignalRProvider({ children }: { children: React.ReactNode }) {
    const [signalRConnection, setSignalRConnection] =
        useState<HubConnection | null>(null)

    useEffect(() => {
        if (signalRConnection !== null) return

        const connect = async () => {
            const builder = new HubConnectionBuilder().withUrl(signalRURL)
            const connection = builder.build()
            try {
                await connection.start()
                setSignalRConnection(connection)
            } catch (e) {
                console.error(e)
            }
        }

        connect()

        return () => {
            if (signalRConnection !== null) {
                ;(signalRConnection as HubConnection).stop()
            }
        }
    }, [signalRConnection])

    return (
        <SignalRContext.Provider value={signalRConnection}>
            {children}
        </SignalRContext.Provider>
    )
}

export default SignalRProvider

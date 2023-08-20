import { useEffect, useState } from 'react'
import { fetchQuery, useRelayEnvironment } from 'react-relay'
import ValidUserEmailQuery from '../relay/queries/ValidUserEmail'

type ValidateEmailArgs = {
    email: string
    exclude: string[]
    onValidated: () => void
    alertTime?: number
}

function useValidateEmail({
    email,
    exclude,
    onValidated,
    alertTime,
}: ValidateEmailArgs) {
    const relayEnv = useRelayEnvironment()
    const [valid, setValid] = useState<boolean>(true)

    useEffect(() => {
        if (email === '') return
        if (exclude.includes(email)) {
            setValid(false)
            return
        }

        fetchQuery(relayEnv, ValidUserEmailQuery, {
            email,
        }).subscribe({
            next: (data: any) => {
                if (data.validUserEmail.valid) {
                    onValidated()
                    setValid(true)
                } else {
                    setValid(false)
                }
            },
            error: (error: any) => console.log(error),
        })
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [email])

    useEffect(() => {
        if (!alertTime || valid) return

        setTimeout(() => setValid(true), alertTime)
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [valid])

    return { valid }
}

export default useValidateEmail

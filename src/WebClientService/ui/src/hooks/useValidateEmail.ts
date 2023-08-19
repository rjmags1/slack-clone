import { useEffect } from 'react'
import { fetchQuery, useRelayEnvironment } from 'react-relay'
import ValidUserEmailQuery from '../relay/queries/ValidUserEmail'

type ValidateEmailArgs = {
    email: string
    exclude: string[]
    onValidated: () => void
    onInvalidated: () => void
}

function useValidateEmail({
    email,
    exclude,
    onValidated,
    onInvalidated,
}: ValidateEmailArgs) {
    const relayEnv = useRelayEnvironment()
    useEffect(() => {
        if (email === '') return
        if (exclude.includes(email)) {
            onInvalidated()
            return
        }

        fetchQuery(relayEnv, ValidUserEmailQuery, {
            email,
        }).subscribe({
            next: (data: any) => {
                if (data.validUserEmail.valid) {
                    onValidated()
                } else {
                    onInvalidated()
                }
            },
            error: (error: any) => console.log(error),
        })
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [email])
}

export default useValidateEmail

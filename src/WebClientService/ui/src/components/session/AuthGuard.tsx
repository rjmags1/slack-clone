import { ReactNode, useContext } from 'react'
import { SessionContext } from './SessionProvider'
import Redirect from '../lib/Redirect'

const LOGIN_URL = '/bff/login?returnUrl=/'

function AuthGuard({ children }: { children: ReactNode }) {
    const claims = useContext(SessionContext)
    return claims === null ? <Redirect to={LOGIN_URL} /> : <>{children}</>
}

export default AuthGuard

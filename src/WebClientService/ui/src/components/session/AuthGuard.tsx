import { ReactElement, useContext } from 'react'
import { SessionContext } from './SessionProvider'
import Redirect from '../utils/Redirect'

const LOGIN_URL = '/bff/login?returnUrl=/'

function AuthGuard({ page }: { page: ReactElement }) {
    const claims = useContext(SessionContext)
    return claims === null ? <Redirect to={LOGIN_URL} /> : page
}

export default AuthGuard

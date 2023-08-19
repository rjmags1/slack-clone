import Button from '../../lib/Button'
import { SessionContext } from '../../session/SessionProvider'
import { useContext } from 'react'

function LogoutBtn() {
    const claims = useContext(SessionContext)

    const logout = () => {
        const logoutUrl = claims!.find(
            (claim: any) => claim.type === 'bff:logout_url'
        )!.value as string
        window.location.href = logoutUrl
    }

    return (
        <Button
            className="rounded-md bg-sky-700 px-2 py-1
                text-xs outline-none hover:bg-sky-900"
            onClick={logout}
        >
            Logout
        </Button>
    )
}

export default LogoutBtn

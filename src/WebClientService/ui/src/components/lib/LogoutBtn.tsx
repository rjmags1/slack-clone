import Button from './Button'
import { SessionContext } from '../session/SessionProvider'
import { useContext } from 'react'

type LogoutBtnProps = {
    className?: string
    label?: string
}

function LogoutBtn({ className, label }: LogoutBtnProps) {
    const claims = useContext(SessionContext)

    const logout = () => {
        const logoutUrl = claims!.find(
            (claim: any) => claim.type === 'bff:logout_url'
        )!.value as string
        window.location.href = logoutUrl
    }

    return (
        <Button
            className={
                className ||
                `rounded-md bg-sky-700 px-2 py-1 text-xs outline-none 
                    hover:bg-sky-900`
            }
            onClick={logout}
        >
            {label || 'Logout'}
        </Button>
    )
}

export default LogoutBtn

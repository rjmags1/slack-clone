import LogoutBtn from '../../lib/LogoutBtn'

function SignoutOption() {
    return (
        <LogoutBtn
            label="Sign out"
            className="w-full min-w-max px-2 py-1 text-xs font-light 
                text-white hover:underline"
        />
    )
}

export default SignoutOption

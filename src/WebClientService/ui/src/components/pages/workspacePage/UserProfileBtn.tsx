import Avatar from '../../lib/Avatar'
import Button from '../../lib/Button'

type UserProfileBtnProps = {
    className?: string
}

function UserProfileBtn({ className }: UserProfileBtnProps) {
    const onClick = () => {
        // TODO
    }

    return (
        <Button onClick={onClick} className={className}>
            <Avatar
                src="/default-avatar.png"
                alt="profile"
                className="h-7 border border-solid border-zinc-300 
                    bg-zinc-300"
            />
        </Button>
    )
}

export default UserProfileBtn

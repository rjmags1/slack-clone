import Avatar, { DEFAULT_AVATAR_NAME } from '../../../../lib/Avatar'
import DatetimeStamp from '../../../../lib/DatetimeStamp'

type DirectMessageHeaderProps = {
    user: {
        id: string
        username: string
        avatar: {
            id: string
            storeKey: string
        }
    } | null
    sentAtUTC: string
    replyToId: string | null
}

function DirectMessageHeader({
    user,
    sentAtUTC,
    replyToId,
}: DirectMessageHeaderProps) {
    return (
        <div
            className="flex w-full items-center justify-start gap-x-2 
                text-sm"
        >
            <Avatar
                src={
                    user?.avatar.storeKey === DEFAULT_AVATAR_NAME
                        ? '/default-avatar.png'
                        : user!.avatar.storeKey
                }
                alt="author-avatar-img"
                className="h-[1.7rem]"
            />
            <div className="flex w-max flex-col justify-start gap-x-1 truncate">
                <h6>{user?.username || 'deleted'}</h6>
                <DatetimeStamp
                    className="text-[.7rem] font-thin italic"
                    serverUTCString={sentAtUTC}
                />
            </div>
        </div>
    )
}

export default DirectMessageHeader

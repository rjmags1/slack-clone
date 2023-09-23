import { UserProfileBtnFragment$data } from '../../../../relay/fragments/__generated__/UserProfileBtnFragment.graphql'
import Avatar, { DEFAULT_AVATAR_NAME } from '../../../lib/Avatar'

type UserBannerProps = {
    user: UserProfileBtnFragment$data
}

function UserBanner({ user }: UserBannerProps) {
    const { avatar } = user

    return (
        <div
            className="flex items-center gap-x-3 rounded-t-md 
                bg-sky-800 p-2"
        >
            <Avatar
                src={
                    avatar.name === DEFAULT_AVATAR_NAME
                        ? '/default-avatar.png'
                        : ''
                }
                alt="user-profile-img"
                className="h-[2.2rem] border border-solid border-zinc-300 
                    bg-zinc-300"
            />
            <div className="h-full text-left">
                <h4 className="text-sm">{user.username}</h4>
                <h5 className="text-xs font-light">{user.onlineStatus}</h5>
            </div>
        </div>
    )
}

export default UserBanner

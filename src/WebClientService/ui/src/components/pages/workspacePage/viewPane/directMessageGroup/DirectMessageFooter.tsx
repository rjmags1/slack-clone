import DatetimeStamp from '../../../../lib/DatetimeStamp'
import MessageInteractionBar from '../MessageInteractionBar'

type DirectMessageFooterProps = {
    lastEditUTC: string | null
    laterFlag: { id: string } | null
    reactions: Readonly<
        {
            emoji: string
            id: string
            user: {
                readonly id: string
            }
        }[]
    > | null
}

function DirectMessageFooter({
    lastEditUTC,
    laterFlag,
    reactions,
}: DirectMessageFooterProps) {
    return (
        <div className="flex w-full flex-col gap-y-1 overflow-hidden">
            {lastEditUTC && (
                <DatetimeStamp
                    serverUTCString="lastEditUtc"
                    className="text-[.7rem] font-thin italic"
                    label="Edited at "
                />
            )}
            <div className="flex w-max gap-x-2">
                <MessageInteractionBar
                    laterFlag={laterFlag}
                    reactions={reactions}
                />
            </div>
        </div>
    )
}

export default DirectMessageFooter

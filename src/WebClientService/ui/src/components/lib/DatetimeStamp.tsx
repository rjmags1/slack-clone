type DatetimeStampProps = {
    serverUTCString: string
    className: string
    label?: string
}

export const serverUTCStringToDate = (s: string) => new Date(s + 'Z')

function DatetimeStamp({
    serverUTCString,
    className,
    label,
}: DatetimeStampProps) {
    const date = serverUTCStringToDate(serverUTCString)

    return (
        <time className={className}>{`${label || ''}${date.toLocaleString(
            'en-us'
        )}`}</time>
    )
}

export default DatetimeStamp

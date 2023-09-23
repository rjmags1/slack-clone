type AvatarProps = {
    src: string
    alt: string
    className?: string
}

export const DEFAULT_AVATAR_NAME = 'DEFAULT_AVATAR'

function Avatar({ src, alt, className }: AvatarProps) {
    return (
        <img
            src={src}
            alt={alt}
            style={{
                borderRadius: '9999px',
            }}
            className={className}
        />
    )
}

export default Avatar

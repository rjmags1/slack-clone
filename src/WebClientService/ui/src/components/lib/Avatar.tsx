type AvatarProps = {
    src: string
    alt: string
    className?: string
}

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

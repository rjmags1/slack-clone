type ExpandIconTriangleProps = {
    expand: boolean
}

function ExpandIconTriangle({ expand }: ExpandIconTriangleProps) {
    return (
        <span
            style={{
                transform: expand ? 'rotate(180deg)' : '',
                fontSize: '75%',
            }}
        >
            ▼
        </span>
    )
}

export default ExpandIconTriangle

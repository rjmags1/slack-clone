import Button from './Button'

type SendButtonProps = {
    className: string
}

function SendButton({ className }: SendButtonProps) {
    return (
        <Button className={className} onClick={() => alert('not implemented')}>
            Send
        </Button>
    )
}

export default SendButton

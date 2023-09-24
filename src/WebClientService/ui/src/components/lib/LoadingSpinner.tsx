type LoadingSpinnerProps = {
    className?: string
}

function LoadingSpinner({ className }: LoadingSpinnerProps) {
    return (
        <div
            className={
                className || 'flex h-full w-full items-center justify-center'
            }
        >
            <div className="lds-ring">
                <div></div>
                <div></div>
                <div></div>
                <div></div>
            </div>
        </div>
    )
}

export default LoadingSpinner

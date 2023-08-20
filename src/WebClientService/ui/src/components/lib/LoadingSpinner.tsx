type LoadingSpinnerProps = {
    className?: string
}

function LoadingSpinner({ className }: LoadingSpinnerProps) {
    return (
        <div className={className}>
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

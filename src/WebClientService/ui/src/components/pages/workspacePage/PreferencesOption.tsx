import Button from '../../lib/Button'

function PreferencesOption() {
    const onClick = () => {
        alert('not implemented')
    }

    return (
        <Button
            onClick={onClick}
            className="w-full min-w-max px-2 py-1 text-xs 
            font-light text-white hover:underline"
        >
            Preferences
        </Button>
    )
}

export default PreferencesOption

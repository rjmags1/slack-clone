import Button from '../../../lib/Button'

function WorkspaceSettingsBtn() {
    return (
        <Button
            className="h-max w-max shrink truncate 
                rounded-md bg-sky-900 p-1 px-2 text-xs"
            onClick={() => alert('not implemented')}
        >
            Settings
        </Button>
    )
}

export default WorkspaceSettingsBtn

import { useRef, useState } from 'react'
import InputTextField from '../../lib/InputTextField'
import SearchField from '../../lib/SearchField'
import TagGroup from '../../lib/TagGroup'
import { Item } from 'react-stately'
import Button from '../../lib/Button'

function CreateWorkspaceForm() {
    const fileInputRef = useRef<HTMLInputElement>(null)
    const [name, setName] = useState<string>('')
    const [description, setDescription] = useState<string>('')
    const [avatar, setAvatar] = useState<File | null>(null)
    const [memberEmails, setMemberEmails] = useState<string[]>([])
    const [searchEmailText, setEmailSearchText] = useState<string>('')

    const handleFileUpload = () => {
        const uploadedFiles = fileInputRef.current?.files
        if (uploadedFiles && uploadedFiles.length > 0) {
            setAvatar(uploadedFiles[uploadedFiles.length - 1])
        }
    }

    const handleMemberEmailAdd = (newMemberEmail: string) => {
        // TODO: validate member email
        setEmailSearchText('')
        setMemberEmails([newMemberEmail, ...memberEmails])
    }

    const submit = () => {
        // TODO
        // call graphql createWorkspace mutation
    }

    return (
        <form className="flex h-full w-full flex-col gap-y-3 text-sm">
            <InputTextField label="Name" onChange={setName} />
            <InputTextField label="Description" onChange={setDescription} />
            <div className="mb-1 flex flex-col gap-y-1 font-extralight">
                <label htmlFor="workspace-avatar">Avatar</label>
                <input
                    ref={fileInputRef}
                    type="file"
                    id="workspace-avatar"
                    name="workspace-avatar"
                    accept="image/png, image/jpeg, image/jpg"
                    onChange={handleFileUpload}
                    className="text-xs font-extralight"
                />
            </div>
            <SearchField
                onClear={() => setEmailSearchText('')}
                onChange={setEmailSearchText}
                onSubmit={handleMemberEmailAdd}
                label="Add members by email"
                value={searchEmailText}
            />
            <Button
                className="w-full rounded-md border border-white bg-sky-700 
                    py-1 hover:opacity-50"
                onClick={submit}
            >
                Create Workspace
            </Button>
            {memberEmails.length > 0 && (
                <TagGroup
                    label="Members"
                    setTags={setMemberEmails}
                    labelClassName="text-sm font-extralight mb-[4px]"
                    gridClassName="flex gap-[2px] w-full h-fit flex-wrap"
                    tagClassNames={{
                        containerClassName:
                            'w-fit h-fit text-xs font-thin bg-zinc-500 border rounded-lg border-zinc-500',
                        gridCellClassName:
                            'flex px-[4px] gap-x-[6px] items-center',
                        removeButtonClassName: 'text-sm font-thin mb-[2px]',
                    }}
                >
                    {memberEmails.map((email) => (
                        <Item>{email}</Item>
                    ))}
                </TagGroup>
            )}
        </form>
    )
}

export default CreateWorkspaceForm

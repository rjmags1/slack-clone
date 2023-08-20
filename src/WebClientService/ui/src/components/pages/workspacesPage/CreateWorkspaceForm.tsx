import { useContext, useEffect, useState } from 'react'
import InputTextField from '../../lib/InputTextField'
import SearchField from '../../lib/SearchField'
import TagGroup from '../../lib/TagGroup'
import { Item } from 'react-stately'
import Button from '../../lib/Button'
import { SessionContext, getSubClaim } from '../../session/SessionProvider'
import useCreateAvatar from '../../../hooks/useCreateAvatar'
import useValidateEmail from '../../../hooks/useValidateEmail'
import useCreateWorkspace from '../../../hooks/useCreateWorkspace'
import FileInput from '../../lib/FileInput'

type CreateWorkspaceFormProps = {
    close: () => void
}

function CreateWorkspaceForm({ close }: CreateWorkspaceFormProps) {
    const claims = useContext(SessionContext)!

    const [name, setName] = useState<string>('')
    const [description, setDescription] = useState<string>('')
    const [avatar, setAvatar] = useState<File | null>(null)
    const [avatarId, setAvatarId] = useState<string | null>(null)
    const [memberEmails, setMemberEmails] = useState<string[]>([])
    const [searchEmailText, setEmailSearchText] = useState<string>('')
    const [submittedSearchEmailText, setSubmittedSearchEmailText] =
        useState<string>('')

    const {
        createAvatar,
        data: uploadedAvatar,
        errors: avatarUploadErrors,
    } = useCreateAvatar(avatar, getSubClaim(claims))
    const { createWorkspace } = useCreateWorkspace()
    const { valid: validEmail } = useValidateEmail({
        alertTime: 3 * 1000,
        email: submittedSearchEmailText,
        exclude: memberEmails,
        onValidated: () => {
            setEmailSearchText('')
            setMemberEmails([submittedSearchEmailText, ...memberEmails])
        },
    })

    const createNewWorkspace = () =>
        createWorkspace({
            sub: getSubClaim(claims),
            name,
            description,
            avatarId: avatarId,
            invitedEmails: memberEmails,
            closeModal: close,
        })

    const initSubmit = () => (avatar ? createAvatar() : createNewWorkspace())

    useEffect(() => {
        if (!uploadedAvatar && !avatarUploadErrors) return
        if (uploadedAvatar && !avatarId) {
            setAvatarId(uploadedAvatar.createAvatar!.id)
            return
        }
        if (avatarUploadErrors) {
            setAvatarId(null)
            for (const e of avatarUploadErrors) {
                console.error(e)
            }
            return
        }
        if (avatarId) {
            createNewWorkspace()
        }

        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [uploadedAvatar, avatarId])

    return (
        <form className="flex h-full w-full flex-col gap-y-3 text-sm">
            <InputTextField label="Name" onChange={setName} />
            <InputTextField label="Description" onChange={setDescription} />
            <div
                className="mb-1 flex max-w-[16rem] flex-col gap-y-1 truncate 
                    font-extralight"
            >
                <FileInput
                    label="Avatar"
                    id="workspace-avatar"
                    accept="image/png, image/jpeg, image/jpg"
                    className="text-xs font-extralight"
                    setFile={setAvatar}
                />
            </div>
            <SearchField
                onClear={() => setEmailSearchText('')}
                onChange={setEmailSearchText}
                onSubmit={() => setSubmittedSearchEmailText(searchEmailText)}
                alertInvalidSearch={!validEmail}
                errorMessage="Invalid email"
                label="Add members by email"
                value={searchEmailText}
            />
            <Button
                className="w-full rounded-md border border-white bg-sky-700 
                    py-1 hover:opacity-50"
                onClick={initSubmit}
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

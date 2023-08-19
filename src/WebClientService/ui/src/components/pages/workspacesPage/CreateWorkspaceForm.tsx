import { useContext, useEffect, useRef, useState } from 'react'
import InputTextField from '../../lib/InputTextField'
import SearchField from '../../lib/SearchField'
import TagGroup from '../../lib/TagGroup'
import { Item } from 'react-stately'
import Button from '../../lib/Button'
import { fetchQuery, useMutation, useRelayEnvironment } from 'react-relay'
import { SessionContext, getSubClaim } from '../../session/SessionProvider'
import ValidUserEmailQuery from '../../../relay/queries/ValidUserEmail'
import CreateAvatarMutation from '../../../relay/mutations/CreateAvatar'
import { generateRandomString } from '../../../utils'
import CreateWorkspaceMutation from '../../../relay/mutations/CreateWorkspace'

type NewWorkspaceSubmissionState =
    | 'NOT_SUBMITTING'
    | 'UPLOADING_AVATAR'
    | 'START_WORKSPACE_COMMIT'
    | 'COMPLETE'

type CreateWorkspaceFormProps = {
    close: () => void
}

function CreateWorkspaceForm({ close }: CreateWorkspaceFormProps) {
    const relayEnv = useRelayEnvironment()
    const [commitWorkspaceMutation] = useMutation(CreateWorkspaceMutation)
    const [commitAvatarMutation] = useMutation(CreateAvatarMutation)

    const claims = useContext(SessionContext)!

    const fileInputRef = useRef<HTMLInputElement>(null)
    const [submissionState, setSubmissionState] =
        useState<NewWorkspaceSubmissionState>('NOT_SUBMITTING')
    const [name, setName] = useState<string>('')
    const [description, setDescription] = useState<string>('')
    const [avatar, setAvatar] = useState<File | null>(null)
    const [avatarId, setAvatarId] = useState<string | null>(null)
    const [memberEmails, setMemberEmails] = useState<string[]>([])
    const [searchEmailText, setEmailSearchText] = useState<string>('')
    const [submittedSearchEmailText, setSubmittedSearchEmailText] =
        useState<string>('')
    const [alertInvalidEmail, setAlertInvalidEmail] = useState<boolean>(false)

    useEffect(() => {
        if (submittedSearchEmailText === '') return
        if (memberEmails.includes(submittedSearchEmailText)) {
            setAlertInvalidEmail(true)
            return
        }

        fetchQuery(relayEnv, ValidUserEmailQuery, {
            email: submittedSearchEmailText,
        }).subscribe({
            next: (data: any) => {
                if (data.validUserEmail.valid) {
                    setEmailSearchText('')
                    setMemberEmails([submittedSearchEmailText, ...memberEmails])
                } else {
                    setAlertInvalidEmail(true)
                }
            },
            error: (error: any) => console.log(error),
        })
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [submittedSearchEmailText])

    useEffect(() => {
        if (!alertInvalidEmail) return

        setTimeout(() => {
            setAlertInvalidEmail(false)
        }, 3000)
    }, [alertInvalidEmail])

    useEffect(() => {
        if (submissionState !== 'START_WORKSPACE_COMMIT') {
            return
        }

        commitWorkspaceMutation({
            variables: {
                creatorId: getSubClaim(claims),
                workspace: {
                    name,
                    description,
                    avatarId: avatarId,
                    invitedUserEmails: memberEmails,
                },
            },
            onCompleted: (response, errors) => {
                if (errors === null) {
                    setSubmissionState('COMPLETE')
                    close()
                }
            },
        })
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [submissionState])

    const handleFileUpload = () => {
        const uploadedFiles = fileInputRef.current?.files
        if (uploadedFiles && uploadedFiles.length > 0) {
            setAvatar(uploadedFiles[uploadedFiles.length - 1])
        }
    }

    const uploadToStore = (file: File): string => {
        // TODO
        return generateRandomString(64)
    }

    const initSubmit = async () => {
        setSubmissionState('UPLOADING_AVATAR')
        if (avatar !== null) {
            const storeKey = uploadToStore(avatar)

            commitAvatarMutation({
                variables: {
                    file: {
                        name: avatar.name,
                        storeKey,
                        uploaderId: getSubClaim(claims),
                    },
                },
                onCompleted: (data: any, errors) => {
                    if (errors === null) {
                        setAvatarId(data.createAvatar.id)
                        setSubmissionState('START_WORKSPACE_COMMIT')
                    }
                },
            })
        } else {
            setSubmissionState('START_WORKSPACE_COMMIT')
        }
    }

    return (
        <form className="flex h-full w-full flex-col gap-y-3 text-sm">
            <InputTextField label="Name" onChange={setName} />
            <InputTextField label="Description" onChange={setDescription} />
            <div
                className="mb-1 flex max-w-[16rem] flex-col gap-y-1 truncate 
                    font-extralight"
            >
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
                onSubmit={() => setSubmittedSearchEmailText(searchEmailText)}
                alertInvalidSearch={alertInvalidEmail}
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

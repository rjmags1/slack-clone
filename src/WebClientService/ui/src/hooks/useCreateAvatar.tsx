import { uploadToStore } from '../utils'
import { useMutation } from 'react-relay'
import { useState } from 'react'
import { PayloadError } from 'relay-runtime'
import CreateAvatarMutation from '../relay/mutations/CreateAvatar'
import { CreateAvatarMutation$data } from '../relay/mutations/__generated__/CreateAvatarMutation.graphql'

function useCreateAvatar(avatar: File | null, uploaderId: string) {
    const [commitAvatarMutation] = useMutation(CreateAvatarMutation)
    const [loading, setLoading] = useState<boolean>(false)
    const [data, setData] = useState<CreateAvatarMutation$data | null>(null)
    const [errors, setErrors] = useState<PayloadError[] | null>(null)

    const createAvatar = () => {
        if (!avatar) return
        setLoading(true)
        const storeKey = uploadToStore(avatar)
        commitAvatarMutation({
            variables: {
                file: {
                    name: avatar.name,
                    storeKey,
                    uploaderId,
                },
            },
            onCompleted: (data: object, errors) => {
                setLoading(false)
                if (errors === null) {
                    setData(data as CreateAvatarMutation$data)
                    setErrors(null)
                } else {
                    setData(null)
                    setErrors(errors)
                }
            },
        })
    }

    return { createAvatar, loading, data, errors }
}

export default useCreateAvatar

import graphql from 'babel-plugin-relay/macro'

export const CreateAvatarMutation = graphql`
    mutation CreateAvatarMutation($file: FileInput!) {
        createAvatar(file: $file) {
            id
            name
            storeKey
        }
    }
`

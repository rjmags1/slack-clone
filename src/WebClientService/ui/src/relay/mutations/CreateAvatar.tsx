import graphql from 'babel-plugin-relay/macro'

const CreateAvatarMutation = graphql`
    mutation CreateAvatarMutation($file: FileInput!) {
        createAvatar(file: $file) {
            id
            name
            storeKey
        }
    }
`

export default CreateAvatarMutation

import graphql from 'babel-plugin-relay/macro'

const ValidUserEmailQuery = graphql`
    query ValidUserEmailQuery($email: String!) {
        validUserEmail(email: $email) {
            valid
        }
    }
`

export default ValidUserEmailQuery

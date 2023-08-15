import graphql from 'babel-plugin-relay/macro'

export const ValidUserEmailQuery = graphql`
    query ValidUserEmailQuery($email: String!) {
        validUserEmail(email: $email) {
            valid
        }
    }
`

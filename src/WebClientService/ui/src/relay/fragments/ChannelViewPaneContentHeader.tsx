import graphql from 'babel-plugin-relay/macro'

const ChannelViewPaneContentHeaderFragment = graphql`
    fragment ChannelViewPaneContentHeaderFragment on Channel {
        name
        private
    }
`

export default ChannelViewPaneContentHeaderFragment

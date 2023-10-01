import { useLocation } from 'react-router-dom'
import ChannelViewPaneContent from './channel/ChannelViewPaneContent'
import DirectMessageGroupViewPaneContent from './directMessageGroup/DirectMessageGroupViewPaneContent'

export enum ViewPaneContent {
    ChannelViewPaneContent = 1,
    DirectMessageGroupViewPaneContent,
}

const viewPaneContentFromUrlPath = (pathname: string) => {
    const workspaceIdIdx = pathname.indexOf('workspace/') + 10
    const contentTypeStartIdx = pathname.indexOf('/', workspaceIdIdx) + 1
    const contentTypeStopIdx = pathname.indexOf('/', contentTypeStartIdx)
    if (workspaceIdIdx < 0 || contentTypeStartIdx < 0 || contentTypeStopIdx < 0)
        return

    const contentType = pathname.slice(contentTypeStartIdx, contentTypeStopIdx)
    switch (contentType) {
        case 'channel':
            return ViewPaneContent.ChannelViewPaneContent
        case 'dms':
            return ViewPaneContent.DirectMessageGroupViewPaneContent
        default:
            return
    }
}

function WorkspacePageViewPane() {
    let rendered = null
    const location = useLocation()
    switch (viewPaneContentFromUrlPath(location.pathname)) {
        case ViewPaneContent.ChannelViewPaneContent:
            rendered = <ChannelViewPaneContent />
            break
        case ViewPaneContent.DirectMessageGroupViewPaneContent:
            rendered = <DirectMessageGroupViewPaneContent />
            break
    }

    return <div className="h-full flex-shrink-[2] grow">{rendered}</div>
}

export default WorkspacePageViewPane

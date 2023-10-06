import MDEditor from '@uiw/react-md-editor'
import { useState } from 'react'
import rehypeSanitize from 'rehype-sanitize'

function MessageEditor() {
    const [value, setValue] = useState<string>()
    return (
        <div className="editor-container w-full">
            <div className="h-[max(20vh,120px)] w-full shrink-0">
                <MDEditor
                    value={value}
                    onChange={setValue}
                    previewOptions={{
                        rehypePlugins: [[rehypeSanitize]],
                    }}
                    preview="edit"
                    autoFocus={true}
                />
                <MDEditor.Markdown
                    source={value}
                    style={{ whiteSpace: 'pre-wrap' }}
                />
            </div>
        </div>
    )
}

export default MessageEditor

import { useRef } from 'react'

type FileInputProps = {
    label?: string
    id: string
    accept: string
    className?: string
    setFile: (f: File) => void
}

function FileInput({ label, id, accept, className, setFile }: FileInputProps) {
    const ref = useRef<HTMLInputElement>(null)

    const handleFileUpload = () => {
        const uploadedFiles = ref.current?.files
        if (uploadedFiles && uploadedFiles.length > 0) {
            setFile(uploadedFiles[uploadedFiles.length - 1])
        }
    }

    return (
        <>
            <label htmlFor={id}>{label}</label>
            <input
                ref={ref}
                type="file"
                id={id}
                name={id}
                accept={accept}
                onChange={handleFileUpload}
                className={className}
            />
        </>
    )
}

export default FileInput

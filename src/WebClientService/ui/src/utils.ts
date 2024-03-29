export const generateRandomString = (length: number) => {
    const characters =
        'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789'
    let result = ''

    for (let i = 0; i < length; i++) {
        const randomIndex = Math.floor(Math.random() * characters.length)
        result += characters.charAt(randomIndex)
    }

    return result
}

export const uploadToStore = (file: File): string => {
    // TODO
    return generateRandomString(64)
}

export const debounce = (fn: () => void, delay: number) => setTimeout(fn, delay)

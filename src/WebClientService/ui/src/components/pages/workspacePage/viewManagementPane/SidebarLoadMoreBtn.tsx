import Button from '../../../lib/Button'

type SidebarLoadMoreBtnProps = {
    loadMore: () => void
}

function SidebarLoadMoreBtn({ loadMore }: SidebarLoadMoreBtnProps) {
    return (
        <Button
            className="mt-1 w-full rounded-md bg-sky-800 py-1 
                text-xs hover:bg-sky-950"
            onClick={loadMore}
        >
            Load more
        </Button>
    )
}

export default SidebarLoadMoreBtn

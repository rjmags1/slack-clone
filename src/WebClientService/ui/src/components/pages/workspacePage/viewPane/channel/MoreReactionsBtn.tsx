import Button from '../../../../lib/Button'

function MoreReactionsBtn() {
    return (
        <Button
            className="-mt-[1px] h-fit rounded px-[.45rem] py-[1px] hover:bg-zinc-600"
            onClick={() => alert('not implemented')}
        >
            ⋮
        </Button>
    )
}

export default MoreReactionsBtn

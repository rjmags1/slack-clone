import Button from '../../../lib/Button'
import DatePicker from '../../../lib/DatePicker'
import SearchField from '../../../lib/SearchField'
import TimeField from '../../../lib/TimeField'

function UpdateUserStatusForm() {
    return (
        <form
            className="mt-3 flex w-full min-w-max flex-col 
                items-center justify-start gap-y-3"
        >
            <SearchField
                label="Status: "
                containerClassName="flex min-w-max gap-x-[2px] font-sm"
            />
            <hr className="my-2 w-full opacity-60" />
            <h5 className="w-full text-left">Until...</h5>
            <DatePicker
                label="Date: "
                containerClassName="w-full flex justify-start gap-x-1 
                    font-sm font-extralight min-w-max"
            />
            <TimeField label="Time: " />
            <Button
                className="w-full rounded bg-sky-600 px-2 py-1 
                    hover:bg-sky-800"
            >
                Submit
            </Button>
        </form>
    )
}

export default UpdateUserStatusForm

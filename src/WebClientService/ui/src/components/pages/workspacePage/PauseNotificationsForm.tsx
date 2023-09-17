import Button from '../../lib/Button'
import DatePicker from '../../lib/DatePicker'
import TimeField from '../../lib/TimeField'

function PauseNotificationsForm() {
    return (
        <form
            className="mt-3 flex w-full min-w-max flex-col 
            items-center justify-start gap-y-3"
        >
            <DatePicker
                label="Date: "
                containerClassName="w-full flex justify-start gap-x-1 
                font-sm font-extralight min-w-max"
            />
            <TimeField label="Time: " />
            <Button
                className="w-full rounded bg-red-600 px-2 py-1 
                hover:bg-red-800"
            >
                Reset
            </Button>
        </form>
    )
}

export default PauseNotificationsForm

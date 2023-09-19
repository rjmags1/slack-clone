import { useState } from 'react'
import { DateValue } from 'react-aria'
import InputTextField from '../../../lib/InputTextField'
import DatePicker from '../../../lib/DatePicker'
import Checkbox from '../../../lib/Checkbox'
import Button from '../../../lib/Button'

function WorkspaceSearchFilterForm() {
    const [fromField, setFromField] = useState('')
    const [inField, setInField] = useState('')
    const [withField, setWithField] = useState('')
    const [userChannelsField, setUserChannelsField] = useState(false)
    const [fromDateField, setFromDateField] = useState<DateValue | null>(null)
    const [toDateField, setToDateField] = useState<DateValue | null>(null)

    const clearFields = () => {
        setFromField('')
        setInField('')
        setWithField('')
        setUserChannelsField(false)
        setFromDateField(null)
        setToDateField(null)
    }

    return (
        <form
            className="flex h-fit w-full flex-col items-start 
                justify-start gap-y-2 text-sm"
        >
            <InputTextField
                label="From: "
                value={fromField}
                onChange={setFromField}
                containerClassName="flex h-fit w-full gap-x-1 font-extralight"
                inputClassName="w-full border-b border-b-white bg-inherit 
                    p-[2px] text-xs text-white outline-none"
            />
            <InputTextField
                label="In: "
                value={inField}
                onChange={setInField}
                containerClassName="flex h-fit w-full gap-x-1 font-extralight"
                inputClassName="w-full border-b border-b-white bg-inherit 
                    p-[2px] text-xs text-white outline-none"
            />
            <InputTextField
                label="With: "
                value={withField}
                onChange={setWithField}
                containerClassName="flex h-fit w-full gap-x-1 font-extralight"
                inputClassName="w-full border-b border-b-white bg-inherit 
                    p-[2px] text-xs text-white outline-none"
            />
            <div className="flex w-full items-center justify-start gap-x-2">
                <DatePicker
                    label="From: "
                    value={fromDateField}
                    onChange={setFromDateField}
                />
                <DatePicker
                    label="To: "
                    value={toDateField}
                    onChange={setToDateField}
                />
            </div>
            <Checkbox
                isSelected={userChannelsField}
                onChange={setUserChannelsField}
            >
                Only my channels
            </Checkbox>
            <Button
                className="h-fit w-full rounded-md bg-zinc-300 p-2 
                    text-black hover:bg-zinc-500"
                onClick={clearFields}
            >
                Clear filters
            </Button>
        </form>
    )
}

export default WorkspaceSearchFilterForm

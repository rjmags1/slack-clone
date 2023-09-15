import {
    AriaCalendarProps,
    DateValue,
    useCalendar,
    useCalendarCell,
    useCalendarGrid,
    useLocale,
} from 'react-aria'
import { CalendarState, useCalendarState } from 'react-stately'
import { createCalendar, getWeeksInMonth } from '@internationalized/date'
import Button from './Button'
import { useRef } from 'react'

function Calendar(props: AriaCalendarProps<DateValue>) {
    const { locale } = useLocale()
    const state = useCalendarState({
        ...props,
        locale,
        createCalendar,
    })

    const { calendarProps, prevButtonProps, nextButtonProps, title } =
        useCalendar(props, state)

    return (
        <div {...calendarProps} className="calendar">
            <div
                className="header flex w-full items-center 
                    justify-between gap-x-6"
            >
                <h2>{title}</h2>
                <div className="mt-2 flex h-fit items-center gap-x-2">
                    <Button
                        {...prevButtonProps}
                        onClick={state.focusPreviousPage}
                        className="h-min rounded-full border border-white 
                            p-1 pb-[0.33rem] pr-[0.3rem] leading-[0.65rem]
                            hover:bg-zinc-800"
                    >
                        &lt;
                    </Button>
                    <Button
                        className="h-min rounded-full border border-white 
                            p-1 pb-[0.33rem] pr-[0.2rem] leading-[0.65rem]
                            hover:bg-zinc-800"
                        {...nextButtonProps}
                        onClick={state.focusNextPage}
                    >
                        &gt;
                    </Button>
                </div>
            </div>
            <CalendarGrid state={state} />
        </div>
    )
}

function CalendarGrid({ state, ...props }: { state: CalendarState }) {
    const { locale } = useLocale()
    const { gridProps, headerProps, weekDays } = useCalendarGrid(props, state)

    const weeksInMonth = getWeeksInMonth(state.visibleRange.start, locale)

    return (
        <table
            {...gridProps}
            className="flex h-full w-full flex-col items-center justify-center"
        >
            <thead {...headerProps} className="w-full">
                <tr className="flex w-full gap-x-1">
                    {weekDays.map((day, index) => (
                        <th className="w-full text-center" key={index}>
                            {day}
                        </th>
                    ))}
                </tr>
            </thead>
            <tbody className="w-full font-extralight">
                {[...Array(weeksInMonth)].map((_, weekIndex) => (
                    <tr key={weekIndex} className="flex w-full gap-x-1">
                        {state
                            .getDatesInWeek(weekIndex)
                            .map((date, i) =>
                                date ? (
                                    <CalendarCell
                                        key={i}
                                        state={state}
                                        date={date}
                                    />
                                ) : (
                                    <td key={i} />
                                )
                            )}
                    </tr>
                ))}
            </tbody>
        </table>
    )
}

function CalendarCell({
    state,
    date,
    ...props
}: {
    state: CalendarState
    date: DateValue
}) {
    const ref = useRef(null)
    const {
        cellProps,
        buttonProps,
        isSelected,
        isOutsideVisibleRange,
        formattedDate,
    } = useCalendarCell({ date: date as any }, state, ref)

    return (
        <td
            {...cellProps}
            className="flex h-full w-full items-center justify-center leading-3"
        >
            <div
                {...buttonProps}
                ref={ref}
                hidden={isOutsideVisibleRange}
                className={`${isSelected ? 'bg-zinc-900' : ''}
                    flex w-full items-center justify-center rounded-full
                    p-1 text-center text-xs hover:bg-zinc-800`}
            >
                {formattedDate}
            </div>
        </td>
    )
}

export default Calendar

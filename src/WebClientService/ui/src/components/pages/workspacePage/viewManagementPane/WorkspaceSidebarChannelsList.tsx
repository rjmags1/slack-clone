import ListBox from '../../../lib/Listbox'
import { Item } from 'react-stately'
import { useState } from 'react'
import WorkspaceSidebarChannelListing from './WorkspaceSidebarChannelListing'

function WorkspaceSidebarChannelsList() {
    const [items, setItems] = useState([
        { id: 1, name: 'Aardvark' },
        { id: 2, name: 'Cat' },
        { id: 3, name: 'Dog' },
        { id: 4, name: 'Kangaroo' },
        { id: 5, name: 'Koala' },
        { id: 6, name: 'Penguin' },
        { id: 7, name: 'Snake' },
        { id: 8, name: 'Turtle' },
        { id: 9, name: 'Wombat' },
        { id: 11, name: 'Aardvark' },
        { id: 21, name: 'Cat' },
        { id: 31, name: 'Dog' },
        { id: 41, name: 'Kangaroo' },
        { id: 51, name: 'Koala' },
        { id: 61, name: 'Penguin' },
        { id: 71, name: 'Snake' },
        { id: 81, name: 'Turtle' },
        { id: 91, name: 'Wombat' },
        { id: 1234, name: 'Aardvark' },
        { id: 2234, name: 'Cat' },
        { id: 3234, name: 'Dog' },
        { id: 4234, name: 'Kangaroo' },
        { id: 5234, name: 'Koala' },
        { id: 6234, name: 'Penguin' },
        { id: 7234, name: 'Snake' },
        { id: 8234, name: 'Turtle' },
        { id: 9234, name: 'Wombat' },
        { id: 11234, name: 'Aardvark' },
        { id: 21234, name: 'Cat' },
        { id: 31234, name: 'Dog' },
        { id: 41234, name: 'Kangaroo' },
        { id: 51234, name: 'Koala' },
        { id: 61234, name: 'Penguin' },
        { id: 71234, name: 'Snake' },
        { id: 81234, name: 'Turtle' },
        { id: 91234, name: 'Wombat' },
    ])

    return (
        <ListBox
            items={items}
            selectionMode="single"
            listClassName="no-scrollbar overflow-y-auto text-xs"
        >
            {(item) => (
                <Item>
                    <WorkspaceSidebarChannelListing item={item} />
                </Item>
            )}
        </ListBox>
    )
}

export default WorkspaceSidebarChannelsList

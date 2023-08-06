import TestPage from './pages/TestPage'
import RelayEnvironment from './relay/RelayEnvironment'
import { createBrowserRouter, RouterProvider } from 'react-router-dom'

const router = createBrowserRouter([
    {
        path: '/',
        element: <TestPage />,
    },
])

function App() {
    return (
        <RelayEnvironment>
            <div className="App">
                <RouterProvider router={router} />
            </div>
        </RelayEnvironment>
    )
}

export default App

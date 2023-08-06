import TestPage from './pages/TestPage'
import WorkspacesPage from './pages/WorkspacesPage'
import RelayEnvironment from './relay/RelayEnvironment'
import { createBrowserRouter, RouterProvider } from 'react-router-dom'
import SessionProvider from './session/SessionProvider'

const router = createBrowserRouter([
    {
        path: '/',
        element: <WorkspacesPage />,
    },
    {
        path: '/test',
        element: <TestPage />,
    },
])

function App() {
    return (
        <div className="App">
            <RelayEnvironment>
                <SessionProvider>
                    <RouterProvider router={router} />
                </SessionProvider>
            </RelayEnvironment>
        </div>
    )
}

export default App
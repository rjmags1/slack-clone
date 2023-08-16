import TestPage from './pages/TestPage'
import WorkspacesPage from './pages/workspacesPage/WorkspacesPage'
import RelayEnvironment from './relay/RelayEnvironment'
import { createBrowserRouter, RouterProvider } from 'react-router-dom'
import SessionProvider from './session/SessionProvider'
import AuthGuard from './session/AuthGuard'
import WorkspacePage from './pages/workspacePage/WorkspacePage'

const router = createBrowserRouter([
    {
        path: '/',
        element: (
            <AuthGuard>
                <WorkspacesPage />
            </AuthGuard>
        ),
    },
    {
        path: '/workspaces',
        element: (
            <AuthGuard>
                <WorkspacesPage />
            </AuthGuard>
        ),
    },
    {
        path: '/workspace/:workspaceId',
        element: (
            <AuthGuard>
                <WorkspacePage />
            </AuthGuard>
        ),
    },
    {
        path: '/test',
        element: <TestPage />,
    },
])

function App() {
    return (
        <div id="App" className="h-screen w-screen bg-zinc-700">
            <RelayEnvironment>
                <SessionProvider>
                    <RouterProvider router={router} />
                </SessionProvider>
            </RelayEnvironment>
        </div>
    )
}

export default App

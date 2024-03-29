import { useEffect, useState } from 'react'

function TestPage() {
    const [userClaims, setUserClaims] = useState<any>(null)
    const [loggedIn, setLoggedIn] = useState<boolean>(false)
    const [localApiResults, setLocalApiResults] = useState<any>(null)
    const [remoteApiResults, setRemoteApiResults] = useState<any>(null)

    useEffect(() => {
        ;(async function () {
            var req = new Request('/bff/user', {
                headers: new Headers({
                    'X-CSRF': '1',
                }),
            })

            try {
                var resp = await fetch(req)
                if (resp.ok) {
                    const claims = await resp.json()
                    setUserClaims(claims)
                    setLoggedIn(true)
                }
            } catch (e) {
                //
            }
        })()
    })

    function login() {
        window.location.href = '/bff/login?returnUrl=/'
    }

    function logout() {
        if (userClaims) {
            var logoutUrl = userClaims.find(
                (claim: any) => claim.type === 'bff:logout_url'
            ).value
            window.location.href = logoutUrl
        } else {
            window.location.href = '/bff/logout'
        }
    }

    async function localApi() {
        var req = new Request('/local/identity', {
            headers: new Headers({
                'X-CSRF': '1',
            }),
        })

        try {
            var resp = await fetch(req)

            let data
            if (resp.ok) {
                data = await resp.json()
            }
            setLocalApiResults(JSON.stringify(data, null, 2))
        } catch (e) {
            setLocalApiResults('local api call failed')
        }
        setRemoteApiResults(null)
    }

    async function remoteApi() {
        const query = `
        query WorkspacePageQuery(
            $userId: ID!, 
            $channelsFilter: ChannelsFilter!,
            $directMessageGroupsFilter: DirectMessageGroupsFilter!,
            $starredFilter: StarredFilter!,
        ) {
            workspacePageData(userId: $userId) {
                id
                channels(first: 10, filter: $channelsFilter) {
                    totalEdges
                    pageInfo {
                        hasNextPage
                    }
                    edges {
                        cursor
                        node {
                            id
                        }
                    }
                }
                directMessageGroups(first: 10, filter: $directMessageGroupsFilter) {
                    totalEdges
                    edges {
                        cursor
                        node {
                            id
                        }
                    }
                }
                starred(first: 10, filter: $starredFilter) {
                    totalEdges
                    pageInfo {
                        hasNextPage
                    }
                    edges {
                        node {
                            id
                            createdAt
                        }
                    }
                }
            }
        }`
        const userId = 'aea5a17c-b73e-43a5-be7b-8352a2adce5d'
        const workspaceId = '2286c703-e564-4cd2-9df1-25924abb9900'
        const variables = {
            userId,
            workspaceId,
            channelsFilter: {
                userId,
                workspaceId,
            },
            directMessageGroupsFilter: {
                userId,
                workspaceId,
            },
            starredFilter: {
                userId,
                workspaceId,
            },
        }

        var req = new Request('/remote/graphql', {
            headers: new Headers({
                'X-CSRF': '1',
                'Content-Type': 'application/json',
            }),
            method: 'POST',
            body: JSON.stringify({ query, variables }),
        })

        try {
            var resp = await fetch(req)

            let data
            if (resp.ok) {
                data = await resp.json()
            }
            setRemoteApiResults(JSON.stringify(data, null, 2))
        } catch (e) {
            setRemoteApiResults('error calling remote API')
        }
        setLocalApiResults(null)
    }

    return (
        <>
            <div className="flex flex-row gap-2 text-xs">
                <button
                    id="login"
                    onClick={login}
                    className="rounded-sm border border-green-300 p-2"
                >
                    Login
                </button>
                <button
                    id="local"
                    onClick={localApi}
                    className="rounded-sm border border-green-300 p-2"
                >
                    Call Local API
                </button>
                <button
                    id="remote"
                    onClick={remoteApi}
                    className="rounded-sm border border-green-300 p-2"
                >
                    Call Remote API
                </button>
                <button
                    id="logout"
                    onClick={logout}
                    className="rounded-sm border border-green-300 p-2"
                >
                    Logout
                </button>
            </div>
            <pre id="results" className="text-xs">
                {remoteApiResults}
                {localApiResults}
                {loggedIn &&
                    !(localApiResults || remoteApiResults) &&
                    userClaims
                        .map((c: object) => JSON.stringify(c, null, 2))
                        .join('\n')}
                {!loggedIn && 'user not logged in'}
            </pre>
        </>
    )
}

export default TestPage

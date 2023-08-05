import { useEffect, useState } from "react";

function TestPage() {
    const [userClaims, setUserClaims] = useState<any>(null);
    const [loggedIn, setLoggedIn] = useState<boolean>(false);
    const [localApiResults, setLocalApiResults] = useState<any>(null);
    const [remoteApiResults, setRemoteApiResults] = useState<any>(null);

    useEffect(() => {
        (async function () {
            var req = new Request("/bff/user", {
                headers: new Headers({
                    "X-CSRF": "1",
                }),
            });

            try {
                var resp = await fetch(req);
                if (resp.ok) {
                    const claims = await resp.json();
                    setUserClaims(claims);
                    setLoggedIn(true);
                }
            } catch (e) {
                //
            }
        })();
    });

    function login() {
        window.location.href = "/bff/login?returnUrl=/";
    }

    function logout() {
        if (userClaims) {
            var logoutUrl = userClaims.find(
                (claim: any) => claim.type === "bff:logout_url"
            ).value;
            window.location.href = logoutUrl;
        } else {
            window.location.href = "/bff/logout";
        }
    }

    async function localApi() {
        var req = new Request("/local/identity", {
            headers: new Headers({
                "X-CSRF": "1",
            }),
        });

        try {
            var resp = await fetch(req);

            let data;
            if (resp.ok) {
                data = await resp.json();
            }
            setLocalApiResults(JSON.stringify(data, null, 2));
        } catch (e) {
            setLocalApiResults("local api call failed");
        }
        setRemoteApiResults(null);
    }

    async function remoteApi() {
        const query = `
        query WorkspacesPageQuery($userId: ID!, $workspacesFilter: WorkspacesFilterInput!) {
            workspacesPageData(userId: $userId, workspacesFilter: $workspacesFilter) {
                user {
                    id
                    createdAt
                    personalInfo {
                        email
                        userNotificationsPreferences {
                            notifSound
                        }
                    }
                }
                workspaces {
                    totalEdges
                    pageInfo {
                        startCursor
                        endCursor
                        hasNextPage
                        hasPreviousPage
                    }
                    edges {
                        node {
                            id
                            createdAt
                            description
                            name
                            numMembers
                            avatar {
                                id
                                storeKey
                            }
                        }
                    }
                }
            }
        }`;
        const variables = {
            userId: "e6560874-6d15-4d94-bc53-f2240ab364e0",
            workspacesFilter: {
                cursor: {
                    first: 2,
                },
                userId: "e6560874-6d15-4d94-bc53-f2240ab364e0",
            },
        };

        var req = new Request("/remote/graphql", {
            headers: new Headers({
                "X-CSRF": "1",
                "Content-Type": "application/json",
            }),
            method: "POST",
            body: JSON.stringify({ query, variables }),
        });

        try {
            var resp = await fetch(req);

            let data;
            if (resp.ok) {
                data = await resp.json();
            }
            setRemoteApiResults(JSON.stringify(data, null, 2));
        } catch (e) {
            setRemoteApiResults("error calling remote API");
        }
        setLocalApiResults(null);
    }

    return (
        <div>
            <button id="login" onClick={login}>
                Login
            </button>
            <button id="local" onClick={localApi}>
                Call Local API
            </button>
            <button id="remote" onClick={remoteApi}>
                Call Remote API
            </button>
            <button id="logout" onClick={logout}>
                Logout
            </button>
            <pre id="results">
                {remoteApiResults}
                {localApiResults}
                {loggedIn &&
                    !(localApiResults || remoteApiResults) &&
                    userClaims
                        .map((c: object) => JSON.stringify(c, null, 2))
                        .join("\n")}
                {!loggedIn && "user not logged in"}
            </pre>
        </div>
    );
}

export default TestPage;

function log() {
    document.getElementById("results").innerText = "";
  
    Array.prototype.forEach.call(arguments, function (msg) {
        if (typeof msg !== "undefined") {
            if (msg instanceof Error) {
                msg = "Error: " + msg.message;
            } else if (typeof msg !== "string") {
                msg = JSON.stringify(msg, null, 2);
            }
            document.getElementById("results").innerText += msg + "\r\n";
        }
    });
}

let userClaims = null;

(async function () {
    var req = new Request("/bff/user", {
            headers: new Headers({
            "X-CSRF": "1",
        }),
    });

    try {
        var resp = await fetch(req);
        if (resp.ok) {
            userClaims = await resp.json();

            log("user logged in", userClaims);
        } else if (resp.status === 401) {
            log("user not logged in");
        }
    } catch (e) {
        log("error checking user status");
    }
})();

document.getElementById("login").addEventListener("click", login, false);
document.getElementById("local").addEventListener("click", localApi, false);
document.getElementById("remote").addEventListener("click", remoteApi, false);
document.getElementById("logout").addEventListener("click", logout, false);

function login() {
    window.location = "/bff/login";
}

function logout() {
    if (userClaims) {
        var logoutUrl = userClaims.find(
        (claim) => claim.type === "bff:logout_url"
        ).value;
        window.location = logoutUrl;
    } else {
        window.location = "/bff/logout";
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
        log("Local API Result: " + resp.status, data);
    } catch (e) {
        log("error calling local API");
    }
}
  
async function remoteApi() {
    /*
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
        }`
    const variables = { 
        userId: "e6560874-6d15-4d94-bc53-f2240ab364e0",
        workspacesFilter: {
            cursor: {
                first: 2
            },
            userId: "e6560874-6d15-4d94-bc53-f2240ab364e0"
        }
    }
    */
    const query = `
        query TestWorkspaceMembers($usersFilter: UsersFilterInput!, $workspaceId: ID!) {
            testWorkspaceMembers(usersFilter: $usersFilter, workspaceId: $workspaceId) {
                id
                name
                members(usersFilter: $usersFilter) {
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
                            title
                            avatar {
                                id
                                storeKey
                            }
                            joinedAt
                            user {
                                id
                                username
                                personalInfo {
                                    firstName
                                    userNotificationsPreferences {
                                        notifSound
                                    }
                                }
                            }
                            workspace {
                                id
                            }
                            workspaceMemberInfo {
                                admin
                                owner
                                workspaceAdminPermissions {
                                    admin {
                                        id
                                    }
                                    all
                                    invite
                                    kick
                                    adminGrant
                                    adminRevoke
                                    grantAdminPermissions
                                    revokeAdminPermissions
                                    editMessages
                                    deleteMessages
                                }
                            }
                        }
                    }
                }
            }
        }`
    const variables = {
        workspaceId: "0f784094-8fb4-4d3c-a163-ddde89e27cb8",
        usersFilter: {
            cursor: {
                first: 10
            },
            workspaceId: "0f784094-8fb4-4d3c-a163-ddde89e27cb8",
        }
    }

    var req = new Request("/remote/graphql", {
        headers: new Headers({
            "X-CSRF": "1",
            "Content-Type": 'application/json'
        }),
        method: "POST",
        body: JSON.stringify({ query, variables })
    });
  
    try {
        var resp = await fetch(req);
    
        let data;
        if (resp.ok) {
            data = await resp.json();
        }
        log("Remote API Result: " + resp.status, data);
    } catch (e) {
        log("error calling remote API");
    }
}
  
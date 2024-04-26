using Common.SlackCloneGraphQL;
using Common.SlackCloneGraphQL.Types;
using Common.Utils;
using GraphQLParser;
using GraphQLParser.AST;
using PersistenceService.Utils.GraphQL;
using SlackCloneGraphQL;

namespace DotnetTest.ApiService.FieldAnalyzerTests;

[Trait("Category", "Order 3")]
[Collection("Database collection 3")]
public class FieldAnalyzerTests
{
    private const string _workspacesQuery1 =
        @"
        query WorkspacesQuery {
            workspaces(arg: testArg) {
                pageInfo {
                    hasNextPage
                }
                edges {
                    node {
                        id
                        createdAt
                        description
                        name
                        avatar {
                            id
                            storeKey
                        }
                        numMembers
                    }
                }
            }
        }
    ";

    private const string _workspacesQuery2 =
        @"
        query WorkspacesQuery {
            workspaces(arg: testArg) {
                ...testFragment
            }
        }

        fragment testFragment on WorkspacesConnection {
            pageInfo {
                hasNextPage
            }
            edges {
                node {
                    id
                    createdAt
                    description
                    name
                    avatar {
                        id
                        storeKey
                    }
                    numMembers
                }
            }
        }
    ";

    private const string _workspacesQuery3 =
        @"
        query WorkspacesQuery {
            workspaces(arg: testArg) {
                pageInfo {
                    hasNextPage
                }
                edges {
                    ...testFragment
                }
            }
        }

        fragment testFragment on WorkspacesConnectionEdge {
            node {
                id
                createdAt
                description
                name
                avatar {
                    id
                    storeKey
                }
                numMembers
            }
        }
    ";

    private const string _workspacesQuery4 =
        @"
        query WorkspacesQuery {
            workspaces(arg: testArg) {
                pageInfo {
                    hasNextPage
                }
                edges {
                    node {
                        ...testFragment
                    }
                }
            }
        }

        fragment testFragment on Workspace {
            id
            createdAt
            description
            name
            avatar {
                id
                storeKey
            }
            numMembers
        }
    ";

    private const string _workspacesQuery5 =
        @"
        query WorkspacesQuery {
            workspaces(arg: testArg) {
                ...testFragment2
            }
        }

        fragment testFragment2 on WorkspacesConnection {
            pageInfo {
                hasNextPage
            }
            edges {
                node {
                    ...testFragment
                }
            }
        }

        fragment testFragment on Workspace {
            id
            createdAt
            description
            name
            avatar {
                id
                storeKey
            }
            numMembers
        }
    ";

    private const string _workspacesQuery6 =
        @"
        query WorkspacesQuery {
            workspaces(arg: testArg) {
                ...testFragment2
            }
        }

        fragment testFragment2 on WorkspacesConnection {
            totalEdges
            pageInfo {
                ...testFragment2
            }
            edges {
                node {
                    ...testFragment
                }
            }
        }

        fragment testFragment2 on PageInfo {
            hasNextPage
            hasPreviousPage
            startCursor
            endCursor
        }

        fragment testFragment on Workspace {
            id
            createdAt
            description
            name
            avatar {
                id
                storeKey
            }
            numMembers
        }
    ";

    private readonly string _workspacesQueryWithFragments =
        @"
        query WorkspacesQuery {
            workspaces(arg: testArg) {
                pageInfo {
                    hasNextPage
                }
                edges {
                    ...edgeFragment
                }
            }
        }

        fragment edgeFragment on WorkspacesEdge {
            node {
                id
            }
        }
    ";

    private const string _userQuery =
        @"
        query UserQuery {
            user(arg: testArg) {
                id
                personalInfo {
                    email
                }
            }
        }
    ";

    private const string _userQuery2 =
        @"
        query UserQuery {
            user {
                id
                avatar {
                    id
                    storeKey
                }
                ...testFragment
                username
                personalInfo {
                    email
                    emailConfirmed
                    firstName
                    lastName
                    userNotificationsPreferences {
                        notifSound
                        allowAlertsStartTimeUTC
                        allowAlertsEndTimeUTC
                        pauseAlertsUntil
                    }
                    theme {
                        id
                        name
                    }
                    timezone
                }
            }
        }

        fragment testFragment on User {
            createdAt,
            onlineStatus
        }
    ";

    private const string _userQuery3 =
        @"
        query UserQuery {
            user {
                id
                avatar {
                    id
                    storeKey
                }
                ...testFragment
                username
                ...testFragment2
            }
        }

        fragment testFragment on User {
            createdAt,
            onlineStatus
        }

        fragment testFragment2 on User {
            personalInfo {
                email
                emailConfirmed
                firstName
                lastName
                userNotificationsPreferences {
                    notifSound
                    allowAlertsStartTimeUTC
                    allowAlertsEndTimeUTC
                    pauseAlertsUntil
                }
                theme {
                    id
                    name
                }
                timezone
            }
        }
    ";

    private const string _userQuery4 =
        @"
        query UserQuery {
            user {
                id
                avatar {
                    ...testFragment4
                }
                ...testFragment
                username
                ...testFragment2
            }
        }

        fragment testFragment4 on File {
            id
            storeKey
        }

        fragment testFragment on User {
            createdAt,
            onlineStatus
        }

        fragment testFragment2 on User {
            personalInfo {
                email
                emailConfirmed
                firstName
                lastName
                ...testFragment3
                theme {
                    id
                    name
                }
                timezone
            }
        }

        fragment testFragment3 on UserInfo {
            userNotificationsPreferences {
                notifSound
                allowAlertsStartTimeUTC
                allowAlertsEndTimeUTC
                pauseAlertsUntil
            }
        }
    ";

    private const string _channelQuery1 =
        @"
        query ChannelQuery {
            channel {
                id
                allowThreads
                allowedPostersMask
                avatar {
                    id
                    storeKey
                }
                createdAtUTC
                createdBy
                description
                name
                numMembers
                private
                topic
                workspace
            }
        }
    ";

    private const string _channelQuery2 =
        @"
        query ChannelQuery {
            channel {
                id
                allowThreads
                allowedPostersMask
                avatar {
                    id
                    storeKey
                }
                createdAtUTC
                createdBy
                description
                members
                name
                numMembers
                private
                topic
                workspace
            }
        }
    ";

    private const string _channelQuery3 =
        @"
        query ChannelQuery {
            channel {
                id
                allowThreads
                allowedPostersMask
                avatar {
                    id
                    storeKey
                }
                createdAtUTC
                createdBy
                description
                messages
                name
                numMembers
                private
                topic
                workspace
            }
        }
    ";

    private const string _channelQuery4 =
        @"
        query ChannelQuery {
            channel {
                id
                allowThreads
                ...testFragment
                private
                topic
                workspace
            }
        }

        fragment testFragment on Channel {
            allowedPostersMask
            avatar {
                id
                storeKey
            }
            createdAtUTC
            createdBy
            description
            messages
            name
            numMembers
        }
    ";

    private const string _channelQuery5 =
        @"
        query ChannelQuery {
            channel {
                id
                allowThreads
                ...testFragment
                private
                topic
                workspace
            }
        }

        fragment testFragment on Channel {
            allowedPostersMask
            avatar {
                id
                storeKey
            }
            createdAtUTC
            createdBy
            description
            name
            numMembers
        }
    ";

    private const string _channelQuery6 =
        @"
        query ChannelsConnectionQuery {
            channels {
                edges {
                    pageInfo {
                        hasNextPage
                    }
                    node {
                        id
                        allowThreads
                        ...testFragment
                        private
                        topic
                        workspace
                    }
                }
            }
        }

        fragment testFragment on Channel {
            allowedPostersMask
            avatar {
                id
                storeKey
            }
            createdAtUTC
            createdBy
            description
            name
            numMembers
        }
    ";

    private const string dmgQuery1 =
        @"
            query DmgQuery {
                directMessageGroup {
                    id
                    createdAtUTC
                    name
                    workspace
                }
            }
        ";

    private const string dmgQuery2 =
        @"
            query DmgQuery {
                directMessageGroup {
                    ...testFragment
                }
            }

            fragment testFragment on DirectMessageGroup {
                id
                createdAtUTC
                name
                workspace
            }
        ";

    private const string dmgQuery3 =
        @"
            query DmgConnectionQuery {
                directMessageGroups {
                    totalEdges
                    pageInfo {
                        hasNextPage
                    }
                    edges {
                        node {
                            id
                            createdAtUTC
                            name
                            workspace
                        }
                    }
                }
            }
        ";

    private const string dmgQuery4 =
        @"
            query DmgConnectionQuery {
                directMessageGroups {
                    totalEdges
                    pageInfo {
                        hasNextPage
                    }
                    edges {
                        node {
                            id
                            ...testFragment
                        }
                    }
                }
            }

            fragment testFragment on DirectMessageGroup {
                createdAtUTC
                name
                workspace
            }
        ";

    private const string groupsQuery1 =
        @"
            query GroupsConnectionQuery {
                starred {
                    totalEdges
                    pageInfo {
                        hasNextPage
                    }
                    edges {
                        node {
                            id
                            createdAtUTC
                            workspace
                            name
                        }
                    }
                }
            }
        ";

    private const string groupsQuery2 =
        @"
            query GroupsConnectionQuery {
                starred {
                    ...testFragment
                }
            }

            fragment testFragment on StarredConnection {
                totalEdges
                pageInfo {
                    hasNextPage
                }
                edges {
                    cursor
                    ...testFragment2
                }
            }

            fragment testFragment2 on StarredConnectionEdge {
                node {
                    ...testFragment3
                }
            }

            fragment testFragment3 on Group {
                id
                createdAtUTC
                workspace
                name
            }
        ";

    [Theory]
    [InlineData(groupsQuery1)]
    [InlineData(groupsQuery2)]
    public void GroupsDbColumns_ShouldGetGroupDbColumns(string query)
    {
        List<string> expectedCols =
            new() { "Id", "CreatedAt", "WorkspaceId", "Name" };

        var docAst = Parser.Parse(query);
        var opDef = docAst.Definitions.First() as GraphQLOperationDefinition;
        var starredFieldDef =
            opDef!.SelectionSet.Selections.First() as GraphQLField;
        var nodeAst = GraphQLUtils.GetNodeASTFromConnectionAST(
            starredFieldDef!,
            docAst,
            "StarredConnection",
            "StarredConnectionEdge"
        );
        var cols = FieldAnalyzer.GroupDbColumns(nodeAst!, docAst!);
        cols.Sort();
        expectedCols.Sort();
        Assert.Equal(expectedCols, cols);
    }

    /*
    [Theory]
    [InlineData(_userQuery, new[] { "Email", "Id" })]
    [InlineData(_userQuery2)]
    [InlineData(_userQuery3)]
    [InlineData(_userQuery4)]
    public void UserDbColumns_ShouldGetUserDbColumns(
        string query,
        IEnumerable<string>? expectedCols = null
    )
    {
        expectedCols ??= new List<string>
        {
            "Id",
            "AvatarId",
            "CreatedAt",
            "OnlineStatus",
            "UserName",
            "Email",
            "EmailConfirmed",
            "FirstName",
            "LastName",
            "NotificationSound",
            "NotificationsAllowStartTime",
            "NotificationsAllowEndTime",
            "NotificationsPauseUntil",
            "ThemeId",
            "Timezone"
        };

        var docAst = Parser.Parse(query);
        var opDef = docAst.Definitions.First() as GraphQLOperationDefinition;
        var rootFieldDef =
            opDef!.SelectionSet.Selections.First() as GraphQLField;
        var cols = FieldAnalyzer.UserDbColumns(rootFieldDef!, docAst!);
        cols.Sort();
        var expected = expectedCols.ToList();
        expected.Sort();
        Assert.Equal(expected, cols);
    }

    [Theory]
    [InlineData(_workspacesQuery1)]
    [InlineData(_workspacesQuery2)]
    [InlineData(_workspacesQuery3)]
    [InlineData(_workspacesQuery4)]
    [InlineData(_workspacesQuery5)]
    [InlineData(_workspacesQuery6)]
    public void WorkspaceDbColumns_ShouldGetWorkspacesDbColumns(string query)
    {
        List<string> expectedCols =
            new()
            {
                "Id",
                "AvatarId",
                "CreatedAt",
                "Description",
                "Name",
                "NumMembers"
            };

        var docAst = Parser.Parse(query);
        var opDef = docAst.Definitions.First() as GraphQLOperationDefinition;
        var workspacesFieldDef =
            opDef!.SelectionSet.Selections.First() as GraphQLField;
        var nodeAst = GraphQLUtils.GetNodeASTFromConnectionAST(
            workspacesFieldDef!,
            docAst,
            "WorkspacesConnection",
            "WorkspacesConnectionEdge"
        );
        var dbColumns = FieldAnalyzer.WorkspaceDbColumns(nodeAst, docAst);
        dbColumns.Sort();
        expectedCols.Sort();
        Assert.Equal(expectedCols, dbColumns);
    }

    [Theory]
    [InlineData(dmgQuery1)]
    [InlineData(dmgQuery2)]
    [InlineData(dmgQuery3)]
    [InlineData(dmgQuery4)]
    public void DirectMessageGroupDbColumns_ShouldGetDmgDbColumns(string query)
    {
        List<string> expectedCols = new() { "Id", "CreatedAt", "WorkspaceId" };
        var docAst = Parser.Parse(query);
        var opDef = docAst.Definitions.First() as GraphQLOperationDefinition;
        if (query == dmgQuery3 || query == dmgQuery4)
        {
            var dmgsNodeAst = GraphQLUtils.GetNodeASTFromConnectionAST(
                (opDef!.SelectionSet.Selections.First() as GraphQLField)!,
                docAst,
                "DirectMessageGroupsConnection",
                "DirectMessageGroupsConnectionEdge"
            );
            var cols = FieldAnalyzer.DirectMessageGroupDbColumns(
                dmgsNodeAst,
                docAst
            );
            cols.Sort();
            expectedCols.Sort();
            Assert.Equal(expectedCols, cols);
            return;
        }

        var dmgFieldAst = (
            opDef!.SelectionSet.Selections.First() as GraphQLField
        )!;
        var dbCols = FieldAnalyzer.DirectMessageGroupDbColumns(
            dmgFieldAst,
            docAst
        );
        dbCols.Sort();
        expectedCols.Sort();
        Assert.Equal(expectedCols, dbCols);
    }

    [Theory]
    [InlineData(_channelQuery1)]
    [InlineData(_channelQuery2)]
    [InlineData(_channelQuery3)]
    [InlineData(_channelQuery4)]
    [InlineData(_channelQuery5)]
    [InlineData(_channelQuery6)]
    public void ChannelDbColumns_ShouldGetChannelsDbColumns(string query)
    {
        List<string> expectedCols =
            new()
            {
                "Id",
                "AllowThreads",
                "AvatarId",
                "AllowedPostersMask",
                "CreatedAt",
                "CreatedById",
                "Description",
                "Name",
                "NumMembers",
                "Private",
                "Topic",
                "WorkspaceId"
            };
        var docAst = Parser.Parse(query);
        var opDef = docAst.Definitions.First() as GraphQLOperationDefinition;
        if (query == _channelQuery6)
        {
            var channelsNodeAst = GraphQLUtils.GetNodeASTFromConnectionAST(
                (opDef!.SelectionSet.Selections.First() as GraphQLField)!,
                docAst,
                "ChannelsConnection",
                "ChannelsConnectionEdgeType"
            );
            var cols = FieldAnalyzer.ChannelDbColumns(channelsNodeAst!, docAst);
            expectedCols.Sort();
            cols.Sort();
            Assert.Equal(expectedCols, cols);
            return;
        }

        var channelsFieldDef =
            opDef!.SelectionSet.Selections.First() as GraphQLField;
        if (
            query == _channelQuery2
            || query == _channelQuery3
            || query == _channelQuery4
        )
        {
            Assert.Throws<InvalidOperationException>(
                () => FieldAnalyzer.ChannelDbColumns(channelsFieldDef!, docAst)
            );
            return;
        }

        var dbCols = FieldAnalyzer.ChannelDbColumns(channelsFieldDef!, docAst);
        expectedCols.Sort();
        dbCols.Sort();
        Assert.Equal(expectedCols, dbCols);
    }
    */

    /**
    
        [Fact]
        public void GetFragments_ShouldGetFragments()
        {
            Assert.Equal(0, FieldAnalyzer.GetFragments(_validBasicQuery).Count());
            Assert.Equal(
                new Dictionary<string, string>
                {
                    { "testFragment1", _testFragment1Contents },
                    { "testFragment2", _testFragment2Contents },
                },
                FieldAnalyzer.GetFragments(_validBasicQueryWithFragments)
            );
        }
    
        [Fact]
        public void Workspaces_ShouldGetWorkspacesConnectionFieldInfo()
        {
            FieldInfo testWorkspacesFieldInfo =
                new()
                {
                    SubfieldNames = new List<string>
                    {
                        "pageInfo",
                        "edges",
                        "hasNextPage",
                        "node",
                        "id"
                    }
                };
            FieldTree workspacesFieldTree = new("workspaces(arg: 'testArg')");
            FieldTree pageInfoFieldTree = new("pageInfo");
            FieldTree hasNextPageFieldTree = new("hasNextPage");
            FieldTree edgesFieldTree = new("edges");
            FieldTree nodeFieldTree = new("node");
            FieldTree idFieldTree = new("id");
            testWorkspacesFieldInfo.FieldTree = workspacesFieldTree;
            workspacesFieldTree.Children.Add(pageInfoFieldTree);
            workspacesFieldTree.Children.Add(edgesFieldTree);
            pageInfoFieldTree.Children.Add(hasNextPageFieldTree);
            edgesFieldTree.Children.Add(nodeFieldTree);
            nodeFieldTree.Children.Add(idFieldTree);
    
            var fragments = new Dictionary<string, string>();
            Assert.ThrowsAny<Exception>(
                () => FieldAnalyzer.Workspaces(_validBasicQuery, fragments)
            );
            Assert.True(
                EquivalentFieldInfo(
                    testWorkspacesFieldInfo,
                    FieldAnalyzer.Workspaces(_workspacesQuery, fragments)
                )
            );
            fragments["edgeFragment"] = _edgeFragmentContents;
            Assert.True(
                EquivalentFieldInfo(
                    testWorkspacesFieldInfo,
                    FieldAnalyzer.Workspaces(
                        _workspacesQueryWithFragments,
                        fragments
                    )
                )
            );
        }
    
        [Fact]
        public void User_ShouldGetUserFieldInfo()
        {
            FieldInfo testUserFieldInfo =
                new()
                {
                    SubfieldNames = new List<string>
                    {
                        "id",
                        "personalInfo",
                        "email"
                    }
                };
            FieldTree userFieldTree = new("user(arg: 'testArg')");
            FieldTree idFieldTree = new("id");
            FieldTree personalInfoFieldTree = new("personalInfo");
            FieldTree emailFieldTree = new("email");
            testUserFieldInfo.FieldTree = userFieldTree;
            userFieldTree.Children.Add(idFieldTree);
            userFieldTree.Children.Add(personalInfoFieldTree);
            personalInfoFieldTree.Children.Add(emailFieldTree);
    
            var fragments = new Dictionary<string, string>();
            Assert.ThrowsAny<Exception>(
                () => FieldAnalyzer.User(_validBasicQuery, fragments)
            );
            Assert.True(
                EquivalentFieldInfo(
                    testUserFieldInfo,
                    FieldAnalyzer.User(_userQuery, fragments)
                )
            );
            fragments["personalInfoFragment"] = _personalInfoFragmentContexts;
            Assert.True(
                EquivalentFieldInfo(
                    testUserFieldInfo,
                    FieldAnalyzer.User(_userQueryWithFragments, fragments)
                )
            );
        }
    
        [Fact]
        public void WorkspaceMembers_ShouldGetWorkspaceMemberConnectionFieldInfo()
        {
            FieldInfo testWorkspaceMembersFieldInfo =
                new()
                {
                    SubfieldNames = new List<string>
                    {
                        "pageInfo",
                        "edges",
                        "hasNextPage",
                        "node",
                        "id"
                    }
                };
            FieldTree workspaceMembersFieldTree = new("members(arg: 'testArg')");
            FieldTree pageInfoFieldTree = new("pageInfo");
            FieldTree hasNextPageFieldTree = new("hasNextPage");
            FieldTree edgesFieldTree = new("edges");
            FieldTree nodeFieldTree = new("node");
            FieldTree idFieldTree = new("id");
            testWorkspaceMembersFieldInfo.FieldTree = workspaceMembersFieldTree;
            workspaceMembersFieldTree.Children.Add(pageInfoFieldTree);
            workspaceMembersFieldTree.Children.Add(edgesFieldTree);
            pageInfoFieldTree.Children.Add(hasNextPageFieldTree);
            edgesFieldTree.Children.Add(nodeFieldTree);
            nodeFieldTree.Children.Add(idFieldTree);
    
            var fragments = new Dictionary<string, string>();
            Assert.ThrowsAny<Exception>(
                () => FieldAnalyzer.Workspaces(_validBasicQuery, fragments)
            );
            Assert.True(
                EquivalentFieldInfo(
                    testWorkspaceMembersFieldInfo,
                    FieldAnalyzer.WorkspaceMembers(
                        _workspaceMembersQuery,
                        fragments
                    )
                )
            );
            fragments["edgeFragment"] = _edgeFragmentContents;
            Assert.True(
                EquivalentFieldInfo(
                    testWorkspaceMembersFieldInfo,
                    FieldAnalyzer.WorkspaceMembers(
                        _workspaceMembersQueryWithFragments,
                        fragments
                    )
                )
            );
        }
    
        [Fact]
        public void ExtractUserFields_ShouldGetSubfieldsOfUserFieldWithPassedName()
        {
            FieldTree rootFieldTree = new("root");
            FieldTree userFieldTree = new("user(arg: 'testArg')");
            FieldTree idFieldTree = new("id");
            FieldTree personalInfoFieldTree = new("personalInfo");
            FieldTree emailFieldTree = new("email");
            rootFieldTree.Children.Add(userFieldTree);
            userFieldTree.Children.Add(idFieldTree);
            userFieldTree.Children.Add(personalInfoFieldTree);
            personalInfoFieldTree.Children.Add(emailFieldTree);
    
            Assert.Equal(
                new List<string> { "id", "personalInfo", "email" },
                FieldAnalyzer.ExtractUserFields("user", rootFieldTree)
            );
        }
    
        private static bool EquivalentFieldInfo(
            FieldInfo expected,
            FieldInfo actual
        )
        {
            if (actual.SubfieldNames.Count != expected.SubfieldNames.Count)
            {
                return false;
            }
            expected.SubfieldNames.Sort();
            actual.SubfieldNames.Sort();
            foreach (var (e, a) in expected.SubfieldNames.Zip(actual.SubfieldNames))
            {
                if (e != a)
                    return false;
            }
    
            return EquivalentFieldTree(expected.FieldTree, actual.FieldTree);
        }
    
        private static int FieldTreeComparator(FieldTree a, FieldTree b) =>
            a.FieldName.CompareTo(b.FieldName);
    
        private static bool EquivalentFieldTree(
            FieldTree expected,
            FieldTree actual
        )
        {
            if (expected.FieldName != actual.FieldName)
                return false;
            if (expected.Children.Count != actual.Children.Count)
                return false;
            expected.Children.Sort(FieldTreeComparator);
            actual.Children.Sort(FieldTreeComparator);
            foreach (var (e, a) in expected.Children.Zip(actual.Children))
            {
                if (!EquivalentFieldTree(e, a))
                {
                    return false;
                }
            }
    
            return true;
        }
    **/
}

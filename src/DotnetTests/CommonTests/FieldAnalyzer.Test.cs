using System.Collections.Immutable;
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
    private readonly string _validBasicQuery =
        @"
        query BasicQuery {
            testObj {
                testField
            }
        }
    ";

    private readonly string _validBasicQueryWithFragments =
        @"
        query BasicQuery {
            testObj {
                testField
                ...testFragment1
            }
            ...testFragment2
        }

        fragment testFragment1 on BasicQuery {
            testField2
        }

        fragment testFragment2 on TestObj {
            testField3
        }
    ";

    private readonly string _testFragment1Contents =
        @"{
            testField2
        }";

    private readonly string _testFragment2Contents =
        @"{
            testField3
        }";

    private readonly string _invalidBasicQuery =
        @"
        badOp BasicQuery {
            testObj {
                testField
            }
        }
    ";

    private readonly string _validBasicQueryWithVars =
        @"
        query BasicQuery {
            testObj(arg: 'testArg') {
                testField
            }
        }
    ";

    private readonly string _emptyQuery =
        @"
        query";

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

    private readonly string _userQueryWithFragments =
        @"
        query UserQuery {
            user(arg: 'testArg') {
                id
                personalInfo {
                    ...personalInfoFragment
                }
            }
        }

        fragment personalInfoFragment on User {
            email
        }
    ";

    private readonly string _workspaceMembersQuery =
        @"
        query WorkspaceMembersQuery {
            workspace {
                members(arg: 'testArg') {
                    pageInfo {
                        hasNextPage
                    }
                    edges {
                        node {
                            id
                        }
                    } 
                }
            }
        }
    ";

    private readonly string _workspaceMembersQueryWithFragments =
        @"
        query WorkspaceMembersQuery {
            workspace {
                members(arg: 'testArg') {
                    pageInfo {
                        hasNextPage
                    }
                    edges {
                        ...edgeFragment
                    } 
                }
            }
        }

        fragment edgeFragment on WorkspacesEdge {
            node {
                id
            }
        }
    ";

    private readonly string _edgeFragmentContents =
        @"{
            node {
                id
            }
        }";

    private readonly string _personalInfoFragmentContexts =
        @"{
            email
        }";

    /**
    
        [Fact]
        public void GetQueryName_ShouldGetQueryName()
        {
            Assert.Null(FieldAnalyzer.GetQueryName(null));
            Assert.Throws<InvalidOperationException>(
                () => FieldAnalyzer.GetQueryName(_invalidBasicQuery)
            );
            Assert.ThrowsAny<Exception>(
                () => FieldAnalyzer.GetQueryName(_emptyQuery)
            );
            Assert.Equal(
                "BasicQuery",
                FieldAnalyzer.GetQueryName(_validBasicQuery)
            );
            Assert.Equal(
                "BasicQuery",
                FieldAnalyzer.GetQueryName(_validBasicQueryWithVars)
            );
        }
    **/

    /**
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
    **/

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

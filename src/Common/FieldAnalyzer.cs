using System.Text.RegularExpressions;
using GraphQL.Types;
using GraphQL.Validation;
using GraphQLParser.AST;

namespace Common.SlackCloneGraphQL;

/// <summary>
/// This class parses individual fields of a valid GraphQL document to
/// determine which subfields of an individual field were queried for by
/// the client. This extra bit of analysis helps optimize database
/// queries; knowing which fields were queried for by the client allows EF Core
/// queries to the database to avoid unnecessary joins, and select only necessary
/// columns when performing multi-row load operations.
///
/// Getting this information solely through the IResolveFieldContext API
/// provided by GraphQL.NET is either not possible or harder than simply
/// writing some document parsing logic myself.
/// </summary>
public static class FieldAnalyzer
{
    public static string? GetQueryName(string? query)
    {
        if (query is null)
        {
            return null;
        }
        int queryStart = query.IndexOf('q');
        if (queryStart == -1 || query[queryStart..(queryStart + 5)] != "query")
        {
            throw new InvalidOperationException();
        }

        int start = queryStart + 6;
        int parenIdx = query.IndexOf('(', start);
        int bracketIdx = query.IndexOf('{', start);
        if (parenIdx == -1 && bracketIdx == -1)
        {
            throw new InvalidOperationException();
        }
        int stop = Math.Min(
            parenIdx == -1 ? int.MaxValue : parenIdx,
            bracketIdx == -1 ? int.MaxValue : bracketIdx
        );
        return query[start..stop].Trim();
    }

    public static Dictionary<string, string> GetFragments(string document)
    {
        Dictionary<string, string> fragments = new();
        int i = document.IndexOf("fragment");
        int k;
        bool done = i == -1;
        int fragmentKeywordLength = 9;
        while (!done)
        {
            i += fragmentKeywordLength;
            int fragmentNameStop = document.IndexOf(" on ", i);
            string fragmentName = document[i..fragmentNameStop];
            int fragmentOpeningIdx = document.IndexOf('{', fragmentNameStop);
            k = GetMatchingClosingParenIdx(document, fragmentOpeningIdx);
            fragments[fragmentName] = document[fragmentOpeningIdx..(k + 1)];
            i = document.IndexOf("fragment", k);
            done = i == -1;
        }

        return fragments;
    }

    private static int GetMatchingClosingParenIdx(string s, int openingIdx)
    {
        int opening = 1;
        int i = openingIdx + 1;
        while (opening > 0)
        {
            char c = s[i++];
            if (c == '{')
            {
                opening++;
            }
            else if (c == '}')
            {
                opening--;
            }
        }
        return i - 1;
    }

    /*
    public static FieldInfo ChannelMessages(
        string opString,
        Dictionary<string, string> fragments,
        string? queryName = null
    )
    {
        string channelMessagesFieldSlice = GetFieldSlice(
            opString,
            "messages",
            queryName == "ChannelMessagesListPaginationQuery" ? null : "channel"
        );
        return CollectFields(channelMessagesFieldSlice, fragments);
    }

    public static FieldInfo DirectMessages(
        string opString,
        Dictionary<string, string> fragments
    )
    {
        string directMessagesFieldSlice = GetFieldSlice(opString, "messages");
        return CollectFields(directMessagesFieldSlice, fragments);
    }

    public static FieldInfo ChannelMembers(
        string opString,
        Dictionary<string, string> fragments
    )
    {
        string channelMembersFieldSlice = GetFieldSlice(
            opString,
            "members",
            "channel"
        );
        return CollectFields(channelMembersFieldSlice, fragments);
    }

    public static FieldInfo Workspaces(
        string opString,
        Dictionary<string, string> fragments
    )
    {
        string workspacesFieldSlice = GetFieldSlice(opString, "workspaces");
        return CollectFields(workspacesFieldSlice, fragments);
    }

    public static FieldInfo Channels(
        string opString,
        Dictionary<string, string> fragments
    )
    {
        string channelsFieldSlice = GetFieldSlice(opString, "channels(");
        return CollectFields(channelsFieldSlice, fragments);
    }

    public static FieldInfo DirectMessageGroups(
        string opString,
        Dictionary<string, string> fragments
    )
    {
        string directMessageGroupsFieldSlice = GetFieldSlice(
            opString,
            "directMessageGroups("
        );
        return CollectFields(directMessageGroupsFieldSlice, fragments);
    }

    public static FieldInfo Starred(
        string opString,
        Dictionary<string, string> fragments
    )
    {
        string starredFieldSlice = GetFieldSlice(opString, "starred(");
        return CollectFields(starredFieldSlice, fragments);
    }

    public static FieldInfo User(
        string opString,
        Dictionary<string, string> fragments
    )
    {
        string userFieldSlice = GetFieldSlice(opString, "user");
        return CollectFields(userFieldSlice, fragments);
    }

    */

    public static List<string> WorkspaceMemberDbColumns(
        GraphQLField workspaceMemberFieldAst,
        GraphQLDocument document
    )
    {
        if (workspaceMemberFieldAst.SelectionSet is null)
        {
            throw new InvalidOperationException(
                "null selection set on workspace member"
            );
        }

        var subfields = workspaceMemberFieldAst.SelectionSet.Selections
            .Where(s => s.Kind == ASTNodeKind.Field)
            .Select(s => (s as GraphQLField)!.Name.StringValue)
            .ToList();
        var fragmentSpreads = workspaceMemberFieldAst.SelectionSet.Selections
            .Where(s => s.Kind == ASTNodeKind.FragmentSpread)
            .Select(
                s => (s as GraphQLFragmentSpread)!.FragmentName.Name.StringValue
            )
            .ToList();

        GraphQLField? memberInfoFieldAst = (GraphQLField?)
            workspaceMemberFieldAst.SelectionSet.Selections.FirstOrDefault(
                s =>
                    s.Kind == ASTNodeKind.Field
                    && (s as GraphQLField)!.Name.StringValue
                        == "workspaceMemberInfo"
            );

        if (fragmentSpreads.Any())
        {
            var fragDefs = document.Definitions.Where(
                d =>
                    d.Kind == ASTNodeKind.FragmentDefinition
                    && fragmentSpreads.Contains(
                        (d as GraphQLFragmentDefinition)!
                            .FragmentName
                            .Name
                            .StringValue
                    )
            );
            subfields.AddRange(
                fragDefs.SelectMany(
                    f =>
                        (
                            f as GraphQLFragmentDefinition
                        )!.SelectionSet.Selections
                            .Where(s => s.Kind == ASTNodeKind.Field)
                            .Select(s => (s as GraphQLField)!.Name.StringValue)
                )
            );

            if (memberInfoFieldAst is null)
            {
                memberInfoFieldAst = (GraphQLField?)(
                    (
                        (GraphQLFragmentDefinition?)
                            fragDefs.FirstOrDefault(
                                f =>
                                    (
                                        f as GraphQLFragmentDefinition
                                    )!.SelectionSet.Selections.Any(
                                        s =>
                                            s.Kind == ASTNodeKind.Field
                                            && (s as GraphQLField)!
                                                .Name
                                                .StringValue
                                                == "workspaceMemberInfo"
                                    )
                            )
                    )?.SelectionSet!.Selections.First(
                        s =>
                            s.Kind == ASTNodeKind.Field
                            && (s as GraphQLField)!.Name.StringValue
                                == "workspaceMemberInfo"
                    )
                );
            }
        }

        if (subfields.Contains("workspaceMemberInfo"))
        {
            if (memberInfoFieldAst is null)
            {
                throw new InvalidOperationException(
                    "workspaceMemberInfo subfield requested but didn't find its AST"
                );
            }

            subfields.Remove("workspaceMemberInfo");
            subfields.AddRange(
                WorkspaceMemberDbColumnsFromMemberInfo(
                    memberInfoFieldAst,
                    document
                )
            );
        }

        return subfields
            .Select(s => s[0].ToString().ToUpper() + s[1..])
            .Select(s => s == "Avatar" ? "AvatarId" : s)
            .Select(s => s == "User" ? "UserId" : s)
            .Select(s => s == "Workspace" ? "WorkspaceId" : s)
            .Where(s => s != "WorkspaceMemberInfo")
            .ToList();
    }

    private static List<string> WorkspaceMemberDbColumnsFromMemberInfo(
        GraphQLField memberInfoFieldAst,
        GraphQLDocument document
    )
    {
        if (memberInfoFieldAst.SelectionSet is null)
        {
            throw new InvalidOperationException(
                "null selection set on group ast node"
            );
        }

        var subfields = memberInfoFieldAst.SelectionSet.Selections
            .Where(s => s.Kind == ASTNodeKind.Field)
            .Select(s => (s as GraphQLField)!.Name.StringValue)
            .ToList();
        var fragmentSpreads = memberInfoFieldAst.SelectionSet.Selections
            .Where(s => s.Kind == ASTNodeKind.FragmentSpread)
            .Select(
                s => (s as GraphQLFragmentSpread)!.FragmentName.Name.StringValue
            )
            .ToList();

        if (fragmentSpreads.Any())
        {
            var fragDefs = document.Definitions.Where(
                d =>
                    d.Kind == ASTNodeKind.FragmentDefinition
                    && fragmentSpreads.Contains(
                        (d as GraphQLFragmentDefinition)!
                            .FragmentName
                            .Name
                            .StringValue
                    )
            );
            subfields.AddRange(
                fragDefs.SelectMany(
                    f =>
                        (
                            f as GraphQLFragmentDefinition
                        )!.SelectionSet.Selections
                            .Where(s => s.Kind == ASTNodeKind.Field)
                            .Select(s => (s as GraphQLField)!.Name.StringValue)
                )
            );
        }

        return subfields
            .Select(s => s[0].ToString().ToUpper() + s[1..])
            .Select(
                s =>
                    s == "WorkspaceAdminPermissions"
                        ? "WorkspaceAdminPermissionsId"
                        : s
            )
            .Select(s => s == "Theme" ? "ThemeId" : s)
            .Select(s => s == "NotifSound" ? "NotificationSound" : s)
            .Select(
                s =>
                    s == "NotificationsAllowTimeStartUTC"
                        ? "NotificationsAllowTimeStart"
                        : s
            )
            .Select(
                s =>
                    s == "NotificationsAllowTimeEndUTC"
                        ? "NotificationsAllowTimeEnd"
                        : s
            )
            .Select(s => s == "OnlineStatusUntilUTC" ? "OnlineStatusUntil" : s)
            .ToList();
    }

    public static List<string> ChannelMessageDbColumns(
        GraphQLField channelMessageFieldAst,
        GraphQLDocument document
    )
    {
        if (channelMessageFieldAst.SelectionSet is null)
        {
            throw new InvalidOperationException(
                "null selection set on channel message"
            );
        }

        var subfields = channelMessageFieldAst.SelectionSet.Selections
            .Where(s => s.Kind == ASTNodeKind.Field)
            .Select(s => (s as GraphQLField)!.Name.StringValue)
            .ToList();
        var fragmentSpreads = channelMessageFieldAst.SelectionSet.Selections
            .Where(s => s.Kind == ASTNodeKind.FragmentSpread)
            .Select(
                s => (s as GraphQLFragmentSpread)!.FragmentName.Name.StringValue
            )
            .ToList();

        if (fragmentSpreads.Any())
        {
            var fragDefs = document.Definitions.Where(
                d =>
                    d.Kind == ASTNodeKind.FragmentDefinition
                    && fragmentSpreads.Contains(
                        (d as GraphQLFragmentDefinition)!
                            .FragmentName
                            .Name
                            .StringValue
                    )
            );
            subfields.AddRange(
                fragDefs.SelectMany(
                    f =>
                        (
                            f as GraphQLFragmentDefinition
                        )!.SelectionSet.Selections
                            .Where(s => s.Kind == ASTNodeKind.Field)
                            .Select(s => (s as GraphQLField)!.Name.StringValue)
                )
            );
        }

        return subfields
            .Select(s => s[0].ToString().ToUpper() + s[1..])
            .Select(s => s == "User" ? "UserId" : s)
            .Select(s => s == "CreatedAtUTC" ? "CreatedAt" : s)
            .Select(s => s == "LastEditUTC" ? "LastEdit" : s)
            .Select(s => s == "Group" ? "ChannelId" : s)
            .Select(s => s == "LaterFlag" ? "LaterFlagId" : s)
            .Select(s => s == "SentAtUTC" ? "SentAt" : s)
            .Where(s => s != "Draft" && s != "Type")
            .ToList();
    }

    public static List<string> ChannelMemberDbColumns(
        GraphQLField channelMemberFieldAst,
        GraphQLDocument document
    )
    {
        if (channelMemberFieldAst.SelectionSet is null)
        {
            throw new InvalidOperationException(
                "null selection set on channel member"
            );
        }

        GraphQLField? memberInfoFieldAst = (GraphQLField?)
            channelMemberFieldAst.SelectionSet.Selections.FirstOrDefault(
                s =>
                    s.Kind == ASTNodeKind.Field
                    && (s as GraphQLField)!.Name.StringValue == "memberInfo"
            );

        var subfields = channelMemberFieldAst.SelectionSet.Selections
            .Where(s => s.Kind == ASTNodeKind.Field)
            .Select(s => (s as GraphQLField)!.Name.StringValue)
            .ToList();
        var fragmentSpreads = channelMemberFieldAst.SelectionSet.Selections
            .Where(s => s.Kind == ASTNodeKind.FragmentSpread)
            .Select(
                s => (s as GraphQLFragmentSpread)!.FragmentName.Name.StringValue
            )
            .ToList();

        if (fragmentSpreads.Any())
        {
            var fragDefs = document.Definitions.Where(
                d =>
                    d.Kind == ASTNodeKind.FragmentDefinition
                    && fragmentSpreads.Contains(
                        (d as GraphQLFragmentDefinition)!
                            .FragmentName
                            .Name
                            .StringValue
                    )
            );

            if (memberInfoFieldAst is null)
            {
                memberInfoFieldAst = (GraphQLField?)(
                    (
                        (GraphQLFragmentDefinition?)
                            fragDefs.FirstOrDefault(
                                f =>
                                    (
                                        f as GraphQLFragmentDefinition
                                    )!.SelectionSet.Selections.Any(
                                        s =>
                                            s.Kind == ASTNodeKind.Field
                                            && (s as GraphQLField)!
                                                .Name
                                                .StringValue == "memberInfo"
                                    )
                            )
                    )?.SelectionSet!.Selections.First(
                        s =>
                            s.Kind == ASTNodeKind.Field
                            && (s as GraphQLField)!.Name.StringValue
                                == "memberInfo"
                    )
                );
            }

            subfields.AddRange(
                fragDefs.SelectMany(
                    f =>
                        (
                            f as GraphQLFragmentDefinition
                        )!.SelectionSet.Selections
                            .Where(s => s.Kind == ASTNodeKind.Field)
                            .Select(s => (s as GraphQLField)!.Name.StringValue)
                )
            );
        }

        if (subfields.Contains("memberInfo"))
        {
            if (memberInfoFieldAst is null)
            {
                throw new InvalidOperationException(
                    "memberInfo subfield requested but didn't find its AST"
                );
            }

            subfields.Remove("memberInfo");
            subfields.AddRange(
                ChannelMemberDbColumnsFromMemberInfo(
                    memberInfoFieldAst,
                    document
                )
            );
        }

        return subfields
            .Select(s => s == "user" ? "UserId" : s)
            .Select(s => s[0].ToString().ToUpper() + s[1..])
            .ToList();
    }

    private static List<string> ChannelMemberDbColumnsFromMemberInfo(
        GraphQLField memberInfoFieldAst,
        GraphQLDocument document
    )
    {
        if (memberInfoFieldAst.SelectionSet is null)
        {
            throw new InvalidOperationException(
                "null selection set on group ast node"
            );
        }

        var subfields = memberInfoFieldAst.SelectionSet.Selections
            .Where(s => s.Kind == ASTNodeKind.Field)
            .Select(s => (s as GraphQLField)!.Name.StringValue)
            .ToList();
        var fragmentSpreads = memberInfoFieldAst.SelectionSet.Selections
            .Where(s => s.Kind == ASTNodeKind.FragmentSpread)
            .Select(
                s => (s as GraphQLFragmentSpread)!.FragmentName.Name.StringValue
            )
            .ToList();

        if (fragmentSpreads.Any())
        {
            var fragDefs = document.Definitions.Where(
                d =>
                    d.Kind == ASTNodeKind.FragmentDefinition
                    && fragmentSpreads.Contains(
                        (d as GraphQLFragmentDefinition)!
                            .FragmentName
                            .Name
                            .StringValue
                    )
            );
            subfields.AddRange(
                fragDefs.SelectMany(
                    f =>
                        (
                            f as GraphQLFragmentDefinition
                        )!.SelectionSet.Selections
                            .Where(s => s.Kind == ASTNodeKind.Field)
                            .Select(s => (s as GraphQLField)!.Name.StringValue)
                )
            );
        }

        return subfields
            .Select(s => s[0].ToString().ToUpper() + s[1..])
            .ToList();
    }

    public static List<string> GroupDbColumns(
        GraphQLField groupFieldAst,
        GraphQLDocument document
    )
    {
        if (groupFieldAst.SelectionSet is null)
        {
            throw new InvalidOperationException(
                "null selection set on group ast node"
            );
        }

        var subfields = groupFieldAst.SelectionSet.Selections
            .Where(s => s.Kind == ASTNodeKind.Field)
            .Select(s => (s as GraphQLField)!.Name.StringValue)
            .ToList();
        var fragmentSpreads = groupFieldAst.SelectionSet.Selections
            .Where(s => s.Kind == ASTNodeKind.FragmentSpread)
            .Select(
                s => (s as GraphQLFragmentSpread)!.FragmentName.Name.StringValue
            )
            .ToList();

        if (fragmentSpreads.Any())
        {
            var fragDefs = document.Definitions.Where(
                d =>
                    d.Kind == ASTNodeKind.FragmentDefinition
                    && fragmentSpreads.Contains(
                        (d as GraphQLFragmentDefinition)!
                            .FragmentName
                            .Name
                            .StringValue
                    )
            );
            subfields.AddRange(
                fragDefs.SelectMany(
                    f =>
                        (
                            f as GraphQLFragmentDefinition
                        )!.SelectionSet.Selections
                            .Where(s => s.Kind == ASTNodeKind.Field)
                            .Select(s => (s as GraphQLField)!.Name.StringValue)
                )
            );
        }

        return subfields
            .Select(s => s == "createdAtUTC" ? "createdAt" : s)
            .Select(s => s == "workspace" ? "workspaceId" : s)
            .Select(s => s[0].ToString().ToUpper() + s[1..])
            .ToList();
    }

    public static List<string> DirectMessageGroupDbColumns(
        GraphQLField dmgFieldAst,
        GraphQLDocument document
    )
    {
        if (dmgFieldAst.SelectionSet is null)
        {
            throw new InvalidOperationException(
                "null selection set on dmg ast node"
            );
        }

        var subfields = dmgFieldAst.SelectionSet.Selections
            .Where(s => s.Kind == ASTNodeKind.Field)
            .Select(s => (s as GraphQLField)!.Name.StringValue)
            .ToList();
        var fragmentSpreads = dmgFieldAst.SelectionSet.Selections
            .Where(s => s.Kind == ASTNodeKind.FragmentSpread)
            .Select(
                s => (s as GraphQLFragmentSpread)!.FragmentName.Name.StringValue
            )
            .ToList();
        if (fragmentSpreads.Any())
        {
            var fragDefs = document.Definitions.Where(
                d =>
                    d.Kind == ASTNodeKind.FragmentDefinition
                    && fragmentSpreads.Contains(
                        (d as GraphQLFragmentDefinition)!
                            .FragmentName
                            .Name
                            .StringValue
                    )
            );
            subfields.AddRange(
                fragDefs.SelectMany(
                    f =>
                        (
                            f as GraphQLFragmentDefinition
                        )!.SelectionSet.Selections
                            .Where(s => s.Kind == ASTNodeKind.Field)
                            .Select(s => (s as GraphQLField)!.Name.StringValue)
                )
            );
        }

        if (subfields.Contains("members") || subfields.Contains("messages"))
        {
            throw new NotImplementedException();
        }
        return subfields
            .Where(s => s != "name")
            .Select(s => s == "createdAtUTC" ? "createdAt" : s)
            .Select(s => s == "workspace" ? "workspaceId" : s)
            .Select(s => s[0].ToString().ToUpper() + s[1..])
            .ToList();
    }

    public static List<string> ChannelDbColumns(
        GraphQLField channelFieldAst,
        GraphQLDocument document
    )
    {
        if (channelFieldAst.SelectionSet is null)
        {
            throw new InvalidOperationException(
                "null selection set on channel ast node"
            );
        }

        var subfields = channelFieldAst.SelectionSet.Selections
            .Where(s => s.Kind == ASTNodeKind.Field)
            .Select(s => (s as GraphQLField)!.Name.StringValue)
            .ToList();
        var fragmentSpreads = channelFieldAst.SelectionSet.Selections
            .Where(s => s.Kind == ASTNodeKind.FragmentSpread)
            .Select(
                s => (s as GraphQLFragmentSpread)!.FragmentName.Name.StringValue
            )
            .ToList();
        if (fragmentSpreads.Any())
        {
            var fragDefs = document.Definitions.Where(
                d =>
                    d.Kind == ASTNodeKind.FragmentDefinition
                    && fragmentSpreads.Contains(
                        (d as GraphQLFragmentDefinition)!
                            .FragmentName
                            .Name
                            .StringValue
                    )
            );
            subfields.AddRange(
                fragDefs.SelectMany(
                    f =>
                        (
                            f as GraphQLFragmentDefinition
                        )!.SelectionSet.Selections
                            .Where(s => s.Kind == ASTNodeKind.Field)
                            .Select(s => (s as GraphQLField)!.Name.StringValue)
                )
            );
        }

        if (subfields.Contains("members") || subfields.Contains("messages"))
        {
            throw new InvalidOperationException(
                "members, messages not channel db columns"
            );
        }

        return subfields
            .Select(s => s.First().ToString().ToUpper() + s[1..])
            .Select(s => s == "Avatar" ? "AvatarId" : s)
            .Select(s => s == "CreatedBy" ? "CreatedById" : s)
            .Select(s => s == "CreatedAtUTC" ? "CreatedAt" : s)
            .Select(s => s == "Workspace" ? "WorkspaceId" : s)
            .ToList();
    }

    public static List<string> WorkspaceDbColumns(
        GraphQLField workspaceFieldAst,
        GraphQLDocument document
    )
    {
        if (workspaceFieldAst.SelectionSet is null)
        {
            throw new InvalidOperationException(
                "null selection set on workspace ast node"
            );
        }

        var subfields = workspaceFieldAst.SelectionSet.Selections
            .Where(s => s.Kind == ASTNodeKind.Field)
            .Select(s => (s as GraphQLField)!.Name.StringValue)
            .ToList();
        var fragmentSpreads = workspaceFieldAst.SelectionSet.Selections
            .Where(s => s.Kind == ASTNodeKind.FragmentSpread)
            .Select(
                s => (s as GraphQLFragmentSpread)!.FragmentName.Name.StringValue
            );
        if (fragmentSpreads.Any())
        {
            var fragDefs = document.Definitions.Where(
                d =>
                    d.Kind == ASTNodeKind.FragmentDefinition
                    && fragmentSpreads.Contains(
                        (d as GraphQLFragmentDefinition)!
                            .FragmentName
                            .Name
                            .StringValue
                    )
            );
            subfields.AddRange(
                fragDefs.SelectMany(
                    f =>
                        (
                            f as GraphQLFragmentDefinition
                        )!.SelectionSet.Selections
                            .Where(s => s.Kind == ASTNodeKind.Field)
                            .Select(s => (s as GraphQLField)!.Name.StringValue)
                )
            );
        }
        if (subfields.Contains("members"))
        {
            throw new NotImplementedException();
        }

        return subfields
            .Select(s => s.First().ToString().ToUpper() + s[1..])
            .Select(s => s == "Avatar" ? "AvatarId" : s)
            .ToList();
    }

    public static List<string> UserDbColumns(
        GraphQLField userFieldAst,
        GraphQLDocument document
    )
    {
        if (userFieldAst.SelectionSet is null)
        {
            throw new InvalidOperationException("empty selection set");
        }

        var numInlineFragments = userFieldAst.SelectionSet.Selections
            .Where(s => s.Kind == ASTNodeKind.InlineFragment)
            .Count();
        if (numInlineFragments > 0)
        {
            throw new NotImplementedException(
                "inline fragment handling for User graphql type"
            );
        }

        return FieldsToDbColumns_User(
            GetSubfields_User(userFieldAst, document)
        );
    }

    private static List<string> GetSubfields_User(
        GraphQLField userFieldAst,
        GraphQLDocument document
    )
    {
        if (userFieldAst.SelectionSet is null)
        {
            throw new InvalidOperationException("empty selection set");
        }
        var subfields = userFieldAst.SelectionSet.Selections
            .Where(s => s.Kind == ASTNodeKind.Field)
            .Select(s => (s as GraphQLField)!.Name.StringValue)
            .ToList();
        if (subfields.Contains("personalInfo"))
        {
            subfields.Remove("personalInfo");
            GraphQLField personalInfoField = (GraphQLField)
                userFieldAst.SelectionSet.Selections
                    .Where(s => s.Kind == ASTNodeKind.Field)
                    .First(
                        s =>
                            (s as GraphQLField)!.Name.StringValue
                            == "personalInfo"
                    );
            foreach (
                var selection in personalInfoField.SelectionSet!.Selections
            )
            {
                if (selection.Kind == ASTNodeKind.Field)
                {
                    GraphQLField sel = (selection as GraphQLField)!;
                    if (sel.Name.StringValue == "userNotificationsPreferences")
                    {
                        foreach (var psel in sel.SelectionSet!.Selections)
                        {
                            if (selection.Kind == ASTNodeKind.Field)
                            {
                                subfields.Add(
                                    (psel as GraphQLField)!.Name.StringValue
                                );
                            }
                        }
                    }
                    else
                    {
                        subfields.Add(sel.Name.StringValue);
                    }
                }
                else if (selection.Kind == ASTNodeKind.FragmentSpread)
                {
                    var notifsPrefsFragName = (
                        selection as GraphQLFragmentSpread
                    )!
                        .FragmentName
                        .Name
                        .StringValue;
                    var notifsPrefsFragDef = GetFragmentDefinition(
                        notifsPrefsFragName,
                        document
                    );
                    if (
                        notifsPrefsFragDef.TypeCondition.Type.Name.StringValue
                        != "UserNotificationsPreferences"
                    )
                    {
                        throw new NotImplementedException();
                    }
                    var notifsPrefsSelections = GetFragmentSelections(
                        GetFragmentSource(notifsPrefsFragDef, document)
                    );
                    subfields.AddRange(notifsPrefsSelections);
                }
            }
        }

        var spreads = userFieldAst.SelectionSet.Selections.Where(
            s => s.Kind == ASTNodeKind.FragmentSpread
        );
        if (!spreads.Any())
            return subfields;

        IEnumerable<string> spreadSelections = spreads
            .Select(
                s =>
                    GetFragmentSource((s as GraphQLFragmentSpread)!, document)
                        .Trim()
            )
            .SelectMany(s => GetFragmentSelections(s));
        foreach (var selection in spreadSelections)
        {
            if (
                selection == "personalInfo"
                || selection == "userNotificationsPreferences"
            )
                continue;
            if (selection == "id" || selection == "name")
            {
                if (spreadSelections.Contains("theme"))
                    continue;
            }
            if (selection.StartsWith("..."))
            {
                var personalInfoFragmentName = selection[3..];
                var personalInfoFragDef = GetFragmentDefinition(
                    personalInfoFragmentName,
                    document
                );
                var personalInfoFragSelections = GetFragmentSelections(
                    GetFragmentSource(personalInfoFragDef, document)
                );
                foreach (var sel in personalInfoFragSelections)
                {
                    if (sel.StartsWith("..."))
                    {
                        var notifsPrefsFragmentName = selection[3..];
                        var notifsPrefsFragDef = GetFragmentDefinition(
                            notifsPrefsFragmentName,
                            document
                        );
                        var notifsPrefsFragSelections = GetFragmentSelections(
                            GetFragmentSource(notifsPrefsFragDef, document)
                        );
                        subfields.AddRange(notifsPrefsFragSelections);
                    }
                    else if (sel != "userNotificationsPreferences")
                    {
                        subfields.Add(sel);
                    }
                }
            }
            else
            {
                subfields.Add(selection);
            }
        }

        return subfields;
    }

    private static List<string> FieldsToDbColumns_User(List<string> fields)
    {
        List<string> cols = fields
            .Select(f => f.First().ToString().ToUpper() + f[1..])
            .ToList();
        for (int i = 0; i < cols.Count; i++)
        {
            var col = cols[i];
            if (col == "NotifSound")
            {
                cols[i] = "NotificationSound";
            }
            else if (col == "AllowAlertsStartTimeUTC")
            {
                cols[i] = "NotificationsAllowStartTime";
            }
            else if (col == "AllowAlertsEndTimeUTC")
            {
                cols[i] = "NotificationsAllowEndTime";
            }
            else if (col == "PauseAlertsUntil")
            {
                cols[i] = "NotificationsPauseUntil";
            }
            else if (col == "Avatar")
            {
                cols[i] = "AvatarId";
            }
            else if (col == "Theme")
            {
                cols[i] = "ThemeId";
            }
            else if (col == "Username")
            {
                cols[i] = "UserName";
            }
        }

        return cols;
    }

    public static GraphQLFragmentDefinition GetFragmentDefinition(
        string fragmentName,
        GraphQLDocument document
    )
    {
        return (GraphQLFragmentDefinition)
            document.Definitions.First(
                d =>
                    d.Kind == ASTNodeKind.FragmentDefinition
                    && (d as GraphQLFragmentDefinition)!
                        .FragmentName
                        .Name
                        .StringValue == fragmentName
            );
    }

    public static string[] GetFragmentSelections(string source)
    {
        var regex1 = new Regex(@"\w+");
        var regex2 = new Regex(@"fragment .* on .*{.*");
        var lines = source.Split(Environment.NewLine);
        lines = lines
            .Where(l => regex1.IsMatch(l) && !regex2.IsMatch(l))
            .Select(line => TrimRemoveTrailingCommaOpeningBracket(line))
            .ToArray();
        return lines;
    }

    private static string TrimRemoveTrailingCommaOpeningBracket(string s)
    {
        return s.Trim().TrimEnd(',').TrimEnd('{').Trim();
    }

    private static string GetFragmentSource(
        GraphQLFragmentDefinition fragDef,
        GraphQLDocument document
    )
    {
        var start = fragDef.Location.Start;
        var stop = fragDef.Location.End;

        return document.Source.Span.Slice(start, stop - start).ToString();
    }

    private static string GetFragmentSource(
        GraphQLFragmentSpread spread,
        GraphQLDocument document
    )
    {
        var fragDef = document.Definitions.First(
            d =>
                d.Kind == ASTNodeKind.FragmentDefinition
                && (d as GraphQLFragmentDefinition)!
                    .FragmentName
                    .Name
                    .StringValue == spread.FragmentName.Name.StringValue
        );
        var start = fragDef.Location.Start;
        var stop = fragDef.Location.End;

        return document.Source.Span.Slice(start, stop - start).ToString();
    }
    /*

    public static FieldInfo WorkspaceMembers(
        string opString,
        Dictionary<string, string> fragments
    )
    {
        string membersFieldSlice = GetFieldSlice(
            opString,
            "members",
            "workspace"
        );
        return CollectFields(membersFieldSlice, fragments);
    }

    /// <summary>
    /// Helper method for getting all subfields of a User object
    /// in a GraphQL document. Not all fields that refer to a User
    /// object have the field name 'user', and this method addresses
    /// that situation.
    /// </summary>
    public static List<string> ExtractUserFields(
        string userFieldName,
        FieldTree parentTree
    )
    {
        List<string> fields = new();
        CollectUserFields(userFieldName, parentTree, fields);
        return fields;
    }

    /// <summary>
    /// Helper method for ExtractUserFields() method
    /// </summary>
    private static void CollectUserFields(
        string userFieldName,
        FieldTree root,
        List<string> fields,
        bool inUserSubtree = false
    )
    {
        if (inUserSubtree)
        {
            fields.Add(root.FieldName);
        }
        bool userRoot =
            root.FieldName.Length >= userFieldName.Length
            && root.FieldName[..userFieldName.Length] == userFieldName;
        foreach (FieldTree child in root.Children)
        {
            CollectUserFields(
                userFieldName,
                child,
                fields,
                userRoot || inUserSubtree
            );
        }
    }

    /// <summary>
    /// Gets the substring containing all subfields of a field.
    /// </summary>
    private static string GetFieldSlice(
        string opString,
        string fieldName,
        string? afterString = null
    )
    {
        int start;
        if (afterString is not null)
        {
            var afterIdx = opString.IndexOf(afterString);
            if (afterIdx == -1)
            {
                throw new InvalidOperationException();
            }
            start = opString.IndexOf(fieldName, afterIdx);
        }
        else
        {
            start = opString.IndexOf(fieldName);
        }

        if (start == -1)
        {
            throw new InvalidOperationException();
        }
        var openingParenIdx = opString.IndexOf('{', start);
        var stop = GetMatchingClosingParenIdx(opString, openingParenIdx) + 1;
        return opString[start..stop];
    }

    /// <summary>
    /// Parses a substring (fieldSlice) from a GraphQL document containing all subfields of
    /// a particular field. Returns a tree data structure representing the parsed
    /// field and its subfields, as well as a list containing the flattened version of
    /// the tree without the root field (only the names of the subfields).
    /// </summary>
    private static FieldInfo CollectFields(
        string fieldSlice,
        Dictionary<string, string> fragments
    )
    {
        int i = fieldSlice.IndexOf("{");
        string rootField = fieldSlice[..i].Trim();
        FieldTree root =
            new() { FieldName = rootField, Children = new List<FieldTree>() };

        List<string> subFieldNames = new();
        TraverseSlice(root, fieldSlice, subFieldNames, i + 1, fragments);

        return new FieldInfo
        {
            FieldTree = root,
            SubfieldNames = subFieldNames
        };
    }

    /// <summary>
    /// Helper method for CollectFields method. Performs most of the heavy lifting;
    /// parses the fieldSlice, generates the representative tree data structure and
    /// list of all subfield names contained in the fieldSlice.
    /// </summary>
    private static (FieldTree fieldTree, int parsedToIdx) TraverseSlice(
        FieldTree root,
        string fieldSlice,
        List<string> subFieldNames,
        int i,
        Dictionary<string, string> fragments
    )
    {
        Stack<(int, string)> returnFromFragmentStack = new();
        while (i < fieldSlice.Length)
        {
            char c = fieldSlice[i];
            if (char.IsWhiteSpace(c))
            {
                i++;
                continue;
            }
            else if (c == '}')
            {
                if (returnFromFragmentStack.Count == 0)
                {
                    break;
                }
                (i, fieldSlice) = returnFromFragmentStack.Pop();
            }
            else if (c == '{')
            {
                FieldTree rootChildWithChildren = root.Children.Last();
                (FieldTree _, int k) = TraverseSlice(
                    rootChildWithChildren,
                    fieldSlice,
                    subFieldNames,
                    i + 1,
                    fragments
                );
                i = k;
            }
            else if (c == '.')
            {
                int stop = fieldSlice.IndexOf("Fragment", i) + 8;
                if (stop < fieldSlice.Length && fieldSlice[stop] == '_')
                {
                    stop += 7;
                }
                string fragmentName = fieldSlice.Substring(
                    i + 3,
                    stop - (i + 3)
                );
                string fragment = fragments[fragmentName];
                returnFromFragmentStack.Push((stop, fieldSlice));
                fieldSlice = fragment;
                i = 1;
            }
            else
            {
                int k = i;
                bool parsingArgs = false;
                while (
                    k < fieldSlice.Length
                    && (!char.IsWhiteSpace(fieldSlice[k]) || parsingArgs)
                )
                {
                    char fieldChar = fieldSlice[k++];
                    if (fieldChar == '(')
                    {
                        parsingArgs = true;
                    }
                    if (fieldChar == ')')
                    {
                        break;
                    }
                }

                string fieldName = fieldSlice[i..k];
                if (fieldName.EndsWith("UTC"))
                {
                    fieldName = fieldName[..(fieldName.Length - 3)];
                }
                subFieldNames.Add(fieldName);
                FieldTree field = new(fieldName);
                root.Children.Add(field);

                i = k;
            }
        }

        return (root, i + 1);
    }
    */
}

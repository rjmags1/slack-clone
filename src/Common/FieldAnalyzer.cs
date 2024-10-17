using System.Text.RegularExpressions;
using GraphQL.Types;
using GraphQL.Validation;
using GraphQLParser.AST;

namespace Common.SlackCloneGraphQL;

public static class FieldAnalyzer
{
    private static List<string> RemoveTypeNameAndDuplicateIdFields(
        IEnumerable<string> fields
    )
    {
        List<string> result = new();
        foreach (var f in fields)
        {
            if (f == "__typename")
            {
                continue;
            }
            if (
                (f == "id" || f == "Id")
                && result.Count > 0
                && result[result.Count - 1] == f
            )
            {
                continue;
            }

            result.Add(f);
        }

        return result;
    }

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

    public static List<string> DirectMessageDbColumns(
        GraphQLField directMessageFieldAst,
        GraphQLDocument document
    )
    {
        if (directMessageFieldAst.SelectionSet is null)
        {
            throw new InvalidOperationException(
                "null selection set on direct message"
            );
        }

        var subfields = directMessageFieldAst.SelectionSet.Selections
            .Where(s => s.Kind == ASTNodeKind.Field)
            .Select(s => (s as GraphQLField)!.Name.StringValue)
            .ToList();
        var fragmentSpreads = directMessageFieldAst.SelectionSet.Selections
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

        subfields = subfields
            .Select(s => s[0].ToString().ToUpper() + s[1..])
            .Select(s => s == "User" ? "UserId" : s)
            .Select(s => s == "CreatedAtUTC" ? "CreatedAt" : s)
            .Select(s => s == "LastEditUTC" ? "LastEdit" : s)
            .Select(s => s == "Group" ? "DirectMessageGroupId" : s)
            .Select(s => s == "LaterFlag" ? "LaterFlagId" : s)
            .Select(s => s == "SentAtUTC" ? "SentAt" : s)
            .Where(s => s != "Draft" && s != "Type")
            .ToList();

        return RemoveTypeNameAndDuplicateIdFields(subfields);
    }

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

        subfields = subfields
            .Select(s => s[0].ToString().ToUpper() + s[1..])
            .Select(s => s == "Avatar" ? "AvatarId" : s)
            .Select(s => s == "User" ? "UserId" : s)
            .Select(s => s == "Workspace" ? "WorkspaceId" : s)
            .Where(s => s != "WorkspaceMemberInfo")
            .ToList();

        return RemoveTypeNameAndDuplicateIdFields(subfields);
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

        subfields = subfields
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

        return RemoveTypeNameAndDuplicateIdFields(subfields);
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

        subfields = subfields
            .Select(s => s[0].ToString().ToUpper() + s[1..])
            .Select(s => s == "User" ? "UserId" : s)
            .Select(s => s == "CreatedAtUTC" ? "CreatedAt" : s)
            .Select(s => s == "LastEditUTC" ? "LastEdit" : s)
            .Select(s => s == "Group" ? "ChannelId" : s)
            .Select(s => s == "LaterFlag" ? "LaterFlagId" : s)
            .Select(s => s == "SentAtUTC" ? "SentAt" : s)
            .Where(s => s != "Draft" && s != "Type")
            .ToList();

        return RemoveTypeNameAndDuplicateIdFields(subfields);
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

        subfields = subfields
            .Select(s => s == "user" ? "UserId" : s)
            .Select(s => s[0].ToString().ToUpper() + s[1..])
            .ToList();

        return RemoveTypeNameAndDuplicateIdFields(subfields);
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

        subfields = subfields
            .Select(s => s[0].ToString().ToUpper() + s[1..])
            .ToList();

        return RemoveTypeNameAndDuplicateIdFields(subfields);
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

        subfields = subfields
            .Select(s => s == "createdAtUTC" ? "createdAt" : s)
            .Select(s => s == "workspace" ? "workspaceId" : s)
            .Select(s => s[0].ToString().ToUpper() + s[1..])
            .ToList();

        return RemoveTypeNameAndDuplicateIdFields(subfields);
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
        subfields = subfields
            .Where(s => s != "name")
            .Select(s => s == "createdAtUTC" ? "createdAt" : s)
            .Select(s => s == "workspace" ? "workspaceId" : s)
            .Select(s => s[0].ToString().ToUpper() + s[1..])
            .ToList();

        return RemoveTypeNameAndDuplicateIdFields(subfields);
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

        subfields = subfields
            .Select(s => s.First().ToString().ToUpper() + s[1..])
            .Select(s => s == "Avatar" ? "AvatarId" : s)
            .Select(s => s == "CreatedBy" ? "CreatedById" : s)
            .Select(s => s == "CreatedAtUTC" ? "CreatedAt" : s)
            .Select(s => s == "Workspace" ? "WorkspaceId" : s)
            .ToList();

        return RemoveTypeNameAndDuplicateIdFields(subfields);
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

        subfields = subfields
            .Select(s => s.First().ToString().ToUpper() + s[1..])
            .Select(s => s == "Avatar" ? "AvatarId" : s)
            .ToList();

        return RemoveTypeNameAndDuplicateIdFields(subfields);
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

        var subfields = GetSubfields_User(userFieldAst, document);

        return RemoveTypeNameAndDuplicateIdFields(
            FieldsToDbColumns_User(subfields)
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
}

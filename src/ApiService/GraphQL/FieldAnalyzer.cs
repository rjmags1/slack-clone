using PersistenceService.Utils.GraphQL;

namespace SlackCloneGraphQL;

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
            int fragmentNameStop = document.IndexOf("on", i) - 1;
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
                subFieldNames.Add(fieldName);
                FieldTree field = new(fieldName);
                root.Children.Add(field);

                i = k;
            }
        }

        return (root, i + 1);
    }
}

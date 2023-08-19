using GraphQL;
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
    public static Dictionary<string, string> GetFragments(
        IResolveFieldContext context
    )
    {
        string document = context.Document.Source.ToString();
        int i = document.IndexOf("fragment") + 9;
        int k;
        Dictionary<string, string> fragments = new();
        bool done = i == -1;
        while (!done)
        {
            int n = document.IndexOf("on", i) - 1;
            string fragmentName = document.Substring(i, n - i);
            int j = document.IndexOf('{', n);
            k = GetMatchingClosingParenIdx(document, j);
            fragments[fragmentName] = document.Substring(j, k - j + 1);
            i = document.IndexOf("fragment", k);
            done = i == -1;
            i += 9;
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

    public static FieldInfo Workspaces(
        IResolveFieldContext context,
        Dictionary<string, string> fragments
    )
    {
        string workspacesFieldSlice = GetFieldSlice(context);
        return CollectFields(workspacesFieldSlice, fragments);
    }

    public static FieldInfo User(
        IResolveFieldContext context,
        Dictionary<string, string> fragments
    )
    {
        string userFieldSlice = GetFieldSlice(context);
        return CollectFields(userFieldSlice, fragments);
    }

    public static FieldInfo WorkspaceMembers(
        IResolveFieldContext context,
        Dictionary<string, string> fragments
    )
    {
        string membersFieldSlice = GetFieldSlice(context);
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
        List<string> fields = new List<string>();
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
        bool userRoot = root.FieldName == userFieldName;
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
    /// Gets the substring containing all subfields of a field
    /// without converting the entire GraphQL document to a string.
    /// </summary>
    private static string GetFieldSliceFromParentContext(
        IResolveFieldContext context,
        string fieldName
    )
    {
        var field = context.SubFields!
            .First(f => f.Key == fieldName)
            .Value.Field;
        int start = field.Location.Start;
        int stop = field.Location.End;
        string fieldSlice = context.Document.Source
            .Slice(start, stop - start)
            .ToString();

        return fieldSlice;
    }

    /// <summary>
    /// Gets the substring containing all subfields of a field
    /// without converting the entire GraphQL document to a string.
    /// </summary>
    private static string GetFieldSlice(IResolveFieldContext context)
    {
        var start = context.FieldAst.Location.Start;
        var stop = context.FieldAst.Location.End;
        string slice = context.Document.Source
            .Slice(start, stop - start)
            .ToString();
        return slice;
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
        string rootField = fieldSlice.Substring(0, i).Trim();
        FieldTree root = new FieldTree
        {
            FieldName = rootField,
            Children = new List<FieldTree>()
        };
        List<string> subFieldNames = new List<string>();

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

                string fieldName = fieldSlice.Substring(i, (k - i));
                subFieldNames.Add(fieldName);
                FieldTree field = new FieldTree(fieldName);
                root.Children.Add(field);

                i = k;
            }
        }

        return (root, i + 1);
    }
}

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
    public static FieldInfo Workspaces(IResolveFieldContext context)
    {
        string workspacesFieldSlice = GetFieldSliceFromParentContext(
            context,
            "workspaces"
        );
        return CollectFields(workspacesFieldSlice);
    }

    public static FieldInfo User(IResolveFieldContext context, Guid userId)
    {
        string userFieldSlice = GetFieldSliceFromParentContext(context, "user");
        return CollectFields(userFieldSlice);
    }

    public static FieldInfo WorkspaceMembers(IResolveFieldContext context)
    {
        string membersFieldSlice = GetFieldSlice(context);
        return CollectFields(membersFieldSlice);
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
    private static FieldInfo CollectFields(string fieldSlice)
    {
        int i = fieldSlice.IndexOf("{");
        string rootField = fieldSlice.Substring(0, i).Trim();
        FieldTree root = new FieldTree
        {
            FieldName = rootField,
            Children = new List<FieldTree>()
        };
        List<string> subFieldNames = new List<string>();

        TraverseSlice(root, fieldSlice, subFieldNames, i + 1);

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
        int i
    )
    {
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
                break;
            }
            else if (c == '{')
            {
                FieldTree rootChildWithChildren = root.Children.Last();
                (FieldTree _, int k) = TraverseSlice(
                    rootChildWithChildren,
                    fieldSlice,
                    subFieldNames,
                    i + 1
                );
                i = k;
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

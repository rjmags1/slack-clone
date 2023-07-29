using System.Collections;
using GraphQL;
using SlackCloneGraphQL.Types;

namespace SlackCloneGraphQL;

public static class FieldAnalyzer
{
    public static ((string, ArrayList), List<string>) Workspaces(
        IResolveFieldContext context,
        WorkspacesFilter filter
    )
    {
        string workspacesFieldSlice = GetFieldSlice(context, "workspaces");
        var (fields, flattened) = CollectFields(workspacesFieldSlice);
        return (fields, flattened);
    }

    public static List<string> User(IResolveFieldContext context, Guid userId)
    {
        string userFieldSlice = GetFieldSlice(context, "user");
        var (fields, flattened) = CollectFields(userFieldSlice);
        return flattened;
    }

    public static ((string, ArrayList), List<string>) WorkspaceMembers(
        IResolveFieldContext context
    )
    {
        var start = context.FieldAst.Location.Start;
        var stop = context.FieldAst.Location.End;
        string membersFieldSlice = context.Document.Source
            .Slice(start, stop - start)
            .ToString();
        var (fields, flattened) = CollectFields(membersFieldSlice);
        return (fields, flattened);
    }

    public static List<string> ExtractUserFields(
        string userFieldName,
        (string, ArrayList) connectionTree
    )
    {
        List<string> fields = new List<string>();
        CollectUserFields(userFieldName, connectionTree, fields);
        return fields;
    }

    private static void CollectUserFields(
        string userFieldName,
        (string, ArrayList) root,
        List<string> fields,
        bool inUserSubtree = false
    )
    {
        if (inUserSubtree)
        {
            fields.Add(root.Item1);
        }
        bool nodeRoot = root.Item1 == userFieldName;
        foreach ((string, ArrayList) child in root.Item2)
        {
            CollectUserFields(
                userFieldName,
                child,
                fields,
                nodeRoot || inUserSubtree
            );
        }
    }

    private static string GetFieldSlice(
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

    private static ((string, ArrayList), List<string>) CollectFields(
        string fieldSlice
    )
    {
        int i = fieldSlice.IndexOf("{");
        string rootField = fieldSlice.Substring(0, i).Trim();
        (string, ArrayList) root = (rootField, new ArrayList());
        List<string> flattened = new List<string>();

        TraverseSlice(root, fieldSlice, flattened, i + 1);

        return (root, flattened);
    }

    private static ((string, ArrayList), int) TraverseSlice(
        (string, ArrayList) root,
        string fieldSlice,
        List<string> flattened,
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
                (string, ArrayList) rootChildWithChildren = ((
                    string,
                    ArrayList
                ))
                    root.Item2[root.Item2.Count - 1]!;
                ((string, ArrayList) _, int k) = TraverseSlice(
                    rootChildWithChildren,
                    fieldSlice,
                    flattened,
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
                flattened.Add(fieldName);
                (string, ArrayList) field = (fieldName, new ArrayList());
                root.Item2!.Add(field);

                i = k;
            }
        }

        return (root, i + 1);
    }
}

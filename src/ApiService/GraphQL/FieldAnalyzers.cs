using GraphQL;
using SlackCloneGraphQL.Types;

namespace SlackCloneGraphQL;

public static class FieldAnalyzer
{
    public static IEnumerable<string> Workspaces(
        IResolveFieldContext context,
        WorkspacesFilter filter
    )
    {
        throw new NotImplementedException();
    }

    public static IEnumerable<string> User(
        IResolveFieldContext context,
        Guid userId
    )
    {
        var userField = context.SubFields!
            .First(f => f.Key == "user")
            .Value.Field;
        int start = userField.Location.Start;
        int stop = userField.Location.End;
        var userFieldSlice = context.Document.Source
            .Slice(start, stop - start)
            .ToString();

        return CollectFields(userFieldSlice);
    }

    private static IEnumerable<string> CollectFields(string fieldSlice)
    {
        List<string> fields = new List<string>();
        Stack<int> stack = new Stack<int>();
        int i = fieldSlice.IndexOf("{");
        stack.Push(i++);
        while (stack.Count > 0)
        {
            char c = fieldSlice[i++];
            if (char.IsWhiteSpace(c))
            {
                continue;
            }
            else if (c == '{')
            {
                stack.Push(i - 1);
            }
            else if (c == '}')
            {
                stack.Pop();
            }
            else
            {
                int k;
                bool hasArgs = false;
                for (k = i; k < fieldSlice.Length; k++)
                {
                    c = fieldSlice[k];
                    if (c == '(')
                    {
                        hasArgs = true;
                    }
                    if (
                        (!hasArgs && char.IsWhiteSpace(c))
                        || (hasArgs && c == ')')
                    )
                    {
                        break;
                    }
                }
                fields.Add(fieldSlice.Substring(i - 1, k - (i - 1)));
                i = hasArgs ? k : ++k;
            }
        }

        return fields;
    }
}

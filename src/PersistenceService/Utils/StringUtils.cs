using System.Collections;

namespace PersistenceService.Utils;

public static class StringUtils
{
    public static string ToUpperFirstLetter(string s)
    {
        if (string.IsNullOrEmpty(s))
        {
            return s;
        }

        return char.ToUpper(s[0]) + s.Substring(1);
    }
}

public static class DynamicLinqUtils
{
    public static string NodeFieldToDynamicSelectString(
        (string, ArrayList) connectionTree
    )
    {
        List<string> nodeFields = new List<string>();
        CollectNodeFields(connectionTree, nodeFields);
        List<string> tokens = new List<string> { "new { " };
        if (nodeFields.Count == 1)
        {
            tokens.Add($"{nodeFields.First()} ");
        }
        else
        {
            int i = 0;
            foreach (string field in nodeFields)
            {
                tokens.Add(
                    $"{StringUtils.ToUpperFirstLetter(field)}{(i++ == nodeFields.Count - 1 ? "" : ",")} "
                );
            }
        }
        tokens.Add("}");

        return string.Join("", tokens);
    }

    private static void CollectNodeFields(
        (string, ArrayList) root,
        List<string> fields
    )
    {
        bool shouldCollect = root.Item1 == "node";
        foreach ((string, ArrayList) child in root.Item2)
        {
            if (shouldCollect)
            {
                fields.Add(child.Item1);
            }
            CollectNodeFields(child, fields);
        }
    }
}

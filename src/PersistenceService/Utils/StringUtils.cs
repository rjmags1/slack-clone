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
        (string, ArrayList) connectionTree,
        List<string>? nonDbMapped = null
    )
    {
        List<string> nodeFields = new List<string>();
        CollectNodeFields(connectionTree, nodeFields, nonDbMapped);
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
                if (!(nonDbMapped is null) && nonDbMapped.Contains(field))
                {
                    continue;
                }
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
        List<string> fields,
        List<string>? nonDbMapped = null,
        bool parentIsNonDbMapped = false
    )
    {
        if (parentIsNonDbMapped)
        {
            fields.Add(root.Item1);
        }
        bool nodeLevel = root.Item1 == "node";
        bool rootInNonDbMapped = nonDbMapped is null
            ? false
            : nonDbMapped.Contains(root.Item1);
        foreach ((string, ArrayList) child in root.Item2)
        {
            if (nodeLevel)
            {
                fields.Add(child.Item1);
            }
            CollectNodeFields(child, fields, nonDbMapped, rootInNonDbMapped);
        }
    }
}

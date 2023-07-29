using PersistenceService.Utils.GraphQL;

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
        FieldTree connectionTree,
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
        FieldTree root,
        List<string> fields,
        List<string>? nonDbMapped = null,
        bool parentIsNonDbMapped = false
    )
    {
        if (parentIsNonDbMapped)
        {
            fields.Add(root.FieldName);
        }
        bool nodeLevel = root.FieldName == "node";
        bool rootInNonDbMapped = nonDbMapped is null
            ? false
            : nonDbMapped.Contains(root.FieldName);
        foreach (FieldTree child in root.Children)
        {
            if (nodeLevel)
            {
                fields.Add(child.FieldName);
            }
            CollectNodeFields(child, fields, nonDbMapped, rootInNonDbMapped);
        }
    }
}

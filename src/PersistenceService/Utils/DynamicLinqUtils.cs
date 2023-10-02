using PersistenceService.Utils.GraphQL;

namespace PersistenceService.Utils;

public static class DynamicLinqUtils
{
    public static string NodeFieldToDynamicSelectString(
        FieldTree connectionTree,
        List<string>? nonDbMapped = null,
        List<string>? forceInclude = null,
        List<string>? skip = null,
        Dictionary<string, string>? map = null
    )
    {
        List<string> nodeFields = new List<string>();
        CollectNodeFields(
            connectionTree,
            nodeFields,
            nonDbMapped: nonDbMapped,
            skip: skip,
            map: map
        );
        if (forceInclude is not null)
        {
            foreach (string include in forceInclude)
            {
                string s = map is null
                    ? include
                    : map.GetValueOrDefault(include, include);
                if (!nodeFields.Contains(s))
                {
                    nodeFields.Add(s);
                }
            }
        }
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
        bool parentIsNonDbMapped = false,
        List<string>? skip = null,
        Dictionary<string, string>? map = null
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
            bool skippedField =
                nodeLevel
                && skip is not null
                && skip.Any(s => child.FieldName.StartsWith(s));
            if (
                nodeLevel
                && child.FieldName.Substring(0, 2) != "__"
                && !skippedField
            )
            {
                if (map is not null && map.ContainsKey(child.FieldName))
                {
                    fields.Add(map[child.FieldName]);
                }
                else
                {
                    fields.Add(child.FieldName);
                }
            }
            if (!skippedField)
            {
                CollectNodeFields(
                    child,
                    fields,
                    nonDbMapped,
                    rootInNonDbMapped,
                    skip,
                    map
                );
            }
        }
    }
}

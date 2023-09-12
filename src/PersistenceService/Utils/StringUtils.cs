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

namespace ApiService.Utils;

public class StringUtils
{
    public static string ToLowerFirstLetter(string s)
    {
        if (string.IsNullOrEmpty(s))
        {
            return s;
        }

        return char.ToLower(s[0]) + s[1..];
    }
}

using System.Globalization;

namespace PersistenceService.Utils;

public class EmojiUtils
{
    public static bool IsEmoji(string s)
    {
        int length = 0;
        var enumerator = StringInfo.GetTextElementEnumerator(s);
        while (enumerator.MoveNext() && length < 2)
        {
            string textElement = enumerator.GetTextElement();
            if (IsNonEmojiTextElement(textElement))
            {
                return false;
            }
            length++;
        }
        return length == 1;
    }

    private static bool IsEmojiCodePoint(UnicodeCategory category)
    {
        return category == UnicodeCategory.OtherSymbol
            || category == UnicodeCategory.OtherNotAssigned;
    }

    private static bool IsNonEmojiTextElement(string textElement)
    {
        foreach (
            var codePoint in StringInfo.ParseCombiningCharacters(textElement)
        )
        {
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(
                textElement,
                codePoint
            );

            if (IsEmojiCodePoint(unicodeCategory))
            {
                return false;
            }
        }

        return true;
    }
}

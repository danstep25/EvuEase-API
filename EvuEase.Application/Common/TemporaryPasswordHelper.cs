using System.Security.Cryptography;

namespace EvuEase.Application.Common;

public static class TemporaryPasswordHelper
{
    // Excludes ambiguous characters (0/O, 1/I/l) for easier manual relay.
    private const string UppercaseChars = "ABCDEFGHJKLMNPQRSTUVWXYZ";
    private const string LowercaseChars = "abcdefghijkmnpqrstuvwxyz";
    private const string DigitChars = "23456789";
    private const string SymbolChars = "@#$%*?";

    private const int DefaultLength = 10;

    public static string Generate(int length = DefaultLength)
    {
        if (length < 8)
        {
            length = 8;
        }

        var allChars = UppercaseChars + LowercaseChars + DigitChars + SymbolChars;

        // Guarantee at least one character from each category.
        var chars = new List<char>
        {
            PickRandom(UppercaseChars),
            PickRandom(LowercaseChars),
            PickRandom(DigitChars),
            PickRandom(SymbolChars)
        };

        while (chars.Count < length)
        {
            chars.Add(PickRandom(allChars));
        }

        return Shuffle(chars);
    }

    private static char PickRandom(string source)
    {
        var index = RandomNumberGenerator.GetInt32(source.Length);
        return source[index];
    }

    private static string Shuffle(List<char> chars)
    {
        for (var i = chars.Count - 1; i > 0; i--)
        {
            var j = RandomNumberGenerator.GetInt32(i + 1);
            (chars[i], chars[j]) = (chars[j], chars[i]);
        }

        return new string(chars.ToArray());
    }
}

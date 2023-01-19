using System.Drawing;

namespace TTT.Tesseract;

/// <summary>
/// </summary>
/// <param name="Text"></param>
/// <param name="Rectangle"></param>
public record TextBox(string Text, Rectangle Rectangle)
{
    private string? _stripedText;
    public string StripedText => _stripedText ??= Text.Striped();

    public bool Match(string find, bool ignoreCase = false, bool contains = true, bool strip = true)
    {
        var source = strip ? StripedText : Text;
        var search = strip ? find.Striped() : find;
        return Match(source, search, ignoreCase, contains);
    }

    private static bool Match(string source, string find, bool ignoreCase = false, bool contains = true)
    {
        var mode = ignoreCase
            ? StringComparison.CurrentCultureIgnoreCase
            : StringComparison.CurrentCulture;

        if (contains)
        {
            if (source.Contains(find, mode)) return true;
        }
        else if (source.Equals(find, mode))
        {
            return true;
        }

        return false;
    }
}
using System.Drawing;

namespace TTT.Tesseract;

/// <summary>
/// TODO : chuyển textbox thành các object chứa luôn các box con hoặc null, search in text hoặc in boxes
/// </summary>
/// <param name="Text"></param>
/// <param name="Rectangle"></param>
public record TextBox(string Text, Rectangle Rectangle)
{
    

    private string? _stripedText;
    public string StripedText => _stripedText ??= Helper.GetStripedText(Text);

    public bool Match(string find, bool ignoreCase = false, bool contains = true, bool strip = true)
    {
        var source = strip ? StripedText : Text;
        return Match(source, find, ignoreCase, contains);
    }

    public static bool Match(string source, string find, bool ignoreCase = false, bool contains = true)
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
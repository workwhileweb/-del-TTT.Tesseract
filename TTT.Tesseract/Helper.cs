using System.Diagnostics;
using System.Drawing;
using Tesseract;
using TTT.Tesseract.Properties;

// ReSharper disable UnusedMember.Global

namespace TTT.Tesseract;

public static class Helper
{
    public const string Space = " ";
    public const string DoubleSpace = Space + Space;

    // ReSharper disable once UnusedMember.Local
    public static readonly Destructor StaticDestructor = new();

    public static readonly Process Process = Process.GetCurrentProcess();

    public static readonly ProcessModule MainModule = Process.MainModule
                                                      ?? throw new InvalidOperationException(nameof(MainModule));

    public static readonly string ExeFile = MainModule.FileName;

    public static readonly string ExeFolder = Path.GetDirectoryName(ExeFile)
                                              ?? throw new InvalidOperationException(nameof(ExeFolder));

    public static readonly string TessDataFolder = Path.Combine(ExeFolder, "tessdata");
    public static readonly string EngTessData = Path.Combine(TessDataFolder, "eng.traineddata");
    public static readonly string OsdTessData = Path.Combine(TessDataFolder, "osd.traineddata");

    public static readonly TesseractEngine EngEngine;

    static Helper()
    {
        if (!Directory.Exists(TessDataFolder))
        {
            Directory.CreateDirectory(TessDataFolder);
            File.WriteAllBytes(EngTessData, Resources.eng);
            File.WriteAllBytes(OsdTessData, Resources.osd);
        }

        EngEngine = new TesseractEngine(TessDataFolder, "eng", EngineMode.Default);

        AppDomain.CurrentDomain.ProcessExit += (_, _) => EngEngine.Dispose();
    }

    public static string Striped(this string input)
    {
        return new string(input.Where(char.IsLetterOrDigit).ToArray());
        /*var filtered = input.Where(x => char.IsWhiteSpace(x) || char.IsLetterOrDigit(x)).ToArray();
        var result = new string(filtered);
        while (result.Contains(DoubleSpace)) result = result.Replace(DoubleSpace, Space);
        return result;*/
    }

    public static Page Parse(this Bitmap bitmap)
    {
        using var pix = PixConverter.ToPix(bitmap);
        return Parse(pix);
    }

    public static Page Parse(this string path)
    {
        using var img = Pix.LoadFromFile(path);
        return Parse(img);
    }

    private static Rectangle ToRectangle(this Rect rect)
    {
        return new Rectangle(rect.X1, rect.Y1, rect.Width, rect.Height);
    }

    public static Page Parse(this Pix img)
    {
        using var page = EngEngine.Process(img);
        using var iterator = page.GetIterator();
        iterator.Begin();

        var blocks = new List<Block>();
        do
        {
            var paragraphs = new List<Paragraph>();
            do
            {
                var lines = new List<Line>();
                do
                {
                    var textBoxes = new List<TextBox>();
                    do
                    {
                        //var beginBlock = iterator.IsAtBeginningOf(PageIteratorLevel.Block);
                        //var endLine = iterator.IsAtFinalOf(PageIteratorLevel.TextLine, PageIteratorLevel.Word);
                        //var endPara = iterator.IsAtFinalOf(PageIteratorLevel.Para, PageIteratorLevel.TextLine);
                        //iterator.TryGetBaseline(PageIteratorLevel.TextLine, out var baseLine);
                        iterator.TryGetBoundingBox(PageIteratorLevel.Word, out var wordRect);
                        var word = iterator.GetText(PageIteratorLevel.Word);
                        textBoxes.Add(new TextBox(word, wordRect.ToRectangle()));
                        //Action<object> status(new { word, baseLine , boundingBox , beginBlock , endLine, endPara });
                        //status(new { text = page.GetText() , meanConfidence=page.GetMeanConfidence() });
                    } while (iterator.Next(PageIteratorLevel.TextLine, PageIteratorLevel.Word));

                    iterator.TryGetBoundingBox(PageIteratorLevel.TextLine, out var lineRect);
                    lines.Add(new Line(textBoxes, lineRect.ToRectangle()));
                } while (iterator.Next(PageIteratorLevel.Para, PageIteratorLevel.TextLine));

                iterator.TryGetBoundingBox(PageIteratorLevel.Para, out var paraRect);
                paragraphs.Add(new Paragraph(lines, paraRect.ToRectangle()));
            } while (iterator.Next(PageIteratorLevel.Block, PageIteratorLevel.Para));

            iterator.TryGetBoundingBox(PageIteratorLevel.Block, out var blockRect);
            blocks.Add(new Block(paragraphs, blockRect.ToRectangle()));
        } while (iterator.Next(PageIteratorLevel.Block));

        return new Page(blocks);
    }

    public static IEnumerable<Rectangle>? FindText(this Page page, string text,
        bool ignoreCase = true, bool contains = true, bool strip = true)
    {
        var results = FindWord(page, text, ignoreCase, contains, strip).ToList();
        if (results.Count > 0) return results;
        results = FindLine(page, text, ignoreCase, contains, strip).ToList();
        if (results.Count > 0) return results;
        results = FindParagraph(page, text, ignoreCase, contains, strip).ToList();
        if (results.Count > 0) return results;
        results = FindBlock(page, text, ignoreCase, contains, strip).ToList();
        return results.Count > 0 ? results : null;
    }

    public static IEnumerable<Rectangle> FindBlock(this Page page, string text,
        bool ignoreCase = true, bool contains = true, bool strip = true)
    {
        return
            from block in page.Boxes
            where block.Match(text, ignoreCase, contains, strip)
            select block.Rectangle;
    }

    public static IEnumerable<Rectangle> FindParagraph(this Page page, string text,
        bool ignoreCase = true, bool contains = true, bool strip = true)
    {
        return
            from block in page.Boxes
            from paragraph in block.Boxes
            where paragraph.Match(text, ignoreCase, contains, strip)
            select paragraph.Rectangle;
    }

    public static IEnumerable<Rectangle> FindLine(this Page page, string text,
        bool ignoreCase = true, bool contains = true, bool strip = true)
    {
        return
            from block in page.Boxes
            from paragraph in block.Boxes
            from line in paragraph.Boxes
            where line.Match(text, ignoreCase, contains, strip)
            select line.Rectangle;
    }

    public static IEnumerable<Rectangle> FindWord(this Page page, string text,
        bool ignoreCase = true, bool contains = true, bool strip = true)
    {
        return
            from block in page.Boxes
            from paragraph in block.Boxes
            from line in paragraph.Boxes
            from box in line.Boxes
            where box.Match(text, ignoreCase, contains, strip)
            select box.Rectangle;
    }

    /// <summary>
    ///     https://stackoverflow.com/questions/4364665/static-destructor
    /// </summary>
    public sealed class Destructor
    {
        ~Destructor()
        {
            EngEngine.Dispose();
        }
    }
}
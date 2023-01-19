using System.Drawing;

namespace TTT.Tesseract;

public record Paragraph:TextBoxes<Line>
{
    public Paragraph(IReadOnlyList<Line> boxes, Rectangle rectangle, string delimiter = Helper.DefaultDelimiter) : base(boxes, rectangle, delimiter)
    {
    }
}
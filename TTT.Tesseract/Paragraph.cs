using System.Drawing;

namespace TTT.Tesseract;

public record Paragraph : TextBoxes<Line>
{
    public Paragraph(
        IReadOnlyList<Line> boxes, 
        Rectangle rectangle, 
        string delimiter = Helper.Space) 
        : base(boxes, rectangle, delimiter)
    {
    }
}
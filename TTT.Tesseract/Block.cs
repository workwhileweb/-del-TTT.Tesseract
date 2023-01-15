using System.Drawing;

namespace TTT.Tesseract;

public record Block:TextBoxes<Paragraph>
{
    public Block(IReadOnlyList<Paragraph> boxes, Rectangle rectangle, string delimiter = DefaultDelimiter) : base(boxes, rectangle, delimiter)
    {
    }
}
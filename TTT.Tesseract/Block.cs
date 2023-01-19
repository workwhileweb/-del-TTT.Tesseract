using System.Drawing;

namespace TTT.Tesseract;

public record Block : TextBoxes<Paragraph>
{
    public Block(
        IReadOnlyList<Paragraph> boxes, 
        Rectangle rectangle, 
        string delimiter = Helper.Space) 
        : base(boxes, rectangle, delimiter)
    {
    }
}
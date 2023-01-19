using System.Drawing;

namespace TTT.Tesseract;

public record TextBoxes<T> : TextBox where T : TextBox
{
    public TextBoxes(IReadOnlyList<T> boxes, Rectangle rectangle, string delimiter = Helper.DefaultDelimiter)
        : base(string.Join(delimiter, boxes.Select(box => box.Text)), rectangle)
    {
        Boxes = boxes;
    }

    public IReadOnlyList<T> Boxes { get; }
}
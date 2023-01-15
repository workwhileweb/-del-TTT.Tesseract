// ReSharper disable UnusedMember.Global

using System.Drawing;

namespace TTT.Tesseract;

public record Page:TextBoxes<Block>
{
    public Page(IReadOnlyList<Block> boxes, string delimiter = DefaultDelimiter) : this(boxes, Rectangle.Empty, delimiter)
    {
    }

    public Page(IReadOnlyList<Block> boxes, Rectangle rectangle, string delimiter = DefaultDelimiter) : base(boxes, rectangle, delimiter)
    {
    }
}
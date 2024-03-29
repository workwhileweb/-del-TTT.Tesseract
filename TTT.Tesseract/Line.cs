﻿using System.Drawing;

namespace TTT.Tesseract;

public record Line : TextBoxes<TextBox>
{
    public Line(
        IReadOnlyList<TextBox> boxes, 
        Rectangle rectangle, 
        string delimiter = Helper.Space) 
        : base(boxes, rectangle, delimiter)
    {
    }
}
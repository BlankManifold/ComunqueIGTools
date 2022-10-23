using Godot;
using System;

public class ColorIcon : Button
{
    private ColorRect _rect;
    private Color _color;
    public Color Color
    {
        get { return _color; }
        set
        {
            _color = value;
            _rect.Color = value;
        }
    }

    public override void _Ready()
    {
        _rect = GetNode<ColorRect>("ColorRect");
    }

}

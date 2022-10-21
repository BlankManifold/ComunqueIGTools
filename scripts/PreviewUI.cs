using Godot;
using System;

public class PreviewUI : Control
{
    private ColorRect _backgroundRect;
    public override void _Ready()
    {
        _backgroundRect = GetNode<ColorRect>("%BackgroundRect");
    }



}

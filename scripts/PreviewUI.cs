using Godot;
using System;

public class PreviewUI : Control
{
    private ColorRect _backgroundRect;
    private Label _textLabel;

    [Export]
    private Vector2 _baseScale = new Vector2(1, 1);



    public override void _Ready()
    {
        _backgroundRect = GetNode<ColorRect>("%BackgroundRect");
        _textLabel = GetNode<Label>("%Text");
    }
    


    private void UpdateTextSize(int size)
    {
        DynamicFont font = (DynamicFont)_textLabel.Get("custom_fonts/font");
        font.Size = size;
    }
    private void UpdateTextContent(TextData textData)
    {
        _textLabel.Text = textData.Incipit + "\n\n" + textData.Main + "\n" + textData.Closing;
    }


    public void UpdateSize(Vector2 ratioVec)
    {
        _backgroundRect.RectScale = _baseScale * ratioVec / Math.Max(ratioVec.x, ratioVec.y);
    }
    public void UpdateColor(Color color)
    {
        _backgroundRect.Color = color;
    }
    public void UpdateTextColor(Color color)
    {
        _textLabel.Set("custom_colors/font_color", color);
    }
    public void UpdateText(TextData textData)
    {
        UpdateTextSize(textData.Size);
        UpdateTextContent(textData);
    }
    public void UpdateZoom(bool zoomIn)
    {
        if (zoomIn)
            _backgroundRect.RectScale *= 1.25f;
        else
            _backgroundRect.RectScale /= 1.25f;
    }

}

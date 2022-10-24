using Godot;
using System;

public class PreviewUI : Control
{
    private ColorRect _backgroundRect;
    private RichTextLabel _textLabel;
    private TextData _textData = new TextData( "", "", "",30, new Color(0f, 0f, 0f), new Color(0f, 0f, 0f));
    private float _maxLabelHeight;
    private Vector2 _maxSize;
    private Vector2 _maxScale;
    private Vector2 _minSize = new Vector2(200, 200);
    private Vector2 _startSize = new Vector2(400, 400);
    private string _symbols = "";
    private string[] _symBbCode = new string[] {"[color=#000000]", "[/color]"};

    [Export]
    private Vector2 _baseScale = new Vector2(1, 1);



    public override void _Ready()
    {
        _backgroundRect = GetNode<ColorRect>("%BackgroundRect");
        _textLabel = GetNode<RichTextLabel>("%Text");
        _maxLabelHeight = _textLabel.RectSize[1];
    }



    private void UpdateTextSize()
    {
        DynamicFont font = (DynamicFont)_textLabel.Get("custom_fonts/normal_font");
        font.Size = _textData.Size;
    }
    private void UpdateTextContent()
    {

        string mainText = _textData.Incipit + "\n\n" + _textData.Main;
        string closing = "\n" + _textData.Closing;

        DynamicFont font = (DynamicFont)_textLabel.Get("custom_fonts/normal_font");
        Vector2 mainTextSize = font.GetWordwrapStringSize(mainText,_textLabel.RectSize[0]);
        float heightLeft = _maxLabelHeight - mainTextSize[1] - font.GetCharSize('I')[1];
        // float fontHeight = font.GetCharSize('~')[1];
        float fontHeight = font.GetHeight();
        int lineToFill = (int)(heightLeft / fontHeight);

        
        string BbSymbols = _symBbCode[0]; 
        _symbols = "";

        for (int i = 0; i < lineToFill; i++)
        {
            _symbols += "\n~"; 
        }
        BbSymbols += _symbols + _symBbCode[1];  

        _textLabel.BbcodeText = mainText + BbSymbols + closing;
    }



    public void UpdateMaxSize(Vector2 size)
    {
        _maxSize = size;

        Vector2 factorVec = _maxSize / _startSize;
        _maxScale = Math.Min(factorVec.x, factorVec.y) * Vector2.One;
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
        _textData.Color = color;
        _textLabel.Set("custom_colors/default_color", color);
    }
    public void UpdateSymbolColor(Color color)
    {
        _textData.SymbolColor = color;

        _symBbCode[0] = $"[color=#{color.ToHtml(false)}]";

        string BbSymbols = _symBbCode[0] + _symbols + _symBbCode[1];  

        _textLabel.BbcodeText =  _textData.Incipit + "\n\n" + _textData.Main + BbSymbols + "\n" + _textData.Closing;
    }
    public void UpdateText(TextData textData)
    {
        _textData = textData;
        UpdateTextSize();
        UpdateTextContent();
    }
    public void UpdateZoom(bool zoomIn, bool maxime)
    {
        if (maxime)
        {
            _backgroundRect.RectScale = _maxScale;
            return;
        }

        Vector2 finalScale;
        Vector2 finalSize;

        if (zoomIn)
        {
            finalScale = _backgroundRect.RectScale * 1.20f;
            finalSize = finalScale * _backgroundRect.RectSize;

            if (finalSize.x < _maxSize.x && finalSize.y < _maxSize.y)
                _backgroundRect.RectScale = finalScale;
        }
        else
        {
            finalScale = _backgroundRect.RectScale / 1.20f;
            finalSize = finalScale * _backgroundRect.RectSize;

            if (finalSize.y > _minSize.x && finalSize.y > _minSize.y)
                _backgroundRect.RectScale = finalScale;
        }
    }

}

using Godot;
using System;

public class PreviewUI : Control
{


    private AnimationPlayer _animationPlayer;
    private ColorRect _backgroundRect;
    private RichTextLabel _textLabel;
    private TextData _textData = new TextData("", "", "", 30, new Color(0f, 0f, 0f), new Color(0f, 0f, 0f));
    private Vector2 _maxSize;
    private Vector2 _maxScale;
    private Vector2 _minSize = new Vector2(400, 400);
    private Vector2 _startSize = new Vector2(2160, 2160);//new Vector2(1080, 1080); //
    private string _symbols = "";
    private string[] _symBbCode = new string[] { "[color=#000000]", "[/color]" };


    public override void _Ready()
    {
        _backgroundRect = GetNode<ColorRect>("%BackgroundRect");
        _animationPlayer = GetNode<AnimationPlayer>("%AnimationPlayer");
        _textLabel = GetNode<RichTextLabel>("%Text");

        DynamicFont font = (DynamicFont)_textLabel.Get("custom_fonts/normal_font");
        _textLabel.Set("custom_fonts/bold_font", font.Duplicate());
    }



    private void UpdateTextSize()
    {
        DynamicFont font = (DynamicFont)_textLabel.Get("custom_fonts/normal_font");
        font.Size = _textData.Size;
        DynamicFont boldFont = (DynamicFont)_textLabel.Get("custom_fonts/bold_font");
        boldFont.Size = _textData.Size;
    }
    private void UpdateTextContent()
    {
        string mainText = _textData.Incipit + "\n\n" + _textData.Main + " " + char.ConvertFromUtf32(0x00002588);
        string closing = "\n" + _textData.Closing;

        DynamicFont font = (DynamicFont)_textLabel.Get("custom_fonts/normal_font");
        Vector2 mainTextSize = font.GetWordwrapStringSize(mainText, _textLabel.RectSize[0]);
        float heightLeft = _textLabel.RectSize[1] - mainTextSize[1] - font.GetCharSize('I')[1];
        // float fontHeight = font.GetCharSize('~')[1];
        float fontHeight = font.GetHeight();
        int lineToFill = (int)(heightLeft / fontHeight);

        DynamicFont boldFont = (DynamicFont)_textLabel.Get("custom_fonts/bold_font");

        if (boldFont.FontData != null)
        {
            _symBbCode[0] = $"[b][color=#{_textData.SymbolColor.ToHtml(false)}]";
            _symBbCode[1] = $"[/color][/b]";
        }
        else
        {
            _symBbCode[0] = $"[color=#{_textData.SymbolColor.ToHtml(false)}]";
            _symBbCode[1] = $"[/color]";
        }


        string BbSymbols = _symBbCode[0];
        _symbols = "";

        for (int i = 0; i < lineToFill; i++)
        {
            _symbols += "\n~";
        }

        BbSymbols += _symbols + _symBbCode[1];

        _textLabel.BbcodeText = mainText + BbSymbols + closing;
    }


    public void UpdateOnlyMain(string main, bool blockOn)
    {
        _textData.Main = main;

        string mainText = _textData.Incipit + "\n\n" + main;
        string closing = "\n" + _textData.Closing;

        if (blockOn)
            mainText += " " + char.ConvertFromUtf32(0x00002588);

        _textLabel.BbcodeText = mainText + _symBbCode[0] + _symbols + _symBbCode[1] + closing;
    }
    public void UpdateOnlyBlock(bool blockOn)
    {
        string mainText = _textData.Incipit + "\n\n" + _textData.Main;
        string closing = "\n" + _textData.Closing;

        if (blockOn)
            mainText += " " + char.ConvertFromUtf32(0x00002588);

        _textLabel.BbcodeText = mainText + _symBbCode[0] + _symbols + _symBbCode[1] + closing;
    }
    private void UpdateLabelMargin(Vector2 size)
    {
        switch (size.x)
        {
            case 1080:
                _textLabel.MarginBottom = 30;
                _textLabel.MarginLeft = 30;
                _textLabel.MarginTop = 30;
                break;
            case 2160:
                _textLabel.MarginBottom = 60;
                _textLabel.MarginLeft = 60;
                _textLabel.MarginTop = 60;
                break;
        }
    }
    public void UpdateMaxSize(Vector2 maxsize)
    {
        _maxSize = maxsize;

        Vector2 factorVec = _maxSize / _startSize;
        _maxScale = Math.Min(factorVec.x, factorVec.y) * Vector2.One;
        _backgroundRect.RectScale = _maxScale;
    }
    public void UpdateSize(Vector2 size)
    {
        Vector2 center = _backgroundRect.GetRect().GetCenter();
        _backgroundRect.RectPivotOffset = size / 2;
        _backgroundRect.RectSize = size;
        _startSize = size;

        _backgroundRect.RectPosition = center - _backgroundRect.RectSize / 2;
        UpdateLabelMargin(size);

        // Vector2 factorVec = _maxSize / _startSize;
        // _maxScale = Math.Min(factorVec.x, factorVec.y) * Vector2.One;

        // _backgroundRect.RectScale = _maxScale;  
        // _backgroundRect.RectScale = _baseScale * ratioVec / Math.Max(ratioVec.x, ratioVec.y);
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

        DynamicFont boldFont = (DynamicFont)_textLabel.Get("custom_fonts/bold_font");
        if (boldFont.FontData != null)
        {
            _symBbCode[0] = $"[b][color=#{color.ToHtml(false)}]";
            _symBbCode[1] = $"[/color][/b]";
        }
        else
        {
            _symBbCode[0] = $"[color=#{color.ToHtml(false)}]";
            _symBbCode[1] = $"[/color]";
        }

        string BbSymbols = _symBbCode[0] + _symbols + _symBbCode[1];

        _textLabel.BbcodeText = _textData.Incipit + "\n\n" + _textData.Main + " " + char.ConvertFromUtf32(0x00002588) + BbSymbols + "\n" + _textData.Closing;
    }
    public void UpdateSpacing(int value, SpacingContainer.Spacing mode)
    {
        DynamicFont font = (DynamicFont)_textLabel.Get("custom_fonts/normal_font");
        DynamicFont boldFont = (DynamicFont)_textLabel.Get("custom_fonts/bold_font");

        switch (mode)
        {
            case SpacingContainer.Spacing.TOP:
                font.ExtraSpacingTop = value;
                boldFont.ExtraSpacingTop = value;
                break;

            case SpacingContainer.Spacing.BOTTOM:
                font.ExtraSpacingBottom = value;
                boldFont.ExtraSpacingBottom = value;
                break;

            case SpacingContainer.Spacing.CHAR:
                font.ExtraSpacingChar = value;
                boldFont.ExtraSpacingChar = value;
                break;

            case SpacingContainer.Spacing.SPACE:
                font.ExtraSpacingSpace = value;
                boldFont.ExtraSpacingSpace = value;
                break;

            default:
                return;
        }
    }
    public void UpdateText(TextData textData)
    {
        _textData = textData;
        UpdateTextSize();
        UpdateTextContent();
    }
    public void UpdateFont(string path)
    {
        DynamicFont font = (DynamicFont)_textLabel.Get("custom_fonts/normal_font");
        font.FontData = ResourceLoader.Load<DynamicFontData>(path);
    }
    public void UpdateBoldFont(string path)
    {
        DynamicFont boldFont = (DynamicFont)_textLabel.Get("custom_fonts/bold_font");
        boldFont.FontData = ResourceLoader.Load<DynamicFontData>(path);
        UpdateTextContent();
    }
    public void SavePNG(string path, bool shrink2)
    {

        Image img = GetViewport().GetTexture().GetData();
        img.FlipY();

        Rect2 frameRect = _backgroundRect.GetRect();
        Image frame = img.GetRect(frameRect);
        //frame.Resize((int)_startSize[0],(int)_startSize[1]);
        if (shrink2)
        {
            frame.ShrinkX2();
        }
        frame.SavePng(path);
    }
    public void BlinkAnimation(bool blinkingOn)
    {
        if (blinkingOn)
            _animationPlayer.Play("BlinkingBlock");
        else
            _animationPlayer.Stop();
        UpdateOnlyBlock(true);

    }
}

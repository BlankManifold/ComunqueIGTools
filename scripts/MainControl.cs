using Godot;
using System;

public class MainControl : Control
{

    private ToolsUI _toolsUI;
    private PreviewUI _previewUI;

    public override void _Ready()
    {
        _toolsUI = GetNode<ToolsUI>("%ToolsUI");
        _previewUI = GetNode<PreviewUI>("%PreviewUI");
        _previewUI.UpdateMaxSize(GetNode<ViewportContainer>("%PreviewViewportContainer").RectSize);

        _toolsUI.ConnectTool(this, Globals.Tool.SIZE, nameof(on_ToolsUI_SizeChanged));
        _toolsUI.ConnectTool(this, Globals.Tool.BGCOLOR, nameof(on_ToolsUI_BGColorSelected));
        _toolsUI.ConnectTool(this, Globals.Tool.TEXT, nameof(on_ToolsUI_UpdateText));
        _toolsUI.ConnectTool(this, Globals.Tool.FONT, nameof(on_ToolsUI_FontSelected));
        _toolsUI.ConnectTool(this, Globals.Tool.BOLDFONT, nameof(on_ToolsUI_BoldFontSelected));
        _toolsUI.ConnectTool(this, Globals.Tool.FONTCOLOR, nameof(on_ToolsUI_FontColorSelected));
        _toolsUI.ConnectTool(this, Globals.Tool.SYMCOLOR, nameof(on_ToolsUI_SymbolColorSelected));
        _toolsUI.ConnectTool(this, Globals.Tool.SPACING, nameof(on_ToolsUI_SpacingChanged));
        _toolsUI.ConnectTool(this, Globals.Tool.ZOOM, nameof(on_ToolsUI_Zoom));
        _toolsUI.ConnectTool(this, Globals.Tool.SAVE, nameof(on_ToolsUI_Save));
    }

    public void on_ToolsUI_SizeChanged(Vector2 ratioVec)
    {
        _previewUI.UpdateSize(ratioVec);
    }
    public void on_ToolsUI_BGColorSelected(Color color)
    {
        _previewUI.UpdateColor(color);
    }
    public void on_ToolsUI_FontSelected(string path)
    {
        _previewUI.UpdateFont(path);
    }
    public void on_ToolsUI_BoldFontSelected(string path)
    {
        _previewUI.UpdateBoldFont(path);
    }
    public void on_ToolsUI_FontColorSelected(Color color)
    {
        _previewUI.UpdateTextColor(color);
    }
    public void on_ToolsUI_SymbolColorSelected(Color color)
    {
        _previewUI.UpdateSymbolColor(color);
    }
    public void on_ToolsUI_SpacingChanged(int value, SpacingContainer.Spacing mode)
    {
        _previewUI.UpdateSpacing(value, mode);
    }
    public void on_ToolsUI_UpdateText(TextData textData)
    {
        _previewUI.UpdateText(textData);
    }
    public void on_ToolsUI_Zoom(bool zoomIn, bool maxime)
    {
        _previewUI.UpdateZoom(zoomIn, maxime);
    }
    public void on_ToolsUI_Save(string path)
    {
        _previewUI.SavePNG(path);
    }


}

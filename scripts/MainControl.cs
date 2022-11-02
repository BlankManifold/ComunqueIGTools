using Godot;

public class MainControl : Control
{

    private ToolsUI _toolsUI;
    private PreviewUI _previewUI;
    private Camera2D _camera;
    private Vector2 _maxZoom = new Vector2(0.05f, 10f);

    public override void _Ready()
    {
        _toolsUI = GetNode<ToolsUI>("%ToolsUI");
        _previewUI = GetNode<PreviewUI>("%PreviewUI");
        _previewUI.UpdateMaxSize(GetNode<ViewportContainer>("%PreviewViewportContainer").RectSize);
        _camera = GetNode<Camera2D>("%Camera2D");


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
        _toolsUI.ConnectTool(this, Globals.Tool.BLINKING, nameof(on_ToolsUI_BlinkingPressed));
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
        if (maxime)
            return;
            
        Vector2 zoom = _camera.Zoom;
        if (zoomIn)
            zoom /= 1.1f; 
        else
            zoom *= 1.1f; 
        
        if (zoom[0] >= _maxZoom[0] && zoom[1] <= _maxZoom[1])
            _camera.Zoom = zoom;
    }
    public void on_ToolsUI_Save(string path, bool shrink2)
    {
        _previewUI.SavePNG(path, shrink2);
    }
    public void on_ToolsUI_BlinkingPressed(bool blinkingOn)
    {
        _previewUI.BlinkAnimation(blinkingOn);
    }


}

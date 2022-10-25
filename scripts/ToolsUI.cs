using Godot;

public class TextData : Godot.Object
{
    public string Incipit;
    public string Closing;
    public string Main;
    public int Size;
    public Color Color;
    public Color SymbolColor;

    public TextData(string incipit, string main, string closing, int size, Color color, Color symbolColor)
    {
        Incipit = incipit;
        Closing = closing;
        Main = main;
        Size = size;
        Color = color;
        SymbolColor = symbolColor;
    }
}
public class ToolsUI : Control
{
    public enum FileDialogMode
    {
        TXT, PALETTE
    }

    [Export]
    private Godot.Collections.Dictionary<string, Vector2> _sizeDict = new Godot.Collections.Dictionary<string, Vector2>()
    {
        {"1:1 (1080x1080)", new Vector2(1080,1080)},
        {"9:16 (1080x1920)", new Vector2(1080,1920)},
    };



    private OptionButton _sizeSelection;
    private OptionButton _incipitSelection;
    private OptionButton _closingSelection;
    private OptionButton _templateSelection;
    private OptionButton _paletteSelection;
    private SpacingContainer _spacingContainer;


    private FileDialog _fileDialog;
    private FileDialogMode _fileDialogMode;
    private PopupPanel _templateNamePanel;
    private TextEditor _textEditor;

    private TextData _textData = new TextData("", "", "", 30, new Color(0f, 0f, 0f), new Color(0f, 0f, 0f));
    private Color _bgColor = new Color(1f, 1f, 1f);



    [Signal]
    delegate void SizeSelected(Vector2 size);
    [Signal]
    delegate void BGColorSelected(Color color);
    [Signal]
    delegate void FontColorSelected(Color color);
    [Signal]
    delegate void SymbolColorSelected(Color color);
    [Signal]
    delegate void FontSizeChanged(Color color);
    [Signal]
    delegate void SpacingChanged(int value, SpacingContainer.Spacing mode);
    [Signal]
    delegate void ZoomChanged(bool zoomIn, bool maxime = false);
    [Signal]
    delegate void UpdateText(TextData textData);
    [Signal]
    delegate void UpdatePalette(string path);




    public override void _Ready()
    {
        _sizeSelection = GetNode<OptionButton>("%FrameSizeSelection");
        _incipitSelection = GetNode<OptionButton>("%IncipitSelection");
        _closingSelection = GetNode<OptionButton>("%ClosingSelection");
        _templateSelection = GetNode<OptionButton>("%TemplateSelection");
        _paletteSelection = GetNode<OptionButton>("%PaletteSelection");
        _spacingContainer = GetNode<SpacingContainer>("%SpacingContainer");

        _templateNamePanel = GetNode<PopupPanel>("%TemplateNamePanel");
        _textEditor = GetNode<TextEditor>("%TextEditor");
        _fileDialog = GetNode<FileDialog>("%FileDialog");
        _fileDialog.CurrentDir = OS.GetSystemDir(OS.SystemDir.Desktop);

        GetNode<SpinBox>("%FontSizeSpinBox").Value = _textData.Size;

        LoadResource(Globals.PATHS.PALETTE, Globals.PATHS.PALETTE_DICT, _paletteSelection);
        LoadResource(Globals.PATHS.TEMPLATE, Globals.PATHS.TEMPLATE_DICT, _templateSelection);
    }



    public void ConnectTool(Node nodeToConnect, Globals.Tool tool, string targetMethod)
    {
        switch (tool)
        {
            case Globals.Tool.SIZE:
                Connect(nameof(SizeSelected), nodeToConnect, targetMethod);
                break;
            case Globals.Tool.BGCOLOR:
                Connect(nameof(BGColorSelected), nodeToConnect, targetMethod);
                break;
            case Globals.Tool.TEXT:
                Connect(nameof(UpdateText), nodeToConnect, targetMethod);
                break;
            case Globals.Tool.INCIPIT:
                Connect(nameof(UpdateText), nodeToConnect, targetMethod);
                break;
            case Globals.Tool.FONTCOLOR:
                Connect(nameof(FontColorSelected), nodeToConnect, targetMethod);
                break;
            case Globals.Tool.SYMCOLOR:
                Connect(nameof(SymbolColorSelected), nodeToConnect, targetMethod);
                break;
            case Globals.Tool.ZOOM:
                Connect(nameof(ZoomChanged), nodeToConnect, targetMethod);
                break;
            case Globals.Tool.SPACING:
                Connect(nameof(SpacingChanged), nodeToConnect, targetMethod);
                break;
        }
    }



    private string GetTextFromFile(string path)
    {
        string text;

        File file = new File();
        file.Open(path, File.ModeFlags.Read);

        text = file.GetAsText();

        file.Close();

        return text;
    }
    private void UpdateTextData(string text, Globals.Text textMode)
    {
        switch (textMode)
        {
            case Globals.Text.MAIN:
                _textData.Main = text;
                break;

            case Globals.Text.INCIPIT:
                _textData.Incipit = text;
                break;

            case Globals.Text.CLOSING:
                _textData.Closing = text;
                break;
        }
    }
    private void UpdateAllText(string text, Globals.Text textMode)
    {
        UpdateTextData(text, textMode);
        _textEditor.UpdateTextEdit(_textData.Main, textMode);

        EmitSignal(nameof(UpdateText), _textData);
    }
    private void UpdateFileDialogFilters(string filter)
    {
        _fileDialog.Filters = new string[] { filter };
    }
    private void AddPalette(string path, string fileName)
    {
        Directory dir = new Directory();
        string resPath = Globals.PATHS.PALETTE + "/" + fileName;

        dir.Copy(path, resPath);

        Globals.PATHS.PALETTE_DICT[fileName] = resPath;
        _paletteSelection.AddItem(fileName);
    }
    private void LoadResource(string path, Godot.Collections.Dictionary<string, string> dict, OptionButton selectionButton)
    {
        Directory dir = new Directory();
        Error err = dir.Open(path);
        dir.ListDirBegin(true, true);

        if (err == Error.Ok)
        {
            string name = dir.GetNext();
            while (name != "")
            {
                dict[name] = path + "/" + name;
                selectionButton.AddItem(name);
                name = dir.GetNext();
            }
        }
    }
    private Vector2 GetFrameSize()
    {
        return _sizeDict[_sizeSelection.Text];
    }
    
    private void UpdateSizeSpinBox()
    {
        GetNode<SpinBox>("%FontSizeSpinBox").Value = _textData.Size;
    }


    public void _on_FrameSizeSelection_item_selected(int idx)
    {
        EmitSignal(nameof(SizeSelected), _sizeDict[_sizeSelection.GetItemText(idx)]);
    }
    public void _on_BackgroundColorSelection_Selected(Color color)
    {
        _bgColor = color;
        EmitSignal(nameof(BGColorSelected), color);
    }
    public void _on_FontColorSelection_Selected(Color color)
    {
        _textData.Color = color;
        EmitSignal(nameof(FontColorSelected), color);
    }
    public void _on_SymbolColorSelection_Selected(Color color)
    {
        _textData.SymbolColor = color;
        EmitSignal(nameof(SymbolColorSelected), color);
    }
    public void _on_FontSize_value_changed(float value)
    {
        _textData.Size = (int)value;
    }
    public void _on_TextEditor_UpdateText(string text)
    {
        UpdateAllText(text, Globals.Text.MAIN);
    }
    public void _on_TextEditor_LoadPressed()
    {
        _fileDialog.Popup_();
        UpdateFileDialogFilters(Globals.RESOURCE_EXT.TXT);
        _fileDialogMode = FileDialogMode.TXT;

    }
    public void _on_LoadPalette_button_down()
    {
        _fileDialog.Popup_();
        UpdateFileDialogFilters(Globals.RESOURCE_EXT.PALETTE);
        _fileDialogMode = FileDialogMode.PALETTE;
    }
    public void _on_FileDialog_file_selected(string path)
    {
        switch (_fileDialogMode)
        {
            case FileDialogMode.TXT:
                string text = GetTextFromFile(path);
                UpdateAllText(text, Globals.Text.MAIN);
                break;
            case FileDialogMode.PALETTE:
                AddPalette(path, _fileDialog.CurrentFile);
                EmitSignal(nameof(UpdatePalette), path);
                break;
        }
    }
    public void _on_IncipitSelection_item_selected(int idx)
    {
        string text = _incipitSelection.GetItemText(idx);
        UpdateAllText(text, Globals.Text.INCIPIT);
    }
    public void _on_ClosingSelection_item_selected(int idx)
    {
        string text = _closingSelection.GetItemText(idx);
        UpdateAllText(text, Globals.Text.CLOSING);
    }
    public void _on_ZoomButtons_Changed(bool zoomIn, bool maxime)
    {
        EmitSignal(nameof(ZoomChanged), zoomIn, maxime);
    }
    public void _on_TemplateSelection_item_selected(int idx)
    {
        string name = _templateSelection.GetItemText(idx);
        if (name == "")
        {
            return;
        }

        Resource templateResource = ResourceLoader.Load(Globals.PATHS.TEMPLATE_DICT[name]);

        Vector2 size = (Vector2)templateResource.Get("size");
        string incipit = (string)templateResource.Get("incipit");
        string closing = (string)templateResource.Get("closing");
        int fontSize = (int)templateResource.Get("fontSize");
        _bgColor = (Color)templateResource.Get("bgColor");
        Color symbolColor = (Color)templateResource.Get("symbolColor");
        Color fontColor = (Color)templateResource.Get("fontColor");
        Godot.Collections.Array spacing = (Godot.Collections.Array)templateResource.Get("spacing");

        EmitSignal(nameof(SizeSelected), size);
        EmitSignal(nameof(BGColorSelected), _bgColor);
        EmitSignal(nameof(FontColorSelected), fontColor);
        EmitSignal(nameof(SymbolColorSelected), symbolColor);
        EmitSignal(nameof(SpacingChanged), spacing[0], SpacingContainer.Spacing.SPACE);
        EmitSignal(nameof(SpacingChanged), spacing[1], SpacingContainer.Spacing.CHAR);
        EmitSignal(nameof(SpacingChanged), spacing[2], SpacingContainer.Spacing.TOP);
        EmitSignal(nameof(SpacingChanged), spacing[3], SpacingContainer.Spacing.BOTTOM);

        _textData.Size = fontSize;
        UpdateSizeSpinBox();
        UpdateAllText(closing, Globals.Text.CLOSING);
        UpdateAllText(incipit, Globals.Text.INCIPIT);
    }
    public void _on_PaletteSelection_item_selected(int idx)
    {
        string name = _paletteSelection.GetItemText(idx);

        if (_paletteSelection.GetItemText(idx) == "")
            return;


        EmitSignal(nameof(UpdatePalette), Globals.PATHS.PALETTE_DICT[name]);
    }
    public void _on_CreateTemplate_button_down()
    {
        _templateNamePanel.Popup_();
    }
    public void _on_TemeplateName_text_entered(string name)
    {
        _templateNamePanel.Hide();

        GDScript templateResourceScript = (GDScript)GD.Load("res://scripts/TemplateResource.gd");
        Resource templateResource = (Resource)templateResourceScript.New();

        templateResource.Call("init", 0, name + ".tres", GetFrameSize(), _bgColor, _textData.Incipit, _textData.Closing, null,
                             _textData.Size, _textData.Color, _textData.SymbolColor, _spacingContainer.GetSpacing());

        string filePath = Globals.PATHS.TEMPLATE + "/" + name + ".tres";
        Godot.Error err = ResourceSaver.Save(filePath, templateResource);
        Globals.PATHS.TEMPLATE_DICT[name] = filePath;
        _templateSelection.AddItem(name);
    }
    public void _on_SpacingContainer_SpacingChanged(int value, SpacingContainer.Spacing mode)
    {
        EmitSignal(nameof(SpacingChanged), value, mode);
    }
    public void _on_TemplateSelection_item_focused(int idx)
    {

    }
}

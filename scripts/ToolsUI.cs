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
        TXT, PALETTE, SAVE_PNG, TXTS, FONT, BOLDFONT
    }

    [Export]
    private Godot.Collections.Dictionary<string, Vector2> _sizeDict = new Godot.Collections.Dictionary<string, Vector2>()
    {
        {"1:1 (1080x1080)", new Vector2(1080,1080)},
        {"1:1 (2160x2160)", new Vector2(2160,2160)},
        {"9:16 (1080x1920)", new Vector2(1080,1920)},
    };



    private OptionButton _sizeSelection;
    private OptionButton _incipitSelection;
    private OptionButton _closingSelection;
    private OptionButton _templateSelection;
    private int _templateFocused = -1;
    private OptionButton _paletteSelection;
    private OptionButton _fontSelection;
    private OptionButton _boldfontSelection;
    private SpacingContainer _spacingContainer;


    private FileDialog _fileDialog;
    private FileDialogMode _fileDialogMode;
    private Label _windowLabel;
    private PopupPanel _templateNamePanel;
    private TextEditor _textEditor;

    private TextData _textData = new TextData("", "", "", 30, new Color(0f, 0f, 0f), new Color(0f, 0f, 0f));
    private Color _bgColor = new Color(1f, 1f, 1f);
    private Godot.Collections.Array<string> _mainTexts = new Godot.Collections.Array<string>() { };
    private int _index = 0;
    private bool _shrink2 = false;



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
    [Signal]
    delegate void FontSelected(string path);
    [Signal]
    delegate void BoldFontSelected(string path);
    [Signal]
    delegate void Save(string path, bool shrink2);
    [Signal]
    delegate void BlinkingPressed(bool blinkingOn);




    public override void _Ready()
    {
        _sizeSelection = GetNode<OptionButton>("%FrameSizeSelection");
        _fontSelection = GetNode<OptionButton>("%FontSelection");
        _boldfontSelection = GetNode<OptionButton>("%BoldFontSelection");
        _incipitSelection = GetNode<OptionButton>("%IncipitSelection");
        _closingSelection = GetNode<OptionButton>("%ClosingSelection");
        _templateSelection = GetNode<OptionButton>("%TemplateSelection");
        _paletteSelection = GetNode<OptionButton>("%PaletteSelection");
        _spacingContainer = GetNode<SpacingContainer>("%SpacingContainer");

        _templateNamePanel = GetNode<PopupPanel>("%TemplateNamePanel");
        _windowLabel = GetNode<Label>("%WindowLabel");
        _textEditor = GetNode<TextEditor>("%TextEditor");
        _fileDialog = GetNode<FileDialog>("%FileDialog");
        _fileDialog.CurrentDir = OS.GetSystemDir(OS.SystemDir.Desktop);

        GetNode<SpinBox>("%FontSizeSpinBox").Value = _textData.Size;

        LoadResource(Globals.PATHS.PALETTE, Globals.PATHS.PALETTE_DICT, _paletteSelection);
        LoadResource(Globals.PATHS.FONT, Globals.PATHS.FONT_DICT, _fontSelection);
        LoadResource(Globals.PATHS.BOLDFONT, Globals.PATHS.BOLDFONT_DICT, _boldfontSelection);
        LoadResource(Globals.PATHS.TEMPLATE, Globals.PATHS.TEMPLATE_DICT, _templateSelection);
    }
    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("DeleteResource"))
        {
            if (_templateFocused > 0)
            {
                Directory file = new Directory();
                string name = _templateSelection.GetItemText(_templateFocused);

                file.Remove(Globals.PATHS.TEMPLATE_DICT[name]);
                Globals.PATHS.TEMPLATE_DICT.Remove(name);
                _templateSelection.RemoveItem(_templateFocused);
            }
        }

        if (@event.IsActionPressed("SwapLeft") && _mainTexts.Count > 1)
        {
            int count = _mainTexts.Count;
            _index = (_index + 1) % count;
            UpdateAllText(_mainTexts[_index], Globals.Text.MAIN);
            UpdateWindowLabel();
        }
        @event.Dispose();
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
            case Globals.Tool.FONT:
                Connect(nameof(FontSelected), nodeToConnect, targetMethod);
                break;
            case Globals.Tool.BOLDFONT:
                Connect(nameof(BoldFontSelected), nodeToConnect, targetMethod);
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
            case Globals.Tool.SAVE:
                Connect(nameof(Save), nodeToConnect, targetMethod);
                break;
            case Globals.Tool.BLINKING:
                Connect(nameof(BlinkingPressed), nodeToConnect, targetMethod);
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
    private void UpdateFileDialogFilters(FileDialogMode mode)
    {
        _fileDialogMode = mode;


        switch (mode)
        {
            case FileDialogMode.TXT:
                _fileDialog.CurrentFile = "";
                _fileDialog.Mode = FileDialog.ModeEnum.OpenFile;
                _fileDialog.WindowTitle = "Open a comunque-text file";
                _fileDialog.Filters = new string[] { Globals.RESOURCE_EXT.TXT };
                break;
            case FileDialogMode.TXTS:
                _fileDialog.Mode = FileDialog.ModeEnum.OpenFiles;
                _fileDialog.WindowTitle = "Open one or more comunque-text files";
                _fileDialog.Filters = new string[] { Globals.RESOURCE_EXT.TXT };
                break;
            case FileDialogMode.PALETTE:
                _fileDialog.CurrentFile = "";
                _fileDialog.Mode = FileDialog.ModeEnum.OpenFile;
                _fileDialog.WindowTitle = "Open a comunque-palette file";
                _fileDialog.Filters = new string[] { Globals.RESOURCE_EXT.PALETTE };
                break;
            case FileDialogMode.SAVE_PNG:
                _fileDialog.CurrentFile = "";
                _fileDialog.Mode = FileDialog.ModeEnum.SaveFile;
                _fileDialog.WindowTitle = "Save the comunque as a PNG";
                _fileDialog.Filters = new string[] { Globals.RESOURCE_EXT.PNG };
                break;
            case FileDialogMode.FONT:
            case FileDialogMode.BOLDFONT:
                _fileDialog.CurrentFile = "";
                _fileDialog.Mode = FileDialog.ModeEnum.OpenFile;
                _fileDialog.WindowTitle = "Load a font file";
                _fileDialog.Filters = new string[] { Globals.RESOURCE_EXT.FONT };
                break;
        }

    }
    private void AddPalette(string path, string fileName)
    {
        Directory dir = new Directory();
        string resPath = Globals.PATHS.PALETTE + "/" + fileName;

        dir.Copy(path, resPath);

        Globals.PATHS.PALETTE_DICT[fileName] = resPath;
        _paletteSelection.AddItem(fileName);
    }
    private void AddFont(string path, string fileName)
    {
        Directory dir = new Directory();
        string resPath = Globals.PATHS.FONT + "/" + fileName;

        dir.Copy(path, resPath);

        Globals.PATHS.FONT_DICT[fileName] = resPath;
        _fontSelection.AddItem(fileName);
    }
    private void AddBoldFont(string path, string fileName)
    {
        Directory dir = new Directory();
        string resPath = Globals.PATHS.BOLDFONT + "/" + fileName;

        dir.Copy(path, resPath);

        Globals.PATHS.BOLDFONT_DICT[fileName] = resPath;
        _boldfontSelection.AddItem(fileName);
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
    private void UpdateFromResource(Resource templateResource)
    {
        string incipit = (string)templateResource.Get("incipit");
        string closing = (string)templateResource.Get("closing");
        int fontSize = (int)templateResource.Get("fontSize");
        _bgColor = (Color)templateResource.Get("bgColor");
        Color symbolColor = (Color)templateResource.Get("symbolColor");
        Color fontColor = (Color)templateResource.Get("fontColor");
        Godot.Collections.Array spacing = (Godot.Collections.Array)templateResource.Get("spacing");

        EmitSignal(nameof(BGColorSelected), _bgColor);
        EmitSignal(nameof(FontColorSelected), fontColor);
        EmitSignal(nameof(SymbolColorSelected), symbolColor);
        EmitSignal(nameof(SpacingChanged), spacing[0], SpacingContainer.Spacing.SPACE);
        EmitSignal(nameof(SpacingChanged), spacing[1], SpacingContainer.Spacing.CHAR);
        EmitSignal(nameof(SpacingChanged), spacing[2], SpacingContainer.Spacing.TOP);
        EmitSignal(nameof(SpacingChanged), spacing[3], SpacingContainer.Spacing.BOTTOM);

        _textData.Size = fontSize;
        UpdateSizeSpinBox();
        _spacingContainer.UpdateSpacing(spacing);
        UpdateAllText(closing, Globals.Text.CLOSING);
        UpdateAllText(incipit, Globals.Text.INCIPIT);
    }
    private void UpdateWindowLabel()
    {
        _windowLabel.Text = $"{_index + 1}/{_mainTexts.Count}";
    }


    ///////////////////////////////////////////////////////////
    // WINDOW FRAME SIZE and ZOOM /////////////////////////////
    ///////////////////////////////////////////////////////////
    public void _on_FrameSizeSelection_item_selected(int idx)
    {
        EmitSignal(nameof(SizeSelected), _sizeDict[_sizeSelection.GetItemText(idx)]);
    }
    public void _on_ZoomButtons_Changed(bool zoomIn, bool maxime)
    {
        EmitSignal(nameof(ZoomChanged), zoomIn, maxime);
    }

    ///////////////////////////////////////////////////////////
    // FONT COLOR  ////////////////////////////////////////////
    ///////////////////////////////////////////////////////////
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


    ///////////////////////////////////////////////////////////
    // FONT  and TEXT  ////////////////////////////////////////
    ///////////////////////////////////////////////////////////
    public void _on_FontSize_value_changed(float value)
    {
        _textData.Size = (int)value;
    }
    public void _on_SpacingContainer_SpacingChanged(int value, SpacingContainer.Spacing mode)
    {
        EmitSignal(nameof(SpacingChanged), value, mode);
    }
    public void _on_LoadFont_button_down()
    {
        UpdateFileDialogFilters(FileDialogMode.FONT);
        _fileDialog.Popup_();
    }
    public void _on_LoadBoldFont_button_down()
    {
        UpdateFileDialogFilters(FileDialogMode.BOLDFONT);
        _fileDialog.Popup_();
    }
    public void _on_TextEditor_UpdateText(string text)
    {
        UpdateAllText(text, Globals.Text.MAIN);
    }
    public void _on_TextEditor_LoadPressed(bool multiple)
    {
        if (!multiple)
            UpdateFileDialogFilters(FileDialogMode.TXT);
        else
            UpdateFileDialogFilters(FileDialogMode.TXTS);

        _fileDialog.Popup_();
    }
    public void _on_FontSelection_item_selected(int idx)
    {
        string name = _fontSelection.GetItemText(idx);

        if (_fontSelection.GetItemText(idx) == "")
            return;
        EmitSignal(nameof(FontSelected), Globals.PATHS.FONT_DICT[name]);
    }
    public void _on_BoldFontSelection_item_selected(int idx)
    {
        string name = _boldfontSelection.GetItemText(idx);

        if (_boldfontSelection.GetItemText(idx) == "")
            return;
        EmitSignal(nameof(BoldFontSelected), Globals.PATHS.FONT_DICT[name]);
    }

    ///////////////////////////////////////////////////////////
    // PALETTE  ///////////////////////////////////////////////
    ///////////////////////////////////////////////////////////
    public void _on_PaletteSelection_item_selected(int idx)
    {
        string name = _paletteSelection.GetItemText(idx);

        if (_paletteSelection.GetItemText(idx) == "")
            return;


        EmitSignal(nameof(UpdatePalette), Globals.PATHS.PALETTE_DICT[name]);
    }
    public void _on_LoadPalette_button_down()
    {
        UpdateFileDialogFilters(FileDialogMode.PALETTE);
        _fileDialog.Popup_();
    }

    ///////////////////////////////////////////////////////////
    // FILE DIALOG  ///////////////////////////////////////////
    ///////////////////////////////////////////////////////////
    public void _on_FileDialog_file_selected(string path)
    {
        switch (_fileDialogMode)
        {
            case FileDialogMode.PALETTE:
                AddPalette(path, _fileDialog.CurrentFile);
                EmitSignal(nameof(UpdatePalette), path);
                break;
            case FileDialogMode.FONT:
                AddFont(path, _fileDialog.CurrentFile);
                EmitSignal(nameof(FontSelected), path);
                break;
            case FileDialogMode.BOLDFONT:
                AddBoldFont(path, _fileDialog.CurrentFile);
                EmitSignal(nameof(BoldFontSelected), path);
                break;
        }
    }
    public void _on_FileDialog_files_selected(Godot.Collections.Array paths)
    {
        switch (_fileDialogMode)
        {
            case FileDialogMode.TXTS:
                string text;
                foreach (string path in paths)
                {
                    text = GetTextFromFile(path);
                    _mainTexts.Add(text);
                }
                UpdateAllText(_mainTexts[0], Globals.Text.MAIN);
                UpdateWindowLabel();
                break;
        }
    }
    public void _on_FileDialog_confirmed()
    {
        if (_fileDialogMode == FileDialogMode.SAVE_PNG)
        {
            EmitSignal(nameof(Save), _fileDialog.CurrentPath, _shrink2);
        }
    }

    ///////////////////////////////////////////////////////////
    // TEXT   /////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////
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

    ///////////////////////////////////////////////////////////
    // TEMPLATE  //////////////////////////////////////////////
    ///////////////////////////////////////////////////////////
    public void _on_TemplateSelection_item_selected(int idx)
    {
        string name = _templateSelection.GetItemText(idx);
        if (name == "")
        {
            return;
        }

        Resource templateResource = ResourceLoader.Load(Globals.PATHS.TEMPLATE_DICT[name]);
        UpdateFromResource(templateResource);
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

        string fileName = name + ".tres";

        templateResource.Call("init", 0, fileName, _bgColor, _textData.Incipit, _textData.Closing, null,
                             _textData.Size, _textData.Color, _textData.SymbolColor, _spacingContainer.GetSpacing());

        string filePath = Globals.PATHS.TEMPLATE + "/" + fileName;
        Godot.Error err = ResourceSaver.Save(filePath, templateResource);
        
        if (!Globals.PATHS.TEMPLATE_DICT.ContainsKey(fileName))
        {
            _templateSelection.AddItem(fileName);
        }

        Globals.PATHS.TEMPLATE_DICT[fileName] = filePath;
    }
    public void _on_TemplateSelection_item_focused(int idx)
    {
        _templateFocused = idx;
    }

    ///////////////////////////////////////////////////////////
    // SAVE  //////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////
    public void _on_Save_button_down()
    {
        UpdateFileDialogFilters(FileDialogMode.SAVE_PNG);
        _fileDialog.Popup_();
    }
    public void _on_Shrink2Box_toggled(bool shrink2On)
    {
        _shrink2 = shrink2On;
    }
    public void _on_BlinkingButton_toggled(bool blinkingOn)
    {
        EmitSignal(nameof(BlinkingPressed), blinkingOn);
    }
}


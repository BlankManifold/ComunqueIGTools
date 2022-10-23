using Godot;

public class TextData : Godot.Object 
{
    public string Incipit;
    public string Closing;
    public string Main;
    public int Size;

    public TextData(string incipit, string main, string closing, int size)
    {
        Incipit = incipit;
        Closing = closing;
        Main = main;
        Size = size;
    }
}
public class ToolsUI : Control
{
    // [Export]
    // private Godot.Collections.Dictionary<Globals.Tool, bool> _toolDict = new Godot.Collections.Dictionary<Globals.Tool, bool>()
    // {
    //     {Globals.Tool.SIZE, false},
    //     {Globals.Tool.TEXT, false},
    //     {Globals.Tool.BGCOLOR, false},
    //     {Globals.Tool.CLOSING, false},
    //     {Globals.Tool.INCIPIT, false},
    //     {Globals.Tool.FONTCOLOR, false},
    //     {Globals.Tool.SYMCOLOR, false},
    //     {Globals.Tool.ZOOM, false}
    // };

    [Export]
    private Godot.Collections.Dictionary<string, Vector2> _sizeDict = new Godot.Collections.Dictionary<string, Vector2>()
    {
        {"1:1 (1080x1080)", new Vector2(1,1)},
        {"9:16 (1080x1920)", new Vector2(9,16)},
    };


    private OptionButton _sizeSelection;
    private OptionButton _incipitSelection;
    private OptionButton _closingSelection;

    private FileDialog _fileDialog;
    private TextEditor _textEditor;

    private TextData _textData = new TextData("", "", "", 30);


    [Signal]
    delegate void SizeSelected(Vector2 ratioVec);
    [Signal]
    delegate void BGColorSelected(Color color);
    [Signal]
    delegate void FontColorSelected(Color color);
    [Signal]
    delegate void FontSizeChanged(Color color);
    [Signal]
    delegate void ZoomChanged();
    [Signal]
    delegate void UpdateText(TextData textData);



    public override void _Ready()
    {
        _sizeSelection = GetNode<OptionButton>("%FrameSizeSelection");
        _incipitSelection = GetNode<OptionButton>("%IncipitSelection");
        _closingSelection = GetNode<OptionButton>("%ClosingSelection");

        _textEditor = GetNode<TextEditor>("%TextEditor");
        _fileDialog = GetNode<FileDialog>("%FileDialog");

        GetNode<SpinBox>("%FontSizeSpinBox").Value = _textData.Size;
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
            case Globals.Tool.ZOOM:
                Connect(nameof(ZoomChanged), nodeToConnect, targetMethod);
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



    public void _on_FrameSizeSelection_item_selected(int idx)
    {
        EmitSignal(nameof(SizeSelected), _sizeDict[_sizeSelection.GetItemText(idx)]);
    }
    public void _on_BackgroundColorSelection_Selected(Color color)
    {
        EmitSignal(nameof(BGColorSelected), color);
    }
    public void _on_FontColorSelection_Selected(Color color)
    {
        EmitSignal(nameof(FontColorSelected), color);
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
    }
    public void _on_FileDialog_file_selected(string path)
    {
        string text = GetTextFromFile(path);
        UpdateAllText(text, Globals.Text.MAIN);
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
    public void _on_ZoomButtons_Changed(bool zoomIn)
    {
        EmitSignal(nameof(ZoomChanged), zoomIn);
    }
}

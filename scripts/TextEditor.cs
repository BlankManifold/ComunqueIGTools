using Godot;

public class TextEditor : VBoxContainer
{
    TextEdit _textEdit;

    [Signal]
    delegate void UpdateText(string text);
    [Signal]
    delegate void LoadPressed();
    

    public override void _Ready()
    {
        _textEdit = GetNode<TextEdit>("TextEdit");
    }


    public void UpdateTextEdit(string text, Globals.Text textMode)
    {
        if (textMode == Globals.Text.MAIN)
            _textEdit.Text = text;
    }



    public void _on_Update_button_down()
    {
        EmitSignal(nameof(UpdateText), _textEdit.Text);
    }
    public void _on_LoadFile_button_down()
    {
        EmitSignal(nameof(LoadPressed));
    }
   

}

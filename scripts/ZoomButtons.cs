using Godot;

public class ZoomButtons : HBoxContainer
{
    [Signal]
    delegate void Changed(bool zoomIn);
        
    public void _on_PlusButton_button_down()
    {
        EmitSignal(nameof(Changed), true);
    }
    public void _on_MinusButton_button_down()
    {
        EmitSignal(nameof(Changed), false);
    }
}

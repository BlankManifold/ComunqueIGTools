using Godot;

public class ZoomButtons : HBoxContainer
{
    [Signal]
    delegate void Changed(bool zoomIn, bool maxime = false);
  

    public void _on_PlusButton_button_down()
    {
        EmitSignal(nameof(Changed), true, false);
    }
    public void _on_MinusButton_button_down()
    {
        EmitSignal(nameof(Changed), false, false);
    }
    public void _on_MaximeButton_button_down()
    {
        EmitSignal(nameof(Changed), true, true);
    }
}

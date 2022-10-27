using Godot;


public class SpacingContainer : VBoxContainer
{
    public enum Spacing
    {
        TOP, BOTTOM, CHAR, SPACE
    }

    [Signal]
    delegate void SpacingChanged(int value, Spacing mode);

    Godot.Collections.Array<int> _spacing = new Godot.Collections.Array<int>() { 0, 0, 0, 0 };

    public Godot.Collections.Array<int> GetSpacing()
    {
        return _spacing;
    }
    public void UpdateSpacing(Godot.Collections.Array spacing)
    {
        GetNode<SpinBox>("%Space").Value = (int)spacing[0];
        GetNode<SpinBox>("%Char").Value = (int)spacing[1];
        GetNode<SpinBox>("%Top").Value = (int)spacing[2];
        GetNode<SpinBox>("%Bottom").Value = (int)spacing[3];
    }

    public void _on_Space_value_changed(float value)
    {
        _spacing[0] = (int)value;
        EmitSignal(nameof(SpacingChanged), (int)value, Spacing.SPACE);
    }
    public void _on_Top_value_changed(float value)
    {
        _spacing[2] = (int)value;
        EmitSignal(nameof(SpacingChanged), (int)value, Spacing.TOP);
    }
    public void _on_Bottom_value_changed(float value)
    {
        _spacing[3] = (int)value;
        EmitSignal(nameof(SpacingChanged), (int)value, Spacing.BOTTOM);
    }
    public void _on_Char_value_changed(float value)
    {
        _spacing[1] = (int)value;
        EmitSignal(nameof(SpacingChanged), (int)value, Spacing.CHAR);
    }

}

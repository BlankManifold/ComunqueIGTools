using Godot;
using System;

public class BackgroundColorSelection : VBoxContainer
{
    [Export]
    private string _title;

    private GridContainer _grid;
    private Button _button;
    private PackedScene _colorIconScene;
    private Godot.Collections.Array<string> _colors = new Godot.Collections.Array<string>()
    {
        "3e4084ff",
        "882b5aff",
        "7c9052ff",
        "000000"
    };


    [Signal]
    delegate void Selected(Color color);
    public override void _Ready()
    {

        _colorIconScene = ResourceLoader.Load<PackedScene>(Globals.PATHS.COLORICON);

        _button = GetNode<Button>("Button");
        _button.Text = _title;

        _grid = GetNode<GridContainer>("ColorGrid");
        _grid.Visible = false;

        foreach (string color in _colors)
        {
            ColorIcon icon = _colorIconScene.Instance<ColorIcon>();
            _grid.AddChild(icon);

            icon.Color = new Color(color);
            icon.Connect("button_down", this, nameof(on_ColorIcon_button_down), new Godot.Collections.Array(){icon.Color});
        }

    }


    public void _on_BackgroundColorSelection_button_down()
    {
        _grid.Visible = !_grid.Visible;
    }
    public void on_ColorIcon_button_down(Color color)
    {
        EmitSignal(nameof(Selected), color);
    }

}
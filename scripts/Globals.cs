namespace Globals
{
    public enum Tool
    {
        SIZE, BGCOLOR, FONTCOLOR, SYMCOLOR, SPACING, INCIPIT, CLOSING, TEXT, ZOOM, TEMPLATE
    }
    public enum Text
    {
        INCIPIT, CLOSING, MAIN
    }

    static class RESOURCE_EXT
    {
        public static readonly string TXT = "*.cmqtxt";
        public static readonly string PALETTE = "*.cmqpal";
    }

    static class PATHS
    {
        public static readonly string COLORICON = "res://scenes/ColorIcon.tscn";
        public static readonly string PALETTE = "user://resources/palette";
        public static readonly string TEMPLATE = "user://resources/template";

        public static Godot.Collections.Dictionary<string, string> TEMPLATE_DICT = new Godot.Collections.Dictionary<string, string>()
        {
        };
        public static Godot.Collections.Dictionary<string, string> PALETTE_DICT = new Godot.Collections.Dictionary<string, string>()
        {
        };
    }
}
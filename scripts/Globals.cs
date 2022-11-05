namespace Globals
{
    public enum Tool
    {
        SIZE,  ZOOM, 
        BGCOLOR, 
        FONT, FONTCOLOR, SYMCOLOR, SPACING, BOLDFONT,
        INCIPIT, CLOSING, TEXT,
        TEMPLATE, 
        SAVE, SAVEBLINKING,
        BLINKING
    }
    public enum Text
    {
        INCIPIT, CLOSING, MAIN
    }

    static class RESOURCE_EXT
    {
        public static readonly string TXT = "*.cmqtxt";
        public static readonly string PALETTE = "*.cmqpal";
        public static readonly string PNG = "*.png";
        public static readonly string FONT = "*.ttf, *.otf";
    }

    static class PATHS
    {
        public static readonly string COLORICON = "res://scenes/ColorIcon.tscn";
        public static readonly string PALETTE = "user://resources/palette";
        public static readonly string FONT = "user://resources/font";
        public static readonly string BOLDFONT = "user://resources/boldfont";
        public static readonly string TEMPLATE = "user://resources/template";

        public static Godot.Collections.Dictionary<string, string> TEMPLATE_DICT = new Godot.Collections.Dictionary<string, string>()
        {
        };
        public static Godot.Collections.Dictionary<string, string> PALETTE_DICT = new Godot.Collections.Dictionary<string, string>()
        {
        };
        public static Godot.Collections.Dictionary<string, string> FONT_DICT = new Godot.Collections.Dictionary<string, string>()
        {
        };
        public static Godot.Collections.Dictionary<string, string> BOLDFONT_DICT = new Godot.Collections.Dictionary<string, string>()
        {
        };
    }
}

extends Resource

class_name TemplateResource

export var id: int
export var name: String
export(float) var frameRatio: float  
export(String, "e comunque...", "o comunque...", "comunque...", "") var incipit: String  
export(String, "-- INSERT --", ":wq", "") var closing: String  
export var font: Font
export var fontSize: int
export var fontColor: Color
export var bgColor: Color
export var symbolColor: Color


func init(_id: int, _name: String, _frameRatio: float, _bgColor: Color, _incipit: String, 
    _closing: String, _font: Font, _fontSize: int,  _fontColor: Color, _symbolColor: Color) -> void:
    id = _id
    name = _name
    frameRatio = _frameRatio
    bgColor = _bgColor
    incipit = _incipit
    closing = _closing
    font = _font
    fontSize = _fontSize
    fontColor = _fontColor
    symbolColor = _symbolColor



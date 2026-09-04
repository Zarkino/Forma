// Type definitions for @uiw/color-convert v1.3.3

// ts2fable 0.9.0
module rec Color_Convert

#nowarn "3390" // disable warnings for invalid XML comments

open Fable.Core

let [<Import("equalColorObjects","@uiw/color-convert")>] equalColorObjects: (ObjectColor -> ObjectColor -> bool) = jsNative
let [<Import("equalColorString","@uiw/color-convert")>] equalColorString: (string -> string -> bool) = jsNative
let [<Import("equalHex","@uiw/color-convert")>] equalHex: (string -> string -> bool) = jsNative
let [<Import("validHex","@uiw/color-convert")>] validHex: (string -> bool) = jsNative
let [<Import("getContrastingColor","@uiw/color-convert")>] getContrastingColor: (U2<string, HsvaColor> -> GetContrastingColor) = jsNative
/// <code lang="js">
/// rgbaToHsva({ r: 255, g: 255, b: 255, a: 1 }) //=> { h: 0, s: 0, v: 100, a: 1 }
/// </code>
let [<Import("rgbaToHsva","@uiw/color-convert")>] rgbaToHsva: (RgbaColor -> HsvaColor) = jsNative
let [<Import("hsvaToHslString","@uiw/color-convert")>] hsvaToHslString: (HsvaColor -> string) = jsNative
let [<Import("hsvaToHsvString","@uiw/color-convert")>] hsvaToHsvString: (HsvaColor -> string) = jsNative
let [<Import("hsvaToHsvaString","@uiw/color-convert")>] hsvaToHsvaString: (HsvaColor -> string) = jsNative
let [<Import("hsvaToHslaString","@uiw/color-convert")>] hsvaToHslaString: (HsvaColor -> string) = jsNative
let [<Import("hslStringToHsla","@uiw/color-convert")>] hslStringToHsla: (string -> HslaColor) = jsNative
let [<Import("hslaStringToHsva","@uiw/color-convert")>] hslaStringToHsva: (string -> HsvaColor) = jsNative
let [<Import("hslStringToHsva","@uiw/color-convert")>] hslStringToHsva: (string -> HsvaColor) = jsNative
let [<Import("hslaToHsva","@uiw/color-convert")>] hslaToHsva: (HslaColor -> HsvaColor) = jsNative
let [<Import("hsvaToHsla","@uiw/color-convert")>] hsvaToHsla: (HsvaColor -> HslaColor) = jsNative
let [<Import("hsvaStringToHsva","@uiw/color-convert")>] hsvaStringToHsva: (string -> HsvaColor) = jsNative
let [<Import("parseHue","@uiw/color-convert")>] parseHue: (string -> (string) option -> float) = jsNative
let [<Import("hsvStringToHsva","@uiw/color-convert")>] hsvStringToHsva: (string -> HsvaColor) = jsNative
let [<Import("rgbaStringToHsva","@uiw/color-convert")>] rgbaStringToHsva: (string -> HsvaColor) = jsNative
let [<Import("rgbStringToHsva","@uiw/color-convert")>] rgbStringToHsva: (string -> HsvaColor) = jsNative
/// Converts an RGBA color plus alpha transparency to hex
let [<Import("rgbaToHex","@uiw/color-convert")>] rgbaToHex: (RgbaColor -> string) = jsNative
let [<Import("rgbaToHexa","@uiw/color-convert")>] rgbaToHexa: (RgbaColor -> string) = jsNative
let [<Import("hexToHsva","@uiw/color-convert")>] hexToHsva: (string -> HsvaColor) = jsNative
let [<Import("hexToRgba","@uiw/color-convert")>] hexToRgba: (string -> RgbaColor) = jsNative
/// <summary>Converts HSVA to RGBA. Based on formula from <see href="https://en.wikipedia.org/wiki/HSL_and_HSV" /></summary>
/// <param name="color">HSVA color as an array [0-360, 0-1, 0-1, 0-1]</param>
let [<Import("hsvaToRgba","@uiw/color-convert")>] hsvaToRgba: (HsvaColor -> RgbaColor) = jsNative
let [<Import("hsvaToRgbString","@uiw/color-convert")>] hsvaToRgbString: (HsvaColor -> string) = jsNative
let [<Import("hsvaToRgbaString","@uiw/color-convert")>] hsvaToRgbaString: (HsvaColor -> string) = jsNative
let [<Import("rgbaToRgb","@uiw/color-convert")>] rgbaToRgb: (RgbaColor -> RgbColor) = jsNative
let [<Import("hslaToHsl","@uiw/color-convert")>] hslaToHsl: (HslaColor -> HslColor) = jsNative
let [<Import("hsvaToHex","@uiw/color-convert")>] hsvaToHex: (HsvaColor -> string) = jsNative
let [<Import("hsvaToHexa","@uiw/color-convert")>] hsvaToHexa: (HsvaColor -> string) = jsNative
let [<Import("hsvaToHsv","@uiw/color-convert")>] hsvaToHsv: (HsvaColor -> HsvColor) = jsNative
let [<Import("color","@uiw/color-convert")>] color: (U2<string, HsvaColor> -> ColorResult) = jsNative

type ObjectColor =
    U6<RgbColor, HslColor, HsvColor, RgbaColor, HslaColor, HsvaColor>

type [<AllowNullLiteral>] ColorResult =
    abstract rgb: RgbColor with get, set
    abstract hsl: HslColor with get, set
    abstract hsv: HsvColor with get, set
    abstract rgba: RgbaColor with get, set
    abstract hsla: HslaColor with get, set
    abstract hsva: HsvaColor with get, set
    abstract hex: string with get, set
    abstract hexa: string with get, set

type [<AllowNullLiteral>] HsvColor =
    abstract h: float with get, set
    abstract s: float with get, set
    abstract v: float with get, set

type [<AllowNullLiteral>] HsvaColor =
    inherit HsvColor
    abstract a: float with get, set

type [<AllowNullLiteral>] RgbColor =
    abstract r: float with get, set
    abstract g: float with get, set
    abstract b: float with get, set

type [<AllowNullLiteral>] RgbaColor =
    inherit RgbColor
    abstract a: float with get, set

type [<AllowNullLiteral>] HslColor =
    abstract h: float with get, set
    abstract s: float with get, set
    abstract l: float with get, set

type [<AllowNullLiteral>] HslaColor =
    inherit HslColor
    abstract a: float with get, set

type [<StringEnum>] [<RequireQualifiedAccess>] GetContrastingColor =
    | [<CompiledName("#ffffff")>] ``#ffffff``
    | [<CompiledName("#000000")>] ``#000000``
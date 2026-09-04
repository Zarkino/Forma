module Color_Picker

open Fable.Core
open Fable.Core.JsInterop
open Feliz

type private Props =
    static member inline prefixCls (prefixCls: string) = Interop.mkAttr "prefixCls" prefixCls
    static member inline color (color: U2<string, Color_Convert.HsvaColor>) = Interop.mkAttr "color" color
    static member inline onChange (onChange: Color_Convert.ColorResult -> unit) = Interop.mkAttr "onChange" onChange
    static member inline disableAlpha (disableAlpha: bool) = Interop.mkAttr "disableAlpha" disableAlpha

let private create props = Interop.reactApi.createElement (import "default" "@uiw/react-color-colorful", createObj !!props)

[<ReactComponent>]
let Color_Picker (hex: U2<string, Color_Convert.HsvaColor>, setHex: string -> unit) =
    create [
        Props.color hex
        Props.onChange (fun hexa -> setHex(hexa.hex))
        Props.disableAlpha true
    ]
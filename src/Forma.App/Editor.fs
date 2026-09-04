module Editor

open Fable.Core
open Fable.Core.JsInterop
open Feliz
open Monaco_Editor

[<ImportMember("./language/language.ts")>]
let private beforeMount(monaco: obj): unit = jsNative

[<ReactComponent>]
let Editor(value, setValue, readonly) =
    let theme = React.useContext(Contexts.themeContext)

    create [
        Props.value value
        Props.language "logi-lang"
        Props.theme (if theme.Equals("dark") then !^"logi-theme-dark" else !^"logi-theme-light")
        Props.options
            (jsOptions<Monaco.Editor.IStandaloneEditorConstructionOptions>(fun o ->
                o.minimap <- Some (jsOptions<Monaco.Editor.IEditorMinimapOptions>(fun oMinimap ->
                    oMinimap.enabled <- Some false
                ))
                o.unicodeHighlight <- Some (jsOptions<Monaco.Editor.IUnicodeHighlightOptions>(fun oUnicodeHighlight ->
                    oUnicodeHighlight.ambiguousCharacters <- Some false
                ))
                o.readOnly <- Some readonly
            ))
        Props.beforeMount beforeMount
        Props.onChange setValue
    ]
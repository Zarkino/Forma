module MonacoEditor

open Fable.Core
open Fable.Core.JsInterop
open Feliz

[<StringEnum>]
type Theme =
    | [<CompiledName("vs-dark")>]   Dark
    | [<CompiledName("light")>]     Light

[<Erase>]
type Size =
    | String of string
    | Number of int

type MonacoEditor =
    static member inline DefaultValue (value: string) = "defaultValue" ==> value
    static member inline DefaultLanguage (value: string) = "defaultLanguage" ==> value
    static member inline DefaultPath (value: string) = "defaultPath" ==> value
    static member inline Value (value: string) = "value" ==> value
    static member inline Language (value: string) = "language" ==> value
    static member inline Path (value: string) = "path" ==> value
    static member inline Theme (value: Theme) = "theme" ==> value
    static member inline Line (value: int) = "line" ==> value
    static member inline Loading (value: ReactElement) = "loading" ==> value
    static member inline Options (value: obj) = "options" ==> value                     // monaco.editor.IStandaloneEditorConstructionOptions
    static member inline OverrideServices (value: obj) = "overrideServices" ==> value   // monaco.editor.IEditorOverrideServices
    static member inline SaveViewState (value: bool) = "saveViewState" ==> value
    static member inline KeepCurrentModel (value: bool) = "keepCurrentModel" ==> value
    static member inline Width (value: string) = "width" ==> value
    static member inline Height (value: string) = "height" ==> value
    static member inline ClassName (value: string) = "className" ==> value
    static member inline WrapperProps (value: obj) = "wrapperProps" ==> value
    static member inline BeforeMount (value: obj -> unit) = "beforeMount" ==> value
    static member inline OnMount (value: obj -> unit) = "onMount" ==> value
    static member inline OnChange (value: string -> unit) = "onChange" ==> value
    static member inline OnValidate (value: obj -> unit) = "onValidate" ==> value
    
    static member inline create props = Interop.reactApi.createElement (import "default" "@monaco-editor/react", createObj !!props)
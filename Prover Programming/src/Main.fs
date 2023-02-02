module Main

open Feliz
open App
open Browser.Dom
open Fable.Core
open Fable.Core.JsInterop
open Fable.React

importSideEffects "./styles/global.scss"

[<StringEnum>]
type Theme =
    | Light
    | Dark

type Size =
    | String of string
    | Number of int

type MonacoEditorProps = {
    DefaultValue: string option
    DefaultLanguage: string option
    Value: string option
    DefaultPath: string option
    Language: string option
    Path: string option
    Theme: Theme option
    Line: int option
    Loading: ReactElement option
    Options: obj option           // monaco.editor.IStandaloneEditorConstructionOptions
    OverrideServices: obj option  // monaco.editor.IEditorOverrideServices
    SaveViewState: bool option
    KeepCurrentModel: bool option
    Width: Size option
    Height: Size option
    ClassName: string option
    WrapperProps: obj option
    BeforeMount: (obj -> unit) option
    OnMount: (obj -> unit) option
    OnChange: (obj -> unit) option
    OnValidate: (obj list -> unit) option
}

let Editor =
    Interop.reactApi.createElement (import "default" "@monaco-editor/react", createObj [
        "width" ==> "100%"
        "height" ==> "90vh"
        "defaultValue" ==> "let x = 10\nlet y = 20"
        "lang" ==> "fsharp"
        "theme" ==> "vs-dark"
    ])

//console.log("Imported value", Editor)

let main =
    Html.div [
        Html.h1 "Editor"
        Editor
    ]

let root = ReactDOM.createRoot(document.getElementById "root")
root.render(main)
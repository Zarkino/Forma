module Main

open Feliz
open App
open Browser.Dom
open Fable.Core.JsInterop
open MonacoEditor

importSideEffects "./styles/global.scss"

let main =
    Html.div [
        Html.h1 "Editor"
        MonacoEditor.create [
            MonacoEditor.Value "let x = 10\nlet f x = x * x"
            MonacoEditor.Language "fsharp"
            MonacoEditor.Height "90vh"
            MonacoEditor.Options (createObj [ "minimap" ==> createObj ["enabled" ==> "false"] ])
        ]
    ]

let root = ReactDOM.createRoot(document.getElementById "root")
root.render(main)
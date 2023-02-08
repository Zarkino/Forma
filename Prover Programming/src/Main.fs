module Main

open Feliz
open App
open Fable.Core.JsInterop

importSideEffects "./styles/global.scss"

let main =
    Html.div [
        Html.h1 "Editor"
        Components.Editor()
    ]

let root = ReactDOM.createRoot(Browser.Dom.document.getElementById "root")
root.render(main)
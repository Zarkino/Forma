module Main

open Feliz
open Feliz.Bulma
open App
open Fable.Core.JsInterop

importSideEffects "./styles/global.scss"

let marko_polo (x: string) = x.Replace("marko", "polo")

[<ReactComponent>]
let Main () =
    let (value, setValue) = React.useState("let x = 10\nlet f x = x * x")
    Html.div [
        Html.h1 [
            prop.text "Editor"
        ]
        Bulma.columns [
            Bulma.column [
                Components.Editor(value, setValue, false)
            ]
            Bulma.column [
                Components.Editor(marko_polo (value), (fun _ -> ()), true)
                
            ]
        ]
    ]

let root = ReactDOM.createRoot(Browser.Dom.document.getElementById "root")
root.render(Main())
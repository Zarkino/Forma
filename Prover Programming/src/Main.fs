module Main

open Feliz
open Feliz.Bulma
open App
open Fable.Core.JsInterop
open Feliz.style

let theme = 1

if theme > 0 then importSideEffects "./styles/light.scss" else importSideEffects "./styles/dark.scss"

let marko_polo (x: string) = x.Replace("marko", "polo")

[<ReactComponent>]
let Main () =
    let (value, setValue) = React.useState("let x = 10\nlet f x = x * x")
    Html.div [
        prop.style [ style.backgroundColor color.black ]
        prop.children [
            Html.h1 [
                prop.text "Editor"
                prop.className "title"
            ]
            Bulma.columns [
                Bulma.column [
                    Components.Editor(value, setValue, false, MonacoEditor.Dark)
                ]
                Bulma.column [
                    Components.Editor(marko_polo (value), (fun _ -> ()), true, MonacoEditor.Dark)
                    
                ]
            ]
        ]
    ]

let root = ReactDOM.createRoot(Browser.Dom.document.getElementById "root")
root.render(Main())
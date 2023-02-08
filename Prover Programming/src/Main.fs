module Main

open Feliz
open Feliz.Bulma
open Fable.Core.JsInterop
open App

importSideEffects "./styles/global.scss"

let marko_polo (x: string) = x.Replace("marko", "polo")

[<ReactComponent>]
let Main () =
    let (theme, setTheme) = React.useState("light")
    let (value, setValue) = React.useState("let x = 10\nlet f x = x * x")
    
    Html.div [
        prop.className theme
        prop.children [
            Html.h1 [
                prop.text "Prover Programming"
                prop.className theme
            ]
            Bulma.button.button [
                prop.text "Switch Theme"
                prop.style [
                    style.color theme
                ]
                prop.onClick (fun _ -> setTheme(if theme.Equals("light") then "dark" else "light"))
            ]
            Bulma.columns [
                Bulma.column [
                    prop.className theme
                    prop.children [
                        Html.h1 [
                            prop.text "Input"
                            prop.className "subtitle"
                        ]
                        Components.Editor(value, setValue, false, theme)
                    ]
                ]
                Bulma.column [
                    prop.className theme
                    prop.children [
                        Html.h1 [
                            prop.text "Output"
                            prop.className "subtitle"
                        ]
                        Components.Editor(marko_polo (value), (fun _ -> ()), true, theme)
                    ]
                ]
            ]
        ]
    ]

let root = ReactDOM.createRoot(Browser.Dom.document.getElementById "root")
root.render(Main())
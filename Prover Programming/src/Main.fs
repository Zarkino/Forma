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
            Navigation_Bar.Navigation(theme, setTheme)
            Button_Level.Button_Level(theme)
            Bulma.columns [
                columns.isGapless
                prop.className theme
                prop.children [
                    Bulma.column [
                        column.isHalf
                        prop.children [
                            Bulma.columns [
                                columns.isMultiline
                                prop.children [
                                    Bulma.column [
                                        column.isFull
                                        prop.children [
                                            Bulma.subtitle [
                                                title.is4
                                                prop.text "Input"
                                            ]
                                        ]
                                    ]
                                    Bulma.column [
                                        column.isFull
                                        prop.className "editor"
                                        prop.children [
                                            Components.Editor(theme, value, setValue)
                                        ]
                                    ]
                                ]
                            ]
                        ]
                    ]
                    Bulma.column [
                        column.isHalf
                        prop.children [
                            Bulma.columns [
                                columns.isMultiline
                                prop.children [
                                    Bulma.column [
                                        column.isFull
                                        prop.children [
                                            Bulma.subtitle [
                                                title.is4
                                                prop.text "Output"
                                            ]
                                        ]
                                    ]
                                    Bulma.column [
                                        column.isFull
                                        prop.className "editor"
                                        prop.children [
                                            Components.Editor(theme, marko_polo (value), readonly=true)
                                        ]
                                    ]
                                ]
                            ]
                        ]
                    ]
                ]
            ]
            Bulma.footer [
                Bulma.color.hasBackgroundPrimary
                prop.style [
                    style.paddingTop 0
                    style.paddingBottom 0
                ]
                prop.text "DTU copyright or something idk"
            ]
        ]
    ]

let root = ReactDOM.createRoot(Browser.Dom.document.getElementById "root")
root.render(Main())
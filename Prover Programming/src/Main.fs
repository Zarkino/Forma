module Main

open Feliz
open Feliz.Bulma
open Fable.Core.JsInterop
open App

importSideEffects "./styles/global.scss"

(*
let monaco: obj = importAll "monaco-editor/esm/vs/editor/editor.api"

Browser.Dom.console.log(monaco?languages?getLanguages())
*)

let marko_polo (x: string) = x.Replace("marko", "polo")

[<ReactComponent>]
let Main () =
    let (theme, setTheme) = React.useState("light")
    let (value, setValue) = React.useState("let x = 10\nlet f x = x * x")
    
    Html.div [
        prop.className theme
        prop.children [
            Bulma.navbar [
                Bulma.color.isPrimary
                prop.children [
                    Bulma.navbarBrand.div [
                        Bulma.navbarItem.a [
                            Html.img [ prop.src "https://bulma.io/images/bulma-logo-white.png"; prop.height 28; prop.width 112; ]
                        ]
                    ]
                    Bulma.navbarMenu [
                        Bulma.navbarStart.div [
                            Bulma.navbarItem.a [ prop.text "Home" ]
                            Bulma.navbarItem.a [ prop.text "Documentation" ]
                            Bulma.navbarItem.a [ prop.text "About" ]
                        ]
                        Bulma.navbarEnd.div [
                            Bulma.navbarItem.div [
                                Bulma.field.div [
                                    Switch.checkbox [
                                        prop.id "theme_switch"
                                        color.isDark
                                        switch.isRounded
                                        switch.isMedium
                                        prop.onClick (fun _ -> setTheme(if theme.Equals("light") then "dark" else "light"))
                                    ]
                                    Html.label [
                                        prop.htmlFor "theme_switch"
                                        prop.text ""
                                    ]
                                ]
                            ]
                        ]
                    ]
                ]
            ]
            Bulma.field.div [
                prop.className "mb-0"
                field.hasAddons
                prop.children [
                    Bulma.control.p [
                        Bulma.button.button [
                            Html.span [ prop.text "Format" ]
                        ]
                    ]
                    Bulma.control.p [
                        Bulma.button.button [
                            Html.span [ prop. text "Download" ]
                        ]
                    ]
                    Bulma.control.p [
                        Bulma.button.button [
                            Html.span [ prop.text "Upload" ]
                        ]
                    ]
                    Bulma.control.p [
                        Bulma.button.button [
                            Html.span [ prop.text "Help" ]
                        ]
                    ]
                    Bulma.control.p [
                        Bulma.button.button [
                            Html.span [ prop.text "Examples" ]
                        ]
                    ]
                ]
            ]
            Bulma.columns [
                columns.isGapless
                prop.className "mb-0"
                prop.children [
                    Bulma.column [
                        column.isHalf
                        prop.className theme
                        prop.children [
                            Bulma.columns [
                                Bulma.column [
                                    column.isNarrow
                                    prop.children [
                                        Html.h1 [
                                            prop.text "Input"
                                            prop.className "subtitle"
                                        ]
                                    ]
                                ]
                                Bulma.column [
                                    prop.children [
                                        Bulma.button.button [
                                        prop.text "Copy to clipboard"
                                        ]
                                    ]
                                ]
                            ]
                            Html.div [
                                prop.className "editor"
                                prop.children [
                                    Components.Editor(value, setValue, false, theme)
                                ]
                            ]
                        ]
                    ]
                    Bulma.column [
                        column.isHalf
                        prop.className theme
                        prop.children [
                            Bulma.columns [
                                Bulma.column [
                                    column.isNarrow
                                    prop.children [
                                        Html.h1 [
                                            prop.text "Output"
                                            prop.className "subtitle"
                                        ]
                                    ]
                                ]
                                Bulma.column [
                                    prop.children [
                                        Bulma.button.button [
                                        prop.text "Copy to clipboard"
                                        ]
                                    ]
                                ]
                            ]
                            Html.div [
                                prop.className "editor"
                                prop.children [
                                    Components.Editor(marko_polo (value), (fun _ -> ()), true, theme)
                                ]
                            ]
                        ]
                    ]
                ]
            ]
            Html.footer [
                prop.text "DTU copyright or something idk"
                prop.className "smallbottomtext"
            ]
        ]
    ]

let root = ReactDOM.createRoot(Browser.Dom.document.getElementById "root")
root.render(Main())
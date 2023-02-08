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
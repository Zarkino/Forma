module Button_Level

open Feliz
open Feliz.Bulma

[<ReactComponent>]
let Button_Level(theme: string) =
    let decide_color theme = if theme.Equals("dark") then Bulma.color.isDark else Bulma.color.isLight
    
    Bulma.level [
        prop.className [
            theme
            "py-2"
        ]
        prop.style [style.margin 0]
        prop.children [
            Bulma.levelLeft [
                Bulma.button.button [
                    prop.className "mx-1"
                    decide_color theme
                    prop.text "Format"
                ]
                Bulma.button.button [
                    prop.className "mx-1"
                    decide_color theme
                    prop.text "Download"
                ]
                Bulma.button.button [
                    prop.className "mx-1"
                    decide_color theme
                    prop.text "Upload"
                ]
                Bulma.button.button [
                    prop.className "mx-1"
                    decide_color theme
                    prop.text "Help"
                ]
                Bulma.button.button [
                    prop.className "mx-1"
                    decide_color theme
                    prop.text "Examples"
                ]
            ]
        ]
    ]
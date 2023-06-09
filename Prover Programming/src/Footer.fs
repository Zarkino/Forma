module Footer

open Feliz
open Feliz.Bulma

[<ReactComponent>]
let Footer() =
    let theme = React.useContext(Contexts.themeContext)
    
    Bulma.footer [
            if theme.Equals("dark") then Bulma.color.hasBackgroundDark else Bulma.color.hasBackgroundPrimary
            Bulma.color.hasTextLight
            prop.style [
                style.textAlign.center
                style.paddingTop 0
                style.paddingBottom 0
            ]
            prop.children [
                Html.p [
                    Html.text "Developed by "
                    Html.b "s204433"
                    Html.text " and "
                    Html.b "s204442"
                    Html.text " at the Technical University of Denmark"
                ]
            ]
        ]
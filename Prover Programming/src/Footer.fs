module Footer

open Feliz
open Feliz.Bulma

[<ReactComponent>]
let Footer() =
    let accent = React.useContext(Contexts.accentContext)
    
    Bulma.footer [
            Bulma.color.hasTextLight
            prop.style [
                style.textAlign.center
                style.paddingTop 0
                style.paddingBottom 0
                style.backgroundColor accent
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
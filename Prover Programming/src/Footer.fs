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
                Html.div [ prop.style [ style.height 20 ] ]
            ]
        ]
module Footer

open Feliz
open Feliz.Bulma

[<ReactComponent>]
let Footer() =
    let accent = React.useContext(Contexts.accentContext)
    
    Bulma.footer [
        prop.style [
            style.backgroundColor accent
            style.padding 0
            style.height 20
        ]
    ]
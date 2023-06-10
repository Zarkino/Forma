module Navigation_Bar

open Feliz
open Feliz.Bulma

[<ReactComponent>]
let Navigation() =
    let accent = React.useContext(Contexts.accentContext)
    
    Bulma.navbar [
        Bulma.color.hasTextLight
        prop.style [ style.backgroundColor accent ]
        prop.children [
            Bulma.navbarBrand.div [
                Bulma.navbarItem.a [
                    prop.href "/"
                    prop.children [
                        Html.img [ prop.src "https://i.postimg.cc/j2NrVSFS/logo.png"; prop.height 28; prop.width 148; ]
                    ]
                ]
            ]
            Bulma.navbarMenu [
                Bulma.navbarStart.div [
                    Bulma.navbarItem.a [ prop.href "/"; prop.text "Home" ]
                    Bulma.navbarItem.a [ prop.href "/settings"; prop.text "Settings" ]
                    Bulma.navbarItem.a [ prop.text "About" ]
                ]
            ]
        ]
    ]
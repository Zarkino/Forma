module Navigation_Bar

open Feliz
open Feliz.Bulma
open Feliz.Router

[<ReactComponent>]
let Navigation() =
    let accent = React.useContext(Contexts.accentContext)
    
    Bulma.navbar [
        Bulma.color.hasTextLight
        prop.style [ style.backgroundColor accent ]
        prop.children [
            Bulma.navbarBrand.div [
                Bulma.navbarItem.a [
                    prop.href (Router.format("home"))
                    prop.children [
                        Html.img [ prop.src "assets/logo.png"; prop.height 28; prop.width 148; ]
                    ]
                ]
            ]
            Bulma.navbarMenu [
                Bulma.navbarStart.div [
                    Bulma.navbarItem.a [ prop.href (Router.format("home")); prop.text "Home" ]
                    Bulma.navbarItem.a [ prop.href (Router.format("settings")); prop.text "Settings" ]
                    Bulma.navbarItem.a [ prop.href (Router.format("about")); prop.text "About" ]
                ]
            ]
        ]
    ]
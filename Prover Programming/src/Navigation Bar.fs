module Navigation_Bar

open Feliz
open Feliz.Bulma

[<ReactComponent>]
let Navigation(theme: string, setTheme: string -> unit) =
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
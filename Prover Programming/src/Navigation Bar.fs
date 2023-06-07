module Navigation_Bar

open Feliz
open Feliz.Bulma

[<ReactComponent>]
let Navigation(theme: string, setTheme: string -> unit) =
    let decide_color theme = if theme.Equals("dark") then Bulma.color.isDark else Bulma.color.isPrimary
    
    Bulma.navbar [
        decide_color theme
        prop.children [
            Bulma.navbarBrand.div [
                Bulma.navbarItem.a [
                    Html.img [ prop.src "https://i.postimg.cc/j2NrVSFS/logo.png"; prop.height 28; prop.width 148; ]
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
                                prop.defaultChecked (Browser.WebStorage.localStorage.getItem("theme").Equals("dark"))
                                prop.id "theme_switch"
                                color.isBlack
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
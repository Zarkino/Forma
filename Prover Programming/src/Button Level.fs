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
                    prop.children [
                        Html.span [ Html.text "Format" ]
                    ]
                ]
                Bulma.button.button [
                    prop.className "mx-1"
                    decide_color theme
                    prop.children [
                        Html.span [ prop.className "icon"; prop.children [ Html.i [ prop.className "fa-solid fa-download" ] ] ]
                        Html.span [ Html.text "Download" ]
                    ]
                ]
                Bulma.button.button [
                    prop.className "mx-1"
                    decide_color theme
                    prop.children [
                        Html.span [ prop.className "icon"; prop.children [ Html.i [ prop.className "fa-solid fa-upload" ] ] ]
                        Html.span [ Html.text "Upload" ]
                    ]
                ]
                Bulma.button.button [
                    prop.className "mx-1"
                    decide_color theme
                    prop.children [
                        Html.span [ prop.className "icon"; prop.children [ Html.i [ prop.className "fa-solid fa-circle-info" ] ] ]
                        Html.span [ Html.text "Help" ]
                    ]
                ]
                Bulma.dropdown [
                    dropdown.isHoverable
                    prop.children [
                        Bulma.dropdownTrigger [
                            prop.children [
                                Bulma.button.button [
                                    prop.className "mx-1"
                                    decide_color theme
                                    prop.ariaHasPopup true
                                    prop.ariaControls "dropdown-menu"
                                    prop.children [
                                        Html.span [ Html.text "Examples" ]
                                        Html.span [ prop.className "icon"; prop.children [ Html.i [ prop.className "fa-solid fa-angle-down" ] ] ]
                                    ]
                                ]
                            ]
                        ]
                        Bulma.dropdownMenu [
                            prop.id "dropdown-menu"
                            prop.role "menu"
                            prop.children [
                                Bulma.dropdownContent [
                                    Bulma.dropdownItem.a [
                                        Html.span [ Html.text "Example 1" ]
                                    ]
                                    Bulma.dropdownItem.a [
                                        Html.span [ Html.text "Example 2" ]
                                    ]
                                ]
                            ]
                        ]
                    ]
                ]
            ]
        ]
    ]
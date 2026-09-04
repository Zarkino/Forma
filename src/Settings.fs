module Settings

open Fable.Core.JsInterop
open Feliz
open Feliz.Bulma

let private colorButton (color, setColor) =
    Bulma.button.button [
        button.isRounded
        prop.className "mx-1"
        prop.style [
            style.backgroundColor color
            style.borderColor "transparent"
        ]
        prop.onClick (fun _ -> setColor(color))
    ]

[<ReactComponent>]
let Settings(setAccent, setTheme) =
    let theme = React.useContext(Contexts.themeContext)
    let accent = React.useContext(Contexts.accentContext)
    
    let active, setActive = React.useState(false)
    
    Html.div [
        prop.className [ theme; "settings" ]
        prop.children [
            Bulma.container [
                prop.className [ "pt-3"; "px-3" ]
                prop.style [
                    style.display.flex
                    style.flexDirection.column
                    style.alignItems.center
                ]
                prop.children [
                    Bulma.title [
                        title.is3
                        prop.text "Appearance"
                    ]
                    Bulma.title [
                        title.is5
                        prop.text "Choose your theme"
                    ]
                    Bulma.field.div [
                        field.isGrouped
                        prop.children [
                            Bulma.label [
                                prop.className "mx-5"
                                prop.style [
                                    style.borderStyle.solid
                                    style.borderWidth 1
                                    style.borderRadius 5
                                    style.width (length.vw 16)
                                    style.marginBottom 0
                                ]
                                prop.children [
                                    Html.div [
                                        prop.className "py-5"
                                        prop.style [
                                            style.backgroundColor "#3D3D3D"
                                            style.display.flex
                                            style.justifyContent.center
                                        ]
                                        prop.children [
                                            Bulma.image [
                                                prop.children [
                                                    Html.img [
                                                        prop.style [
                                                            style.borderRadius 5
                                                            style.width (length.vw 10)
                                                        ]
                                                        prop.src $"%s{Env.Vite.BASE_URL}assets/web-appearance-light.svg"
                                                    ]
                                                ]
                                            ]
                                        ]
                                    ]
                                    Html.div [
                                        prop.className [ "py-2"; "px-3" ]
                                        prop.style [
                                            style.borderTopStyle borderStyle.solid
                                            style.borderWidth 1
                                        ]
                                        prop.children [
                                            Bulma.input.radio [
                                                prop.className "mr-2"
                                                prop.name "mode"
                                                prop.defaultChecked (theme |> function "light" -> true | _ -> false)
                                                prop.onClick (fun _ -> setTheme("light"))
                                            ]
                                            Html.span [ Html.text "Light" ]
                                        ]
                                    ]
                                ]
                            ]
                            Bulma.label [
                                prop.className "mx-5"
                                prop.style [
                                    style.borderStyle.solid
                                    style.borderWidth 1
                                    style.borderRadius 5
                                    style.width (length.vw 16)
                                    style.marginBottom 0
                                ]
                                prop.children [
                                    Html.div [
                                        prop.className "py-5"
                                        prop.style [
                                            style.backgroundColor "#3D3D3D"
                                            style.display.flex
                                            style.justifyContent.center
                                        ]
                                        prop.children [
                                            Bulma.image [
                                                prop.children [
                                                    Html.img [
                                                        prop.style [
                                                            style.borderRadius 5
                                                            style.width (length.vw 10)
                                                        ]
                                                        prop.src $"%s{Env.Vite.BASE_URL}assets/web-appearance-dark.svg"
                                                    ]
                                                ]
                                            ]
                                        ]
                                    ]
                                    Html.div [
                                        prop.className [ "py-2"; "px-3" ]
                                        prop.style [
                                            style.borderTopStyle borderStyle.solid
                                            style.borderWidth 1
                                        ]
                                        prop.children [
                                            Bulma.input.radio [
                                                prop.className "mr-2"
                                                prop.name "mode"
                                                prop.defaultChecked (theme |> function "dark" -> true | _ -> false)
                                                prop.onClick (fun _ -> setTheme("dark"))
                                            ]
                                            Html.span [ Html.text "Dark" ]
                                        ]
                                    ]
                                ]
                            ]
                        ]
                    ]
                    Bulma.title [
                        title.is5
                        prop.className "mt-3"
                        prop.text "Choose your accent color"
                    ]
                    Bulma.field.div [
                        field.isGrouped
                        prop.children [
                            colorButton("#4059F2", setAccent)
                            colorButton("#0FD269", setAccent)
                            colorButton("#EBBE11", setAccent)
                            colorButton("#E99E0F", setAccent)
                            colorButton("#E91111", setAccent)
                            colorButton("#EA1195", setAccent)
                            colorButton("#CC10EA", setAccent)
                            colorButton("#5A0FEC", setAccent)
                            colorButton("#489FBD", setAccent)
                            colorButton("#0ABB91", setAccent)
                            colorButton("#3F90F7", setAccent)
                            Bulma.button.button [
                                prop.className [ "mx-1"; theme ]
                                prop.target "modal-picker"
                                prop.onClick (fun _  -> setActive(true))
                                prop.onKeyDown (key.escape, fun _ -> setActive(false))
                                prop.children [ Html.text "Custom" ]
                            ]
                            Bulma.modal [
                                prop.id "modal-picker"
                                if active then modal.isActive
                                prop.children [
                                    Bulma.modalBackground [ prop.onClick (fun _ -> setActive(false)) ]
                                    Bulma.modalClose [ prop.onClick (fun _ -> setActive(false)) ]
                                    Bulma.modalContent [
                                        prop.style [
                                            style.display.flex
                                            style.justifyContent.center
                                        ]
                                        prop.children [
                                            Bulma.box [
                                                prop.className theme
                                                prop.style [ style.width.maxContent ]
                                                prop.children [
                                                    Color_Picker.Color_Picker(!^accent, setAccent)
                                                    Bulma.field.div [
                                                        field.isGrouped
                                                        prop.className "mt-4"
                                                        prop.style [ style.justifyContent.center ]
                                                        prop.children [
                                                            Bulma.button.button [
                                                                prop.style [ style.color "white"; style.backgroundColor accent ]
                                                                prop.onClick
                                                                    (fun _  -> setActive(false))
                                                                prop.children [ Html.span [ Html.text "Done" ] ]
                                                            ]
                                                        ]
                                                    ]
                                                ]
                                            ]
                                        ]
                                    ]
                                ]
                            ]
                        ]
                    ]
                ]
            ]
        ]
    ]
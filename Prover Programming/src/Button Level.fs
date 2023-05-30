module Button_Level

open Feliz
open Feliz.Bulma

let examples = [|
    "lemma (P & Q) --> (Q & P)\nproof (rule Imp_I) {\n\tassume P & Q\n\tfrom P & Q have P by (rule Con_E1)\n\tfrom P & Q have Q by (rule Con_E2)\n\tfrom P and Q show Q & P by (rule Con_I)\n}"
    "lemma P --> ~~P\nproof (rule Imp_I) {\n\tassume P\n\tshow ~~P\n\tproof (rule Falsity_E) {\n\t\tassume ~P\n\t\tfrom ~P and P show F by (rule Neg_E)\n\t}\n}"
    "lemma ((P --> Q) & ~Q) --> ~P\nproof (rule Imp_I) {\n\tassume (P --> Q) & ~Q\n\tshow ~P\n\tproof (rule Falsity_E) {\n\t\tassume P\n\t\tfrom (P --> Q) & ~Q have P --> Q by (rule Con_E1)\n\t\tfrom (P --> Q) & ~Q have ~Q by (rule Con_E2)\n\t\tfrom P --> Q and P have Q by (rule Imp_E)\n\t\tfrom ~Q and Q show F by (rule Neg_E)\n\t}\n}"
    "lemma (P <--> Q) <--> (Q <--> P)\nproof (rule Iff_I) {\n\tshow (P <--> Q) --> (Q <--> P)\n\tproof (rule Imp_I) {\n\t\tassume P <--> Q\n\t\tfrom P <--> Q have P --> Q by (rule Iff_E1)\n\t\tfrom P <--> Q have Q --> P by (rule Iff_E2)\n\t\tfrom P --> Q and Q --> P show Q <--> P by (rule Iff_I)\n\t}\n\tshow (Q <--> P) --> (P <--> Q)\n\tproof (rule Imp_I) {\n\t\tassume Q <--> P\n\t\tfrom Q <--> P have Q --> P by (rule Iff_E1)\n\t\tfrom Q <--> P have P --> Q by (rule Iff_E2)\n\t\tfrom Q --> P and P --> Q show P <--> Q by (rule Iff_I)\n\t}\n}"
|]

[<ReactComponent>]
let Button_Level (theme: string, setInput: string -> unit) =
    let (help, setHelp) = React.useState(false)
    
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
                    prop.target "modal-help"
                    prop.onClick (fun _ -> setHelp(true))
                    prop.onKeyDown (key.escape, fun _ -> setHelp(false))
                    prop.children [
                        Html.span [ prop.className "icon"; prop.children [ Html.i [ prop.className "fa-solid fa-circle-info" ] ] ]
                        Html.span [ Html.text "Help" ]
                    ]
                ]
                Bulma.modal [
                    prop.id "modal-help"
                    if help then modal.isActive
                    prop.children [
                        Bulma.modalBackground [ prop.onClick (fun _ -> setHelp(false)) ]
                        Bulma.modalContent [
                            Bulma.box [
                                Html.h1 [
                                    prop.className [ "title"; "is-4" ]
                                    prop.text "Language Reference"
                                ]
                                Html.span [ Html.text "..." ]
                                Html.h1 [
                                    prop.className [ "title"; "is-4" ]
                                    prop.text "Rules"
                                ]
                                Bulma.table [
                                    table.isFullWidth
                                    prop.children [
                                        Html.thead [
                                            Html.tr [
                                                Html.th [ Html.abbr "Rule" ]
                                                Html.th [ Html.abbr "Definition" ]
                                            ]
                                        ]
                                        Html.tbody (
                                            Proof_Interface.rules
                                            |> Map.toList
                                            |> List.map
                                                (fun (k, rule) ->
                                                    Html.tr [
                                                        Html.td $"%s{k}"
                                                        Html.td $"%s{Logic.ML.Meta.ToString rule}"
                                                    ]
                                                )
                                        )
                                    ]
                                ]
                            ]
                        ]
                        Bulma.modalClose [ prop.onClick (fun _ -> setHelp(false)) ]
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
                                Bulma.dropdownContent
                                    (examples |> Array.mapi (fun i ex ->
                                        Bulma.dropdownItem.a [
                                            prop.onClick (fun _ -> setInput(ex))
                                            prop.text $"Example %i{i+1}"
                                        ]))
                            ]
                        ]
                    ]
                ]
            ]
        ]
    ]
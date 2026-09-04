module Language_Reference

open Feliz
open Feliz.Bulma

let private styles = Stylesheet.load "./styles/language.module.scss"

[<ReactComponent>]
let Language_Reference() =
    let theme = React.useContext(Contexts.themeContext)
    let accent = React.useContext(Contexts.accentContext)
    
    let formatSubcript(string: string) = Html.sub [ Html.strong string ]
    
    let formatKeyword(string: string) =
        match string with
        | "lemma" | "proof" | "from" | "by" | "next" | "have"   -> Some(styles[theme + "_keyword1"])
        | "and"                                                 -> Some(styles[theme + "_keyword2"])
        | "assume" | "show"                                     -> Some(styles[theme + "_keyword3"])
        | _                                                     -> None
        |> function
            | Some(color)   -> Html.strong [ prop.style [ style.color color ]; prop.children [ Html.text string ] ]
            | None          -> Html.text string
    
    let separator() = Html.strong [ prop.style [ style.color accent ]; prop.children [ Html.text " | " ] ]
    
    Bulma.content [
        Html.p [ Html.text "The following list outlines the structure and syntax of the language." ]
        Bulma.table [
            table.isFullWidth
            table.isNarrow
            prop.className theme
            prop.children [
                Html.tbody [
                    Html.tr [
                        prop.children [
                            Html.td [ Html.strong "L" ]
                            Html.td [ formatKeyword("lemma"); Html.text " [<name>:] "; Html.strong "F"; formatSubcript("M"); Html.text " "; Html.strong "P" ]
                        ]
                    ]
                    Html.tr [
                        Html.td [ Html.strong "P" ]
                        Html.td [
                            formatKeyword("proof"); Html.strong " M"; formatSubcript("R"); Html.text " { "; Html.strong "B"; Html.text " }"
                        ]
                    ]
                    Html.tr [
                        Html.td [ Html.strong "B" ]
                        Html.td [
                            Html.strong "B "; formatKeyword("next"); Html.strong " B"
                            separator()
                            Html.text "["; Html.strong "S"; Html.text "]+ "; Html.strong "C"; formatSubcript("S")
                        ]
                    ]
                    Html.tr [
                        Html.td [ Html.strong "M"; formatSubcript("R") ]
                        Html.td [
                            Html.text "(rule <name>)"
                            separator()
                            Html.text "this"
                            separator()
                            Html.text "none"
                            separator()
                            Html.text "-"
                        ]
                    ]
                    Html.tr [
                        Html.td [ Html.strong "M"; formatSubcript("C") ]
                        Html.td [
                            Html.text "(rule <name>)"
                            separator()
                            Html.text "this"
                        ]
                    ]
                    Html.tr [
                        Html.td [ Html.strong "S" ]
                        Html.td [
                            formatKeyword("assume"); Html.strong " F"; formatSubcript("M"); Html.text " ["; formatKeyword("and"); Html.strong " F"; formatSubcript("M"); Html.text "]+"
                            separator()
                            Html.strong "C"; formatSubcript("H")
                        ]
                    ]
                    Html.tr [
                        Html.td [ Html.strong "C"; formatSubcript("H") ]
                        Html.td [
                            Html.text "["; formatKeyword("from"); Html.strong " F"; formatSubcript("M"); Html.text " ["; formatKeyword("and"); Html.strong " F"; formatSubcript("M"); Html.text "]+"; Html.text "] "; formatKeyword("have"); Html.strong " F"; formatSubcript("M"); Html.text " "; formatKeyword("by"); Html.strong " M"; formatSubcript("C")
                            separator()
                            formatKeyword("have"); Html.strong " F"; formatSubcript("M"); Html.text " "; Html.strong "P"
                        ]
                    ]
                    Html.tr [
                        Html.td [ Html.strong "C"; formatSubcript("S") ]
                        Html.td [
                            Html.text "["; formatKeyword("from"); Html.strong " F"; formatSubcript("M"); Html.text " ["; formatKeyword("and"); Html.strong " F"; formatSubcript("M"); Html.text "]+"; Html.text "] "; formatKeyword("show"); Html.strong " F"; formatSubcript("M"); Html.text " "; formatKeyword("by"); Html.strong " M"; formatSubcript("C")
                            separator()
                            formatKeyword("show"); Html.strong " F"; formatSubcript("M"); Html.text " "; Html.strong "P"
                        ]
                    ]
                    Html.tr [
                        Html.td [ Html.strong "F"; formatSubcript("M") ]
                        Html.td [
                            Html.strong "F"; formatSubcript("M"); Html.text " ⟹ "; Html.strong "F"; formatSubcript("M")
                            separator()
                            Html.strong "F"; formatSubcript("M"); Html.text " ≡ "; Html.strong "F"; formatSubcript("M")
                            separator()
                            Html.text "⋀"; Html.text "<var list>"; Html.text "+. "; Html.strong "F"; formatSubcript("M")
                            separator()
                            Html.text "("; Html.strong "F"; formatSubcript("M"); Html.text ")"
                            separator()
                            Html.strong "F"; formatSubcript("P")
                        ]
                    ]
                    Html.tr [
                        Html.td [ Html.strong "F"; formatSubcript("P") ]
                        Html.td [
                            Html.text "<var>"
                            separator()
                            Html.text "⊤"
                            separator()
                            Html.text "⊥"
                            separator()
                            Html.text " ¬"; Html.strong "F"; formatSubcript("P")
                            separator()
                            Html.strong "F"; formatSubcript("P"); Html.text " ⟶ "; Html.strong "F"; formatSubcript("P")
                            separator()
                            Html.strong "F"; formatSubcript("P"); Html.text " ∧ "; Html.strong "F"; formatSubcript("P")
                            separator()
                            Html.strong "F"; formatSubcript("P"); Html.text " ∨ "; Html.strong "F"; formatSubcript("P")
                            separator()
                            Html.strong "F"; formatSubcript("P"); Html.text " ⟷ "; Html.strong "F"; formatSubcript("P")
                            separator()
                            Html.text "("; Html.strong "F"; formatSubcript("P"); Html.text ")"
                        ]
                    ]
                ]
            ]
        ]
        Html.h1 [
            prop.className [ "title"; "is-5" ]
            prop.style [ style.marginBottom 0 ]
            prop.text "Notes"
        ]
        Html.ul [
            prop.style [ style.marginTop 10 ]
            prop.children [
                Html.li [ Html.span [ Html.samp "[...]";  Html.text " means optional." ] ]
                Html.li [ Html.span [ Html.text "+ means you can have more than 1." ] ]
                Html.li [ Html.span [ Html.text "A <name> is a string up to 10 characters long. " ] ]
                Html.li [ Html.span [ Html.text "A <var> is any alphanumeric string. " ] ]
            ]
        ]
    ]
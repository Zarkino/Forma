module Parsing_Help

open Feliz
open Feliz.Bulma

[<ReactComponent>]
let Parsing_Help() =
    Bulma.content [
        Html.p "Parsing is strictly right-associative. All binary operators also have the same precedence. This means they are treated equally in the order of evaluation. As a result, it is essential to use parentheses when necessary to control the order of operations."
        Html.div [
            prop.style [
                style.display.grid
                style.justifyItems.center
                style.gridTemplateColumns [ length.percent 45; length.percent 10; length.percent 45 ]
                style.gridTemplateRows [ length.percent 50; length.percent 50 ]
            ]
            prop.children [
                Html.p "P ⟹ Q ⟹ R"
                Html.p "="
                Html.p "P ⟹ (Q ⟹ R)"
                
                Html.p "P ⟶ P ∨ Q"
                Html.p "="
                Html.p "P ⟶ (P ∨ Q)"
            ]
        ]
        Html.p "It is important to note that negations go as deep as possible, matching constants, variables or negations, before finally trying binary operators."
        Html.div [
            prop.style [
                style.display.grid
                style.justifyItems.center
                style.gridTemplateColumns [ length.percent 45; length.percent 10; length.percent 45 ]
            ]
            prop.children [
                Html.p "¬¬P ⟶ P"
                Html.p "="
                Html.p "(¬¬P) ⟶ P"
            ]
        ]
    ]
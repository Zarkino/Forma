module Rules

open Feliz
open Feliz.Bulma

[<ReactComponent>]
let Rules() =
    let theme = React.useContext(Contexts.themeContext)
    
    Bulma.content [
        Bulma.table [
            table.isFullWidth
            table.isNarrow
            prop.className theme
            prop.children [
                Html.thead [ Html.tr [ Html.th [ Html.abbr "Rule" ]; Html.th [ Html.abbr "Definition" ] ] ]
                Html.tbody [
                    Html.tr [ Html.td "Imp_I";      Html.td "(P ⟹ Q) ⟹ P ⟶ Q" ]
                    Html.tr [ Html.td "Imp_E";      Html.td "P ⟶ Q ⟹ P ⟹ Q" ]
                    Html.tr [ Html.td "Con_I";      Html.td "P ⟹ Q ⟹ P ∧ Q" ]
                    Html.tr [ Html.td "Con_E1";     Html.td "P ∧ Q ⟹ P" ]
                    Html.tr [ Html.td "Con_E2";     Html.td "P ∧ Q ⟹ Q" ]
                    Html.tr [ Html.td "Dis_I1";     Html.td "P ⟹ P ∨ Q" ]
                    Html.tr [ Html.td "Dis_I2";     Html.td "Q ⟹ P ∨ Q" ]
                    Html.tr [ Html.td "Dis_E";      Html.td "P ∨ Q ⟹ (P ⟹ R) ⟹ (Q ⟹ R) ⟹ R" ]
                    Html.tr [ Html.td "Neg_I";      Html.td "(P ⟹ ⊥) ⟹ ¬P" ]
                    Html.tr [ Html.td "Neg_E";      Html.td "¬P ⟹ P ⟹ Q" ]
                    Html.tr [ Html.td "Iff_I";      Html.td "(P ⟹ Q) ⟹ (Q ⟹ P) ⟹ P ⟷ Q" ]
                    Html.tr [ Html.td "Iff_E1";     Html.td "P ⟷ Q ⟹ P ⟹ Q" ]
                    Html.tr [ Html.td "Iff_E2";     Html.td "P ⟷ Q ⟹ Q ⟹ P" ]
                    Html.tr [ Html.td "Falsity_E";  Html.td "⊥ ⟹ P" ]
                    Html.tr [ Html.td "Truth_I";    Html.td "⊥ ⟶ ⊥ ⟹ ⊤" ]
                    Html.tr [ Html.td "LEM";        Html.td "P ∨ ¬P" ]
                ]
            ]
        ]
    ]
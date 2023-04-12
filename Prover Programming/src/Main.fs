module Main

open Feliz
open Feliz.Bulma
open Fable.Core.JsInterop
open App

importSideEffects "./styles/global.scss"

let initialContent = "/*
Just some example arguments from wikipedia,
mainly a language feature demonstrator.
*/

((p -> q) & p) ||= q                //Modus Ponens
((p -> q) & ~q) ||= ~p              //Modus Tollens
((p | q) & ~p) ||= q                //Disjunctive Syllogism
(p <-> q) ||= ((p -> q) & (q -> p))  //Material Equivalence (1)

//Pre-determined truth values
(T -> T) & (F -> T)

//Wikipedia FOL example
!x!y(P(f(x)) -> ~(P(x) -> Q(f(y), x, z)))

//Constants can be used like so
!x(_c)

//using a macro
.myMacro

//defining a macro
def myMacro {
    //Macro containing Modus Ponens
    ((p -> q) & p) ||= q
}"

let lemma = "lemma (A & B) -> (B & A)\nproof {\n\tassume A & B\n\thave A by Con_E1\n\thave B by Con_E2\n\tshow B & A by Con_I\n}"

let test_lemma = "lemma (A & B) -> (B & A)
proof {
	assume A & B
	have A by Con_E1
	have B by Con_E2
	have B | C by Dis_I1
	have C | B by Dis_I2
	have A | C by Dis_I1
	assume !A
	have B by Dis_E1
	assume !B
	have A by Dis_E2
	have A -> B by Imp_I
	have B by Imp_E1
	have !A by Imp_E2
	have B -> A by Imp_I
	have A <-> B by Iff_I
	have A -> B by Iff_E1
	have B -> A by Iff_E2
	show B & A by Con_I
}
"

let test_meta = "!!x. p ==> q\np == q\n!p ==> p ==> q"

let rec evaluate_lemma value =
    match Language.Parser.parse_lemma(value) with
    | None, error               ->  error
    | Some(lemma), remaining    ->  (match Proof_Interface.prove(lemma.Proof, Set.empty) with
                                    | Proof_Interface.Success(_, d) ->  (sprintf "%s on %s" (if d then "Success" else "Not Success") $"Lemma %s{lemma.Identifier.ToString()}")
                                    | Proof_Interface.Fail(msg)     ->  msg)
                                    |> (fun result ->
                                        if remaining.Trim().Length > 0 then $"%s{result}\n%s{evaluate_lemma remaining}"
                                        else result)

let rec evaluate_meta value =
    match Language.Parser.parse_meta(value) with
    | None, error           ->  error
    | Some(meta), remaining ->  let result = Meta_Logic.Meta.ToString meta
                                if remaining.Trim().Length > 0 then $"%s{result}\n%s{evaluate_meta remaining}"
                                else result

[<ReactComponent>]
let Main () =
    let (theme, setTheme) = React.useState("light")
    let (value, setValue) = React.useState(test_meta)
    
    React.fragment [    
        Navigation_Bar.Navigation(theme, setTheme)
        Button_Level.Button_Level(theme)
        Bulma.columns [
            columns.isGapless
            prop.className theme
            prop.style [
                style.marginBottom 0
                style.paddingBottom (length.rem 1.5)
            ]
            prop.children [
                Bulma.column [
                    column.isHalf
                    prop.children [
                        Bulma.columns [
                            Bulma.column [
                                column.isFull
                                prop.className "editor"
                                prop.children [
                                    Components.Editor(theme, value, (fun value -> setValue(value.ToString())))
                                ]
                            ]
                        ]
                    ]
                ]
                Bulma.column [
                    column.isHalf
                    prop.children [
                        Bulma.columns [
                            Bulma.column [
                                column.isFull
                                prop.className "editor"
                                prop.children [
                                    Components.Editor(theme, evaluate_meta value, readonly = true)
                                ]
                            ]
                        ]
                    ]
                ]
            ]
        ]
        Bulma.footer [
            Bulma.color.hasBackgroundPrimary
            prop.style [
                style.textAlign.center
                style.paddingTop 0
                style.paddingBottom 0
            ]
            prop.children [
                Html.p [
                    Html.text "Developed by "
                    Html.strong "s204433"
                    Html.text " and "
                    Html.strong "s204442"
                    Html.text " at the Technical University of Denmark"
                ]
            ]
        ]
    ]

let root = ReactDOM.createRoot(Browser.Dom.document.getElementById "root")
root.render(Main())
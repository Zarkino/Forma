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

let lemma = "lemma (A & B) -> (B & A)\nproof {\n\tassume A & B\n\tshow B & A\n}"

let marko_polo (x: string) = x.Replace("marko", "polo")

[<ReactComponent>]
let Main () =
    let (theme, setTheme) = React.useState("light")
    let (value, setValue) = React.useState(lemma)
    
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
                                    Components.Editor(theme, Language.Parser.parse_lemma(value), readonly = true)
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
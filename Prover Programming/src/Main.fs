module Main

open Feliz
open Feliz.Bulma
open Fable.Core.JsInterop
open App
open Propositional_Logic
open System.Text.RegularExpressions

importSideEffects "./styles/global.scss"

let lemma = "lemma Con_S: (A & B) -> (B & A)\nproof (rule Imp_I) {\n\tassume A & B\n\tfrom A & B have A by Con_E1\n\tfrom A & B have B by Con_E2\n\tfrom A and B show B & A by Con_I\n}"

let lemma2 = "lemma P -> ~~P\nproof (rule Imp_I) {\n\tassume P\n\tshow ~~P\n\tproof (rule Neg_I) {\n\t\tassume ~P\n\t\tfrom ~P and P show ~P -> F by Neg_E\n\t}\n}"

let rec evaluate_meta value =
    match Language.Parser.parse_meta(value) with
    | None, error           ->  error
    | Some(meta), remaining ->  let result = Meta_Logic.Meta.ToString meta
                                if remaining.Trim().Length > 0 then $"%s{result}\n%s{evaluate_meta remaining}"
                                else result

[<ReactComponent>]
let Main () =
    let (theme, setTheme) = React.useState("light")
    let (input, setInput) = React.useState(lemma2)
    let (output, setOutput) = React.useState(System.String.Empty)
    let (rules, setRules) = React.useState(Proof_Interface.rules)
    
    React.useEffect(fun () ->
        let parse string =
            let rec inner string cont =
                match Language.Parser.parse_lemma string with
                | None, error               ->  Some(error), cont []
                | Some(lemma), remaining    ->  match remaining.Trim().Length with
                                                | 0 -> None, cont [lemma]
                                                | _ -> inner remaining (fun tail -> cont (lemma::tail))
            inner string id
        
        let evaluate (lemma: Proof_Interface.Lemma) =
            match Proof_Interface.prove(lemma.Goal, lemma.Proof, Set.empty, rules) with
            | Proof_Interface.Fail(msg) -> msg
            | Proof_Interface.Success _ ->
                match lemma.Name with
                | None          ->  ()
                | Some(name)    ->  match lemma.Goal |> standardize |> separate with
                                    | None          -> ()
                                    | Some(a, r)    -> setRules (Map.add name ([a], r) rules)
                $"Successful lemma %s{lemma.ToString()}"
        
        let format (msg, list) =
            (match msg with
            | None  -> System.String.Empty
            | Some(msg) -> "\n" + msg)
            |> sprintf "%s%s" (list |> List.map evaluate |> String.concat "\n")
        
        setRules(Proof_Interface.rules)
        
        Regex.Replace(input, "\/\/.*(?:\n|$)", System.String.Empty)
        |> parse
        |> format
        |> setOutput
    , [|input :> obj|])
    
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
                                    Components.Editor(theme, input, (fun value -> setInput(value.ToString())))
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
                                    Components.Editor(theme, output, readonly = true)
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
module Main

open Feliz
open Feliz.Bulma
open Fable.Core.JsInterop
open App
open Propositional_Logic
open System.Text.RegularExpressions

importSideEffects "./styles/global.scss"

let lemma = "lemma Con_S: (A & B) -> (B & A)\nproof (rule Imp_I) {\n\tassume A & B\n\tfrom A & B have A by Con_E1\n\tfrom A & B have B by Con_E2\n\tfrom A & B show B & A by Con_I\n}"

let lemma2 = "lemma P -> ~~P\nproof (rule Imp_I) {\n\tassume P\n\tshow ~~P\n\tproof (rule Neg_I) {\n\t\tassume ~P\n\t\tfrom ~P and P show F by Neg_E\n\t}\n}"

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
        let rec evaluate_lemma value =
            match Language.Parser.parse_lemma value with
            | None, error               ->  error
            | Some(lemma), remaining    ->  match Proof_Interface.isValid lemma with
                                            | false, msg    -> msg
                                            | true, _       ->
                                                (match Proof_Interface.prove(lemma.Proof, Set.empty, rules) with
                                                | Proof_Interface.Success(_, d) ->
                                                    match d with
                                                    | false ->  $"Unsuccessful lemma %s{lemma.ToString()}"
                                                    | true  ->  match lemma.Name with
                                                                | None          ->  ()
                                                                | Some(name)    ->  match lemma.Identifier |> standardize |> separate with
                                                                                    | None          -> ()
                                                                                    | Some(a, r)    -> setRules (Map.add name ([a], r) rules)
                                                                $"Successful lemma %s{lemma.ToString()}"
                                                | Proof_Interface.Fail(msg)     ->  msg)
                                                |> (fun result ->
                                                    if remaining.Trim().Length > 0 then $"%s{result}\n%s{evaluate_lemma remaining}"
                                                    else result)
        
        let evaluate_comment value = Regex.Matches(value, "\/\/.*(?:\n|$)") |> Seq.fold (fun (acc: string) old -> acc.Replace(old.Value, System.String.Empty)) value
            
        setRules(Proof_Interface.rules)
        setOutput(evaluate_lemma (evaluate_comment input))
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
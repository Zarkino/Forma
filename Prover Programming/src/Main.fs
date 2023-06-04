module Main

open Feliz
open Feliz.Bulma
open Fable.Core.JsInterop
open System.Text.RegularExpressions
open Parsec

importSideEffects "./styles/global.scss"

let rec evaluate_meta input =
    match runString Language.ML.meta () input with
    | Error(msg)    ->  $"Error: %A{msg}"
    | Ok(v, r, _)   ->  (StringSegment.toString r)
                        |> (fun remaining ->
                            let result = Logic.ML.Meta.ToString v
                            if remaining.Trim().Length > 0 then $"%s{result}\n%s{evaluate_meta remaining}"
                            else result)

[<ReactComponent>]
let Main () =
    let (theme, setTheme) = React.useState("light")
    let (input, setInput) = React.useState(Browser.WebStorage.localStorage.getItem("input"))
    let (output, setOutput) = React.useState(System.String.Empty)
    
    React.useEffect(fun () ->
        let parse (string: string) =
            let rec inner string cont =
                match runString Language.Proof.lemma () string with
                | Error(msg)        ->  Some($"Error: %A{msg}"), cont []
                | Ok(lemma, r, _)   ->  (StringSegment.toString r)
                                        |> (fun remaining ->
                                            match remaining.Trim().Length with
                                            | 0 -> None, cont [lemma]
                                            | _ -> inner remaining (fun tail -> cont (lemma::tail)))
            if string.Trim().Length > 0 then inner string id else None, []
        
        let evaluate (lemmas: Proof_Interface.Lemma list) =
            ((Proof_Interface.rules, id), lemmas)
            ||> List.fold
                (fun (rules, cont) lemma ->
                    match Proof_Interface.prove(lemma.Goal, lemma.Proof, List.empty, rules) with
                    | Error(msg)    -> (rules, fun tail -> cont (msg::tail))
                    | Ok _          ->
                        match lemma.Name with
                        | None          -> rules
                        | Some(name)    -> Map.add name lemma.Goal rules
                        |> (fun rules' ->  (rules', fun tail -> cont ($"Successful Lemma %s{lemma.ToString()}"::tail))))
            |> (fun (_, cont) -> String.concat "\n" (cont []))
        
        Browser.WebStorage.localStorage.setItem("input", input)
        
        Regex.Replace(input, "\/\/.*", System.String.Empty)
        |> parse
        |> fun (msg, list) ->
            let output = evaluate list
            match msg, list with
            | None, []      -> System.String.Empty
            | None, _       -> output
            | Some(msg), [] -> msg
            | Some(msg), _  -> $"%s{output}\n%s{msg}"
        |> setOutput
    , [|input :> obj|])
    
    React.fragment [    
        Navigation_Bar.Navigation(theme, setTheme)
        Button_Level.Button_Level(theme, input, setInput)
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
                                    App.Components.Editor(theme, input, (fun value -> setInput(value.ToString())))
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
                                    App.Components.Editor(theme, output, readonly = true)
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
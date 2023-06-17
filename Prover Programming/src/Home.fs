module Home

open Feliz
open Feliz.Bulma

[<ReactComponent>]
let Home() =
    let theme = React.useContext(Contexts.themeContext)
    
    let (input, setInput) = React.useState(Browser.WebStorage.sessionStorage.getItem("input") |> function null -> System.String.Empty | x -> x)
    let (output, setOutput) = React.useState(System.String.Empty)
    
    React.useEffect(fun () ->
        let parse (string: string) =
            let rec inner string cont =
                match Parsec.runString Language.Proof.lemma () string with
                | Error(msg)        ->  Some($"Error: %A{msg}"), cont []
                | Ok(lemma, r, _)   ->  (Parsec.StringSegment.toString r)
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
        
        Browser.WebStorage.sessionStorage.setItem("input", input)
        
        System.Text.RegularExpressions.Regex.Replace(input, "(\/\/.*)|(\/\*[\s\S]*?\*/)", System.String.Empty)
        |> parse
        |> fun (msg, list) ->
            let output = evaluate list
            match msg, list with
            | None, []      -> System.String.Empty
            | None, _       -> output
            | Some(msg), [] -> msg
            | Some(msg), _  -> $"%s{output}\n%s{msg}"
        |> setOutput
    , [|box input|])
    
    React.fragment [
        Button_Level.Button_Level(input, setInput)
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
                                    Monaco_Editor.Editor(input, (fun value -> setInput(value.ToString())), false)
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
                                    Monaco_Editor.Editor(output, (fun _ -> ()), true)
                                ]
                            ]
                        ]
                    ]
                ]
            ]
        ]
    ]
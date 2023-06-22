module Home

open Feliz
open Feliz.Bulma
open Proof_Interface

let private parse (input: string) =
    let rec inner string cont =
        match Parsec.runString Language.Proof.lemma () string with
        | Error(msg)        ->  Some($"Error: %A{msg}"), cont []
        | Ok(lemma, r, _)   ->  match r.Value.Trim() with
                                | str when str.Length > 0   -> inner str (fun tail -> cont (lemma::tail))
                                | _                         -> None, cont [lemma]
    
    System.Text.RegularExpressions.Regex.Replace(input, "(\/\/.*)|(\/\*[\s\S]*?\*/)", System.String.Empty)
    |> fun string ->
        match string.Trim() with
        |str when str.Length > 0    -> inner str id
        | _                         -> None, List.empty

let private evaluate (lemmas: Lemma list) =
    ((rules, id), lemmas)
    ||> List.fold
        (fun (rules, cont) lemma ->
            match prove(lemma.Goal, lemma.Proof, Set.empty, rules) with
            | Error(msg)    -> (rules, fun tail -> cont (Error(msg)::tail))
            | Ok _          ->
                match lemma.Name with
                | None          -> rules
                | Some(name)    -> Map.add name lemma.Goal rules
                |> (fun rules' ->  (rules', fun tail -> cont (Ok(lemma)::tail))))
    |> snd <| []

let private format(msg: string option, list: Lemma list) =
    let output = evaluate list |> List.map (function Ok(lemma) -> $"Successful Lemma %s{lemma.ToString()}" | Error(msg) -> msg) |> String.concat "\n"
    match msg, list with
    | None, []      -> System.String.Empty
    | None, _       -> output
    | Some(msg), [] -> msg
    | Some(msg), _  -> $"%s{output}\n%s{msg}"

[<ReactComponent>]
let Home() =
    let theme = React.useContext(Contexts.themeContext)
    
    let (input, setInput) = React.useState(Browser.WebStorage.sessionStorage.getItem("input") |> function null -> System.String.Empty | x -> x)
    let (output, setOutput) = React.useState((None, List.empty))
    
    React.useEffect(fun () ->
        Browser.WebStorage.sessionStorage.setItem("input", input)
        
        setOutput(parse(input))
    , [|box input|])
    
    React.fragment [
        Button_Level.Button_Level(input, setInput, output)
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
                                    Monaco_Editor.Editor(format(output), (fun _ -> ()), true)
                                ]
                            ]
                        ]
                    ]
                ]
            ]
        ]
    ]
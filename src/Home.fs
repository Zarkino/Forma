module Home

open Feliz
open Feliz.Bulma
open Proof_Interface

type System.String with
    member this.TrimOrDefault(?defaultValue) = if System.String.IsNullOrWhiteSpace(this) then defaultArg defaultValue null else this.Trim()

let private parse (input: string) =
    let rec inner (string: string) cont =
        match string.TrimOrDefault() with
        | null      -> cont []
        | segment   ->
            match Parsec.runString Language.Proof.lemma () segment with
            | Ok(lemma, rest, _)    -> inner (Parsec.StringSegment.toString rest) (fun tail -> cont (Ok(lemma)::tail))
            | Error(msg)            ->
                match segment[1..].IndexOf("lemma") with
                | -1    -> cont [Error(msg)]
                | i     -> inner segment[1+i..] (fun tail -> cont (Error(msg)::tail))

    System.Text.RegularExpressions.Regex.Replace(input, "(\/\/.*)|(\/\*[\s\S]*?\*/)", System.String.Empty)
    |> fun string -> inner string id

let private evaluate (list: Result<Lemma, Parsec.ParseError<_>> list) =
    ((rules, System.Text.StringBuilder()), list)
    ||> List.fold
        (fun (rules, sb) result ->
            match result with
            | Error(msg)    -> (rules, sb.AppendLine(Parsec.ParseError.format msg))
            | Ok(lemma)     ->
                match prove(lemma.Goal, lemma.Proof, Set.empty, rules) with
                | Error(msg)    -> (rules, sb.AppendLine(msg))
                | Ok _          ->
                    match lemma.Name with
                    | None          -> rules
                    | Some(name)    -> Map.add name lemma.Goal rules
                    |> (fun rules' -> (rules', sb.AppendLine($"Successful Lemma %s{lemma.ToString()}"))))
    |> snd |> (fun sb -> sb.ToString().TrimEnd())

[<ReactComponent>]
let Home() =
    let theme = React.useContext(Contexts.themeContext)
    
    let (input, setInput) = React.useState(Browser.WebStorage.sessionStorage.getItem("input") |> function null -> System.String.Empty | x -> x)
    let (output, setOutput) = React.useState(List.empty)
    
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
                                    Monaco_Editor.Editor(evaluate(output), (fun _ -> ()), true)
                                ]
                            ]
                        ]
                    ]
                ]
            ]
        ]
    ]
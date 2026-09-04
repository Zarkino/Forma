module Home

open Feliz
open Feliz.Bulma
open Parsec
open Proof_Interface
open System.Text.RegularExpressions

module Seq =
    let count x = Seq.filter ((=) x) >> Seq.length

let private parse (input: string) =
    let rec inner string cont acc =
        match System.String.IsNullOrWhiteSpace(string) with
        | true  -> cont []
        | false ->
            let line = acc + (string |> Seq.takeWhile System.Char.IsWhiteSpace |> Seq.count '\n')
            let segment = string.TrimStart()
            
            match runString Language.Proof.lemma () segment with
            | Ok(lemma, rest, _)    -> inner (StringSegment.toString rest) (fun tail -> cont (Ok(lemma)::tail)) (line + rest.startLine)
            | Error(msgs, state)    ->
                let msgs' = msgs |> List.map (fun (pos, msg) -> { pos with Line = pos.Line + line }, msg)
                
                Regex.Matches(segment, "(^|[ \t]+)lemma", RegexOptions.Multiline)
                |> Seq.tryFind (fun m -> m.Index > 0)
                |> function
                    | None      -> cont [Error(msgs', state)]
                    | Some(m)   ->
                        let next = Seq.count '\n' segment[..m.Index]
                        inner segment[m.Index..] (fun tail -> cont (Error(msgs', state)::tail)) (line + next)
    
    Regex.Replace(input, "(\/\/.*)|(\/\*[\s\S]*?\*\/)", MatchEvaluator(fun m -> String.replicate (Seq.count '\n' m.Value) "\n"))
    |> fun string -> inner string id 1

let private evaluate (list: Result<Lemma, ParseError<_>> list) =
    ((rules, System.Text.StringBuilder()), list)
    ||> List.fold
        (fun (rules, sb) result ->
            match result with
            | Error(msg)    -> (rules, sb.AppendLine(ParseError.format msg))
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
                                    Editor.Editor(input, (fun value -> setInput(value.ToString())), false)
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
                                    Editor.Editor(evaluate(output), (fun _ -> ()), true)
                                ]
                            ]
                        ]
                    ]
                ]
            ]
        ]
    ]
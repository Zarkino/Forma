namespace Language

open Parsec

module PL =
    open Logic.PL
    
    let (formula: Parser<Formula, obj>), formulaRef = createParserForwardedToRef()
    
    let constant = pchar 'T' <|> pchar 'F' |>> function | 'T' -> Constant(true) | _ -> Constant(false)
    let var = many1Chars asciiLetter |>> string
    let variable = var |>> Variable
    let negation = pchar '~' >>. spaces >>. (constant <|> variable <|> formula) |>> Negation
    let binaryFormula operators = (operators |> List.map pstring |> choice) >>. spaces >>. formula
    
    do formulaRef.Value <- parse {
        let! left = spaces >>. (constant <|> variable <|> negation <|> (between (pchar '(') (pchar ')') formula))
        return! spaces >>. choice [
            binaryFormula ["&"; "∧"] |>> (fun right -> Conjunction(left, right))
            binaryFormula ["|"; "∨"] |>> (fun right -> Disjunction(left, right))
            binaryFormula ["->"; "→"] |>> (fun right -> Implication(left, right))
            binaryFormula ["<->"; "↔"] |>> (fun right -> Equivalence(left, right))
            preturn left
        ]
    }

module ML =
    open PL
    open Logic.ML
    
    let meta, metaRef = createParserForwardedToRef()
    
    let entity = formula |>> Meta.Entity
    
    do metaRef.Value <- parse {
        let! left = spaces >>. (
            (pipe2 (pstring "!!" >>. var .>> pchar '.') (spaces1 >>. meta) (fun left right -> Meta.Universal(left, right))) <|>
            entity <|>
            between (pchar '(') (pchar ')') meta)
        return! spaces >>. choice [
            pstring "==>" >>. spaces >>. meta |>> (fun right -> Meta.Implication(left, right))
            pstring "==" >>. spaces >>. meta |>> (fun right -> Meta.Equality(left, right))
            preturn left
        ]
    }

module Proof =
    open ML
    open Proof_Interface
    
    let proof, proofRef = createParserForwardedToRef()
    
    let rule = manyMinMaxSatisfy 1 10 (fun c -> isLetter c || isDigit c || c = '_')
    
    let tactic_rule = pchar '(' >>. pstring "rule" >>. spaces1 >>. rule .>> pchar ')' |>> Tactic.Rule
    let tactic_assumption = pstring "assumption" |>> (fun _ -> Tactic.Assumption)
    
    let tactic = choice [
        tactic_rule
        tactic_assumption
    ]
    
    let command keyword = choice [
        pipe3 (opt (pstring "from" >>. (sepBy1 meta (pstring "and")))) (pstring keyword >>. spaces1 >>. meta) (pstring "by" >>. spaces1 >>. tactic) (fun a f r -> Instant(a, f, r))
        pipe2 (pstring keyword >>. spaces1 >>. meta) proof (fun f p -> Delayed(f, p))
    ]
    
    let statements = many (spaces >>. choice [
        pstring "assume" >>. spaces1 >>. meta |>> Assumption
        command "have" |>> Intermediate
        command "show" |>> Conclusion
    ] .>> spaces)
    
    proofRef.Value <- spaces >>. pstring "proof" >>. spaces1 >>. tactic_rule .>>. (spaces >>. between (pchar '{') (pchar '}') statements) |>> Proof
    
    let name = spaces >>. opt (many1CharsTillMax anyChar ':' 10)
    
    let lemma = spaces >>. pstring "lemma" >>. spaces1 >>. pipe3 name meta proof (fun name id proof -> { Name = name; Goal = id; Proof = proof })
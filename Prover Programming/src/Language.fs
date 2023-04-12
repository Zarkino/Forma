namespace Language

open Parsec

module Grammar_PL =
    open Propositional_Logic
    
    let formula, formulaRef = createParserForwardedToRef()
    
    let constant = pchar 'T' <|> pchar 'F' |>> function | 'T' -> Constant(true) | _ -> Constant(false)
    let var = many1Chars asciiLetter |>> string
    let variable = var |>> Variable
    let negation = pchar '~' >>. spaces >>. (constant <|> variable <|> formula) |>> Negation
    let binaryFormula operator = pstring operator >>. spaces >>. formula
    
    do formulaRef.Value <- parse {
        let! left = spaces >>. (constant <|> variable <|> negation <|> (between (pchar '(') (pchar ')') formula))
        return! spaces >>. choice [
            binaryFormula "&" |>> (fun right -> Conjunction(left, right))
            binaryFormula "|" |>> (fun right -> Disjunction(left, right))
            binaryFormula "->" |>> (fun right -> Implication(left, right))
            binaryFormula "<->" |>> (fun right -> Equivalence(left, right))
            preturn left
        ]
    }

module Grammar_ML =
    open Grammar_PL
    open Meta_Logic
    
    let meta, metaRef = createParserForwardedToRef()
    
    let formula_ml = formula |>> Meta.Formula
    
    do metaRef.Value <- parse {
        let! left = spaces >>. (
            (pipe2 (pstring "!!" >>. var .>> pchar '.') (spaces1 >>. meta) (fun left right -> Meta.Universal(left, right))) <|>
            formula_ml <|>
            between (pchar '(') (pchar ')') meta)
        return! spaces >>. choice [
            pstring "==>" >>. spaces >>. meta |>> (fun right -> Meta.Implication(left, right))
            pstring "==" >>. spaces >>. meta |>> (fun right -> Meta.Equality(left, right))
            preturn left
        ]
    }

module Grammar_Proof =
    open Grammar_PL
    open Proof_Interface
    
    let proof, proofRef = createParserForwardedToRef()
    
    let rule = rules.Keys |> Seq.map pstring |> List.ofSeq |> choice
    
    let statements = many (spaces >>. choice [
            pstring "assume" >>. spaces1 >>. formula |>> Assumption
            pipe2 (pstring "have" >>. spaces1 >>. formula) (spaces <|> spaces1 >>. pstring "by" >>. spaces1 >>. rule) (fun left right -> Intermediate(left, right))
            pipe2 (pstring "show" >>. spaces1 >>. formula) (spaces <|> spaces1 >>. pstring "by" >>. spaces1 >>. rule) (fun left right -> Conclusion(left, right))
            proof |>> Subproof
        ])
    
    proofRef.Value <- spaces >>. pstring "proof" >>. spaces >>. pchar '{' >>. statements .>> spaces .>> pchar '}' |>> (fun statements -> { Statements = statements })
    
    let lemma = pipe2 (spaces >>. pstring "lemma" >>. spaces1 >>. formula) proof (fun id proof -> { Identifier = id; Proof = proof })

module Parser =
    open Grammar_PL
    open Grammar_ML
    open Grammar_Proof
    
    let parse_formula input =
        match runString formula () input with
        | Ok(v, r, _)   -> Some v, StringSegment.toString r
        | Error(e)      -> None, $"Error: %A{e}"
    
    let parse_lemma input =
        match runString lemma () input with
        | Ok(v, r, _)   -> Some v, StringSegment.toString r
        | Error(e)      -> None, $"Error: %A{e}"
    
    let parse_meta input =
        match runString meta () input with
        | Ok(v, r, _)   -> Some v, StringSegment.toString r
        | Error(e)      -> None, $"Error: %A{e}"
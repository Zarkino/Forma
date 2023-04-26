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
    
    let rule = manyMinMaxSatisfy 1 10 (fun c -> isLetter c || isDigit c || c = '_')
    
    let proofRule = pchar '(' >>. pstring "rule" >>. spaces1 >>. rule .>> pchar ')'
    
    let keyword = choice [pstring "have"; pstring "show"]
    
    let statements = many (spaces >>. choice [
            pstring "assume" >>. spaces1 >>. formula |>> Assumption
            pipe3 (pstring "from" >>. (sepBy1 formula (pstring "and"))) (keyword >>. spaces1 >>. formula) (pstring "by" >>. spaces1 >>. rule) (fun a f r -> Instant(a, f, r))
            keyword >>. spaces1 >>. formula |>> Delayed
            proof |>> Subproof
        ] .>> spaces)
    
    proofRef.Value <- spaces >>. pstring "proof" >>. spaces1 >>. proofRule .>>. (spaces >>. between (pchar '{') (pchar '}') statements) |>> Proof
    
    let name = spaces >>. opt (many1CharsTillMax anyChar ':' 10)
    
    let lemma = spaces >>. pstring "lemma" >>. spaces1 >>. pipe3 name formula proof (fun name id proof -> { Name = name; Identifier = id; Proof = proof })

module Parser =
    open Grammar_PL
    open Grammar_ML
    open Grammar_Proof
    
    let parse_formula input =
        match runString formula () input with
        | Ok(v, r, _)   -> Some v, StringSegment.toString r
        | Error(e)      -> None, $"Error: %A{e}"
    
    let parse_proof input =
        match runString proof () input with
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
namespace Language

open Parsec

module Grammar_PL =
    open Propositional_Logic
    
    let formula, formulaRef = createParserForwardedToRef()
    
    let variable = regex "^[a-zA-Z]" |>> Variable
    let negation = pchar '!' >>. spaces >>. (variable <|> formula) |>> Negation
    let binaryFormula operator = pstring operator >>. spaces >>. formula
    
    do formulaRef.Value <- parse {
        let! left = spaces >>. choice [
            variable
            negation
            pchar '(' >>. formula .>> pchar ')'
        ]
        return! spaces >>. choice [
            binaryFormula "&" |>> (fun right -> Conjunction(left, right))
            binaryFormula "|" |>> (fun right -> Disjunction(left, right))
            binaryFormula "->" |>> (fun right -> Implication(left, right))
            binaryFormula "<->" |>> (fun right -> Equivalence(left, right))
            preturn left
        ]
    }

module Grammar_Proof =
    open Grammar_PL
    open Proof_Interface
    
    let proof, proofRef = createParserForwardedToRef()
    
    let statements = many (spaces >>. choice [
            pstring "assume" >>. formula |>> Assumption
            pstring "have" >>. formula |>> Intermediate
            pstring "show" >>. formula |>> Conclusion
            proof |>> Subproof
        ])
    
    proofRef.Value <- pstring "proof" >>. spaces >>. pchar '{' >>. statements .>> spaces .>> pchar '}' |>> (fun statements -> { Statements = statements })
    
    let lemma = spaces >>. pstring "lemma" >>. formula .>>. (spaces >>. proof) |>> (fun (id, proof) -> { Identifier = id; Proof = proof })

module Parser =
    open Grammar_PL
    open Grammar_Proof
    
    let parse_formula input =
        match runString formula () input with
        | Ok(v, _, _)   -> v.ToString()
        | Error(e)      -> $"Error: %A{e}"
    
    let parse_lemma input =
        match runString lemma () input with
        | Ok(v, _, _)   -> v.ToString()
        | Error(e)      -> $"Error: %A{e}"
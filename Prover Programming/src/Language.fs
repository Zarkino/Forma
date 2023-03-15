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
    open Proof_Interface
    
    let (proof: Parser<Proof, unit>), proofRef = createParserForwardedToRef()
    
    let proposition = spaces >>. regex "^[a-z]+"
    
    let statement =
        choice [
            spaces >>. pstring "assume" >>. proposition |>> Assumption
            spaces >>. pstring "have" >>. proposition |>> Intermediate
            proof |>> Subproof
        ]
    
    do proofRef.Value <- parse {
        let! statements = statement
        let! conclusion = spaces >>. pstring "show" >>. proposition
        return { Statements = statements; Conclusion = conclusion }
    }
    
    let lemma = spaces >>. pstring "lemma" >>. spaces >>. pchar '{' >>. proof .>> pchar '}'

module Parser =
    open Grammar_PL
    
    let parse_formula input =
        match runString formula () input with
        | Ok(v, _, _)   -> v
        | Error(e)      -> failwith $"Error: %A{e}"
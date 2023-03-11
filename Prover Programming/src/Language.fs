namespace Language

module Grammar =
    open Parsec
    
    type Formula =
        | Variable of string
        | Negation of Formula
        | Conjunction of Formula * Formula
        | Disjunction of Formula * Formula
        | Implication of Formula * Formula
        | Equivalence of Formula * Formula
    
    let formula, formulaRef = createParserForwardedToRef()
    
    let variable = regex "\s*^[a-z]" |>> Variable
    let negation = pstring "!" >>. formula |>> Negation
    
    let binaryFormula operator = spaces >>. pstring operator .>> spaces >>. formula
    
    do formulaRef.Value <- parse {
        let! left = choice [
            variable
            pstring "!" >>. variable |>> Negation
            pstring "!" >>. formula |>> Negation
            pchar '(' >>. formula .>> pchar ')'
        ]
        return! choice [
            binaryFormula "&" |>> (fun right -> Conjunction(left, right))
            binaryFormula "|" |>> (fun right -> Disjunction(left, right))
            binaryFormula "->" |>> (fun right -> Implication(left, right))
            binaryFormula "<->" |>> (fun right -> Equivalence(left, right))
            preturn left
        ]
    }

module Parser =
    open Parsec
    
    open Grammar
    
    let tryParse input =
        match runString formula () input with
            | Ok(v, s, state)   -> Ok(v, s, state)
            | Error e           -> Error e
    
    let x = match tryParse "!(a -> !b)" with
            | Ok(v, s, _)   -> sprintf $"Success: %A{v}\nRemaining: %s{s.Value}"
            | Error(e)      -> sprintf $"Error: %A{e}"
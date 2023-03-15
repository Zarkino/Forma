namespace Language

module Grammar =
    open Parsec
    
    open Propositional_Logic
    
    let formula, formulaRef = createParserForwardedToRef()
    
    let variable = spaces >>. regex "^[a-z]" |>> Variable
    let negation = spaces >>. pstring "!" >>. spaces >>. (variable <|> formula) |>> Negation
    let binaryFormula operator = spaces >>. pstring operator .>> spaces >>. formula
    
    do formulaRef.Value <- parse {
        let! left = choice [
            variable
            negation
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
    
    let parse_formula input =
        match runString formula () input with
        | Ok(v, _, _)   -> v
        | Error(e)      -> failwith $"Error: %A{e}"
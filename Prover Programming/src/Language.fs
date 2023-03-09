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

    let (formula: Parser<Formula, obj>), formulaRef = createParserForwardedToRef()
    
    let variable: Parser<Formula, obj> =
        (anyChar |> manyChars) |>> Variable
        
    let primary =
        choice [
            variable
            between (skipChar '(') (skipChar ')') formula
        ]

    let negation: Parser<Formula, obj> =
        (skipChar '!' >>. anyChar |> manyChars) |>> fun formula -> Negation(Variable formula)
    
    let conjunction: Parser<Formula, obj> =
        (anyChar |> manyCharsTill <| skipChar '&')
        .>>.
        (anyChar |> manyChars)
        |>> fun (name, value) -> Conjunction(Variable(name), Variable(value))

module PL =
    open Parsec
    
    open Grammar
    
    let result input = run variable input
    
    let tryParse source =
        // Discard leading whitespace and ensure the parser reaches end of stream
        let variable = spaces >>. variable .>> eof
        
        fun state ->
            match run variable state source with
            | Ok(_, s, state)   -> Ok((), s, state)
            | Error e           -> Error e
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

    let stringLiteral: Parser<string,obj> =
        let escape =  anyOf "\"\\/bfnrt"
                      |>> function
                          | 'b' -> "\b"
                          | 'f' -> "\u000C"
                          | 'n' -> "\n"
                          | 'r' -> "\r"
                          | 't' -> "\t"
                          | c   -> string c // every other char is mapped to itself

        let unicodeEscape =
            /// converts a hex char ([0-9a-fA-F]) to its integer number (0-15)
            let hex2int c = (int c &&& 15) + (int c >>> 6)*9

            pstring "u" >>. pipe4 hex hex hex hex (fun h3 h2 h1 h0 ->
                (hex2int h3)*4096 + (hex2int h2)*256 + (hex2int h1)*16 + hex2int h0
                |> char |> string
            )

        let escapedCharSnippet = pstring "\\" >>. (escape <|> unicodeEscape)
        let normalCharSnippet  = manySatisfy (fun c -> c <> '"' && c <> '\\')

        between (pstring "\"") (pstring "\"")
                (stringsSepBy normalCharSnippet escapedCharSnippet)
    
    let variable = stringLiteral |>> Variable
    let negation = (skipString "!") >>. formula |>> Negation
    let conjunction = (formula .>> skipString "&") .>>. formula |>> Conjunction
    let disjunction = (formula .>> skipString "|") .>>. formula |>> Disjunction
    let implication = (formula .>> skipString "->") .>>. formula |>> Implication
    let equivalence = (formula .>> skipString "<->") .>>. formula |>> Equivalence
    
    do formulaRef.Value <-
        choice [
            variable
            negation
            conjunction
            disjunction
            implication
            equivalence
            between (skipChar '(') (skipChar ')') formula
        ]

module Parser =
    open Parsec
    
    open Grammar
    
    // Discard leading whitespace and ensure the parser reaches end of stream
    let expression = spaces >>. formula >>. spaces .>> eof
    
    let parseExpression input = runString expression input
    
    let tryParse input =
        match runString expression None input with
            | Ok(_, s, state)   -> Ok((), s, state)
            | Error e           -> Error e
    
    let x = match tryParse "!a" with
            | Ok(v, _, _)   -> sprintf "%A" v
            | Error(e)      -> sprintf "%A" e
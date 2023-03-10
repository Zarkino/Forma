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
    
    let variable = regex "\s*^[a-z]" |>> Variable
    let negation = pstring "!" >>. formula |>> Negation
    let conjunction = (formula .>> pstring "&") .>>. formula |>> Conjunction
    let disjunction = (formula .>> pstring "|") .>>. formula |>> Disjunction
    let implication = (formula .>> pstring "->") .>>. formula |>> Implication
    let equivalence = (formula .>> pstring "<->") .>>. formula |>> Equivalence
    
    (*
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
    *)
    
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
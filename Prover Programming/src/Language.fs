namespace Language

open Parsec

module PL =
    open Logic.PL
    
    let (formula: Parser<Formula, obj>), formulaRef = createParserForwardedToRef()
    
    let constant = (anyOf ['T'; '⊤'] >>% Constant(true)) <|> (anyOf ['F'; '⊥'] >>% Constant(false))
    let var = many1Chars asciiLetter |>> string
    let variable = var |>> Variable
    let negation = (anyOf ['~'; '¬']) >>. spaces >>. (constant <|> variable <|> formula) |>> Negation
    let binaryFormula operator = operator >>. spaces >>. formula
    
    do formulaRef.Value <- parse {
        let! left = spaces >>. choice [
            constant
            variable
            negation
            between (pchar '(') (pchar ')') formula
        ]
        return! spaces >>. choice [
            binaryFormula (anyOfStr ["&"; "∧"]) |>> (fun right -> Conjunction(left, right))
            binaryFormula (anyOfStr ["|"; "∨"]) |>> (fun right -> Disjunction(left, right))
            binaryFormula (anyOfStr ["-->"; "⟶"]) |>> (fun right -> Implication(left, right))
            binaryFormula (anyOfStr ["<-->"; "⟷"]) |>> (fun right -> Equivalence(left, right))
            preturn left
        ]
    }

module ML =
    open PL
    open Logic.ML
    
    let meta, metaRef = createParserForwardedToRef()
    
    let entity = formula |>> Meta.Entity
    
    do metaRef.Value <- parse {
        let! left = spaces >>. choice [
            pipe2 ((anyOfStr ["!!"; "⋀"]) >>. many1 var .>> pchar '.') (spaces1 >>. meta) (fun left right -> Meta.Universal(left, right))
            entity
            between (pchar '(') (pchar ')') meta
        ]
        return! spaces >>. choice [
            (anyOfStr ["==>"; "⟹"]) >>. spaces >>. meta |>> (fun right -> Meta.Implication(left, right))
            (anyOfStr ["=="; "≡"]) >>. spaces >>. meta |>> (fun right -> Meta.Equality(left, right))
            preturn left
        ]
    }

module Proof =
    open ML
    open Proof_Interface
    
    let proof, proofRef = createParserForwardedToRef()
    
    let name = spaces >>. manyMinMaxSatisfy 1 10 (function ' ' | '\n' | ':' | '(' | ')' -> false | _ -> true)
    
    let method_rule = between (pchar '(') (pchar ')') (spaces >>. pstring "rule" >>. spaces1 >>. name .>> spaces) |>> Method.Rule
    let method_this = pstring "this" >>% Method.This
    let method_trivial = (anyOfStr ["-"; "none"]) >>% Method.Trivial
    
    let proof_method = choice [
        method_rule
        method_this
        method_trivial
    ]
    
    let command_method = choice [
        method_rule
        method_this
    ]
    
    let command keyword = choice [
        pipe3 (opt (pstring "from" >>. (sepBy1 meta (pstring "and")))) (pstring keyword >>. spaces1 >>. meta) (pstring "by" >>. spaces1 >>. command_method) (fun a f r -> Instant(a, f, r))
        pipe2 (pstring keyword >>. spaces1 >>. meta) proof (fun f p -> Delayed(f, p))
    ]
    
    let statements = many (spaces >>. choice [
        pstring "assume" >>. spaces1 >>. (sepBy1 meta (pstring "and")) |>> Assumption
        command "have" |>> Intermediate
    ] .>> spaces)
    
    let conclusion = spaces >>. command "show" .>> spaces |>> Conclusion
    
    let next = spaces >>. pstring "next" .>> spaces >>% Next
    
    let block, blockRef = createParserForwardedToRef()
    blockRef.Value <- parse {
        let! left = pipe2 statements conclusion (fun statements conclusion -> statements@[conclusion])
        return! choice [
            next .>>. block >>= (fun (next, block) -> preturn (left@next::block))
            preturn left
        ]
    }
    
    proofRef.Value <-
        spaces >>. pstring "proof" >>. spaces1 >>.
        pipe2
            proof_method
            (spaces >>. between (pchar '{') (pchar '}') block)
            (fun method statements -> Proof(method, statements))
    
    let lemma = spaces >>. pstring "lemma" >>. spaces1 >>. pipe3 (opt (name .>> spaces .>> pchar ':')) meta proof (fun name goal proof -> { Name = name; Goal = goal; Proof = proof })
module Proof_Interface

open Propositional_Logic

type Proof = {
    Statements: Statement list
}
and Statement =
    | Assumption of Formula
    | Intermediate of Formula * string
    | Conclusion of Formula * string
    | Subproof of Proof

type Lemma = {
    Identifier: Formula
    Proof: Proof
}

let rules = [
        ("Con_I",   2)
        ("Con_E1",  1)
        ("Con_E2",  1)
        ("Dis_I1",  1)
        ("Dis_I2",  1)
        ("Dis_E1",  2)
        ("Dis_E2",  2)
        ("Imp_I",   2)
        ("Imp_E1",  2)
        ("Imp_E2",  2)
        ("Iff_I",   2)
        ("Iff_E",   1)] |> Map.ofList

// Most of the following simply inspired by https://en.wikipedia.org/wiki/List_of_rules_of_inference
let apply (rule, assumptions, result) =
    match rule, assumptions, result with
    | "Con_I",  [p; q],                                 [Conjunction(p', q')]                       when p' = p && q' = q                       -> true // Conjunction Introduction
    | "Con_E1", [Conjunction(p, _)],                    [p']                                        when p' = p                                 -> true // Conjunction Elimination (right)
    | "Con_E2", [Conjunction(_, q)],                    [q']                                        when q' = q                                 -> true // Conjunction Elimination (left)
    | "Dis_I1", [p],                                    [Disjunction(p', _)]                        when p' = p                                 -> true // Disjunction Introduction
    | "Dis_I2", [p],                                    [Disjunction(_, p')]                        when p' = p                                 -> true // Disjunction Introduction
    | "Dis_E1", [Disjunction(p, q)
                 Negation(r)],                          [q']                                        when (p = r) && (q' = q)                    -> true // Disjunctive Syllogism (left)
    | "Dis_E2", [Disjunction(p, q)
                 Negation(r)],                          [p']                                        when (q = r) && (p' = p)                    -> true // Disjunctive Syllogism (right)
    | "Imp_I",  [p; q],                                 [Implication(p', q')]                       when p' = p && q' = q                       -> true // Implication Introduction
    | "Imp_E1", [Implication(p, q); r],                 [q']                                        when (p = r) && (q' = q)                    -> true // Modus Ponens (Elimination)
    | "Imp_E2", [Implication(p, q)
                 Negation(r)],                          [Negation(p')]                              when (q = r) && (p' = p)                    -> true // Modus Tollens
    | "Iff_I",  [Implication(p, q)
                 Implication(r, s)],                    [Equivalence(p', q')]                       when (p = s && q = r) && (p' = p && q' = q) -> true //
    | "Iff_E",  [Equivalence(p, q)],                    [Implication(p', q')]                       when p' = p && q' = q                       -> true //
    | _, _, _                                                                                                                                   -> false //failwith $"The rule %s{rule} could not be applied to %A{assumptions}"

/// Creates all permutations of the list with length n
let rec combinations n list =
    match n, list with
    | 0, _      -> [[]]
    | _, []     -> []
    | k, x::xs  -> List.map ((@) [x]) (combinations (k-1) list) @ combinations k xs

type Result =
    | Success of Formula list * bool
    | Fail of string

let rec prove (proof: Proof, assumptions: Formula List) =
    (Success(assumptions, false), proof.Statements)
    ||> List.fold
        (fun state statement ->
            match state with
            | Fail _                        -> state
            | Success(formulas, judgement)  ->
                let fs = List.distinct formulas
                match statement with
                | Assumption(f)         ->  Success(f::fs, judgement)
                | Intermediate(f, rule) ->  match combinations rules[rule] fs |> List.exists (fun a -> apply(rule, a, [f])) with
                                            | true  -> Success(f::fs, judgement)
                                            | false -> Fail($"Could not have %A{f} by %s{rule}")
                | Conclusion(f, rule)   ->  match combinations rules[rule] fs |> List.exists (fun a -> apply(rule, a, [f])) with
                                            | true  -> Success(f::fs, true)
                                            | false -> Fail($"Conclusion %A{f} could not be reached using rule %s{rule}")
                | Subproof(p)           ->  prove(p, fs)
        )
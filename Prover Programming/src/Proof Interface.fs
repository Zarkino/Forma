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
        ("Con_I",   2);
        ("Con_E1",  1);
        ("Con_E2",  1);
        ("Dis_I",   1);
        ("Dis_E1",  2);
        ("Dis_E2",  2);
        ("Dis_E3",  2);
        ("Dis_E4",  3);
        ("Imp_I",   2);
        ("Imp_E1",  2);
        ("Imp_E2",  2);
        ("Eqv_I",   2);
        ("Eqv_E",   1);
        ("DNe_I",   1);
        ("DNe_E",   1)] |> Map.ofList

// Most of the following simply inspired by https://en.wikipedia.org/wiki/List_of_rules_of_inference
let apply (rule, assumptions, result) =
    match rule, assumptions, result with
    | "Con_I",  [p; q],                                 [Conjunction(p', q')]                       when p' = p && q' = q                                           -> true // Conjunction Introduction
    | "Con_E1", [Conjunction(p, _)],                    [p']                                        when p' = p                                                     -> true // Conjunction Elimination (right)
    | "Con_E2", [Conjunction(_, q)],                    [q']                                        when q' = q                                                     -> true // Conjunction Elimination (left)
    | "Dis_I",  [p],                                    [Disjunction(p', _)]                        when p' = p                                                     -> true // Disjunction Introduction
    | "Dis_E1", [Disjunction(p, q)
                 Negation(r)],                          [q']                                        when (p = r) && (q' = q)                                        -> true // Disjunctive Syllogism (left)
    | "Dis_E2", [Disjunction(p, q)
                 Negation(r)],                          [p']                                        when (q = r) && (p' = p)                                        -> true // Disjunctive Syllogism (right)
    | "Dis_E3", [Implication(p, q)
                 Implication(r, s)],                    [Disjunction(p', r'); q']                   when (q = s) && (p' = p && q' = q && r' = r)                    -> true // Disjunction Elimination
    | "Dis_E4", [Implication(p, q) as I1
                 Implication(r, s) as I2
                 Disjunction(t, u)],                    [Disjunction(p', r'); Disjunction(q', s')]  when p' = p && q' = q && r' = r && s' = s && t = I1 && u = I2   -> true // Constructive Dilemma
    | "Imp_I",  [p; q],                                 [Implication(p', q')]                       when p' = p && q' = q                                           -> true // Implication Introduction
    | "Imp_E1", [Implication(p, q); r],                 [q']                                        when (p = r) && (q' = q)                                        -> true // Modus Ponens (Elimination)
    | "Imp_E2", [Implication(p, q)
                 Negation(r)],                          [Negation(p')]                              when (q = r) && (p' = p)                                        -> true // Modus Tollens
    | "Eqv_I",  [Implication(p, q)
                 Implication(r, s)],                    [Equivalence(p', q')]                       when (p = s && q = r) && (p' = p && q' = q)                     -> true //
    | "Eqv_E",  [Equivalence(p, q)],                    [Implication(p', q')]                       when p' = p && q' = q                                           -> true //
    | "DNe_I",  [p],                                    [Negation(Negation(p'))]                    when p' = p                                                     -> true //
    | "DNe_E",  [Negation(Negation(p))],                [p']                                        when p' = p                                                     -> true //
    | _, _, _                                                                                                                                                       -> false //failwith $"The rule %s{rule} could not be applied to %A{assumptions}"

/// Creates all permutations of the list with length n
let rec combinations n list =
    match n, list with
    | 0, _      -> [[]]
    | _, []     -> []
    | k, x::xs  -> List.map ((@) [x]) (combinations (k-1) xs) @ combinations k xs

type Result =
    | Success of Formula list * bool
    | Fail of string

let rec prove (proof: Proof, assumptions: Formula List) =
    (Success(assumptions, false), proof.Statements)
    ||> List.fold
        (fun s v ->
            match s with
            | Fail _        -> s
            | Success(s, d) ->
                match v with
                | Assumption(f)         ->  Success(f::s, d)
                | Intermediate(f, rule) ->  match combinations rules[rule] s |> List.exists (fun a -> apply(rule, a, [f])) with
                                            | true  -> Success(f::s, d)
                                            | false -> Fail($"Could not have %A{f} by %s{rule}")
                | Conclusion(f, rule)   ->  match combinations rules[rule] s |> List.exists (fun a -> apply(rule, a, [f])) with
                                            | true  -> Success(f::s, true)
                                            | false -> Fail($"Conclusion %A{f} could not be reached using rule %s{rule}")
                | Subproof(p)           ->  prove(p, s)
        )
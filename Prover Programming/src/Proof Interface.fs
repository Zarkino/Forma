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

let rules = Map.ofList [
    ("Neg_I",   ([Implication(Variable("0"), Constant(false))],                 Negation(Variable("0"))))
    ("Neg_E",   ([Implication(Variable("0"), Constant(false))
                  Variable("0")],                                               Variable("1"))) 
    ("Con_I",   ([Variable("0"); Variable("1")],                                Conjunction(Variable("0"), Variable("1"))))
    ("Con_E1",  ([Conjunction(Variable("0"), Variable("1"))],                   Variable("0")))
    ("Con_E2",  ([Conjunction(Variable("0"), Variable("1"))],                   Variable("1")))
    ("Dis_I1",  ([Variable("0")],                                               Disjunction(Variable("0"), Variable("1"))))
    ("Dis_I2",  ([Variable("1")],                                               Disjunction(Variable("0"), Variable("1"))))
    ("Dis_E1",  ([Disjunction(Variable("0"), Variable("1"))
                  Negation(Variable("0"))],                                     Variable("1")))
    ("Dis_E2",  ([Disjunction(Variable("0"), Variable("1"))
                  Negation(Variable("1"))],                                     Variable("0")))
    ("Imp_I",   ([Variable("0"); Variable("1")],                                Implication(Variable("0"), Variable("1"))))
    ("Imp_E1",  ([Implication(Variable("0"), Variable("1")); Variable("0")],    Variable("1")))
    ("Imp_E2",  ([Implication(Variable("0"), Variable("1"))
                  Negation(Variable("1"))],                                     Negation(Variable("0"))))
    ("Iff_I",   ([Implication(Variable("0"), Variable("1"))
                  Implication(Variable("1"), Variable("0"))],                   Equivalence(Variable("0"), Variable("1"))))
    ("Iff_E1",  ([Equivalence(Variable("0"), Variable("1"))],                   Implication(Variable("0"), Variable("1"))))
    ("Iff_E2",  ([Equivalence(Variable("0"), Variable("1"))],                   Implication(Variable("1"), Variable("0"))))
    ("Abs_P",   ([Variable("0"); Negation(Variable("0"))],                      Implication(Variable("0"), Constant(false))))
    ("Abs_N",   ([Variable("0"); Negation(Variable("0"))],                      Implication(Negation(Variable("0")), Constant(false))))
]
    
let apply rule a r =
    match Map.tryFind rule rules with
    | None          ->  false
    | Some(a', r')  ->  match unify r' r Map.empty with
                        | false, _  -> false
                        | true, map -> List.forall (fun x -> Set.exists (fun y -> unify x y map |> fst) a) a'

type Result =
    | Success of Set<Formula> * bool
    | Fail of string

let rec prove (proof: Proof, assumptions: Set<Formula>) =
    (Success(assumptions, false), proof.Statements)
    ||> List.fold
        (fun state statement ->
            match state with
            | Fail _                        -> state
            | Success(fs, judgement)  ->
                match statement with
                | Assumption(f)         ->  Success(Set.add f fs, judgement)
                | Intermediate(f, rule) ->  match apply rule fs f with
                                            | true  -> Success(Set.add f fs, judgement)
                                            | false -> Fail($"Could not have %A{f} by %s{rule}")
                | Conclusion(f, rule)   ->  match apply rule fs f with
                                            | true  -> Success(Set.add f fs, true)
                                            | false -> Fail($"Conclusion %A{f} could not be reached using rule %s{rule}")
                | Subproof(p)           ->  match prove(p, fs) with
                                            | Success _ -> state
                                            | error     -> error
        )
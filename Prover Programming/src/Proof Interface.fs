module Proof_Interface

open Propositional_Logic

type Proof = Statement list
and Statement =
    | Assumption of Formula
    | Intermediate of Formula * string
    | Conclusion of Formula * string
    | Subproof of Proof

let unfoldConclusion = function
    | []    ->  None
    | xs    ->  List.last xs |> function
                | Conclusion(p, _)  -> Some(p)
                | _                 -> None

type Lemma = {
    Name: string option
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
    
let apply rule a r rules =
    match Map.tryFind rule rules with
    | None          ->  false, $"Rule \"%s{rule}\" does not exist"
    | Some(a', r')  ->  match unify r' r Map.empty with
                        | false, _  ->  false, $"Could not match %s{Formula.ToString r} with %s{Formula.ToString r'}"
                        | true, map ->  match List.forall (fun x -> Set.exists (fun y -> unify x y map |> fst) a) a' with
                                        | false -> false, $"Could not apply rule %s{rule}: Not all conditions were met"
                                        | true  -> true, System.String.Empty

type Result =
    | Success of Set<Formula> * bool
    | Fail of string

let rec prove (proof: Proof, assumptions: Set<Formula>, rules: Map<string, Formula list * Formula>) =
    (Success(assumptions, false), proof)
    ||> List.fold
        (fun state statement ->
            match state with
            | Fail _                    ->  state
            | Success(fs, judgement)    ->
                match statement with
                | Assumption(f)         ->  Success(Set.add f fs, judgement)
                | Intermediate(f, rule) ->  match apply rule fs f rules with
                                            | true, _       ->  Success(Set.add f fs, judgement)
                                            | false, msg    ->  Fail(msg)
                | Conclusion(f, rule)   ->  match apply rule fs f rules with
                                            | true, _       ->  Success(Set.add f fs, true)
                                            | false, msg    ->  Fail(msg)
                | Subproof(proof')      ->  match prove(proof', fs, rules) with
                                            | Success _ ->  match unfoldConclusion proof' with
                                                            | Some(f)   -> Success(Set.add f fs, judgement)
                                                            | None      -> Fail("Sub-proof must end with a conclusion")
                                            | Fail(msg) ->  Fail($"Sub-proof does not hold: %s{msg}"))
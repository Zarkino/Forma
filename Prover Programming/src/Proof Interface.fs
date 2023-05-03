module Proof_Interface

open Propositional_Logic

type Proof = string * Statement list
and Statement =
    | Assumption of Formula
    | Instant of Formula list option * Formula * string
    | Delayed of Formula * Proof

type Lemma = {
    Name: string option
    Goal: Formula
    Proof: Proof
}
with
    static member ToString lemma = (lemma.Name |> function Some(name) -> $"%s{name}: " | _ -> System.String.Empty) |> (fun s -> $"%s{s}%s{lemma.Goal.ToString()}")
    override this.ToString() = Lemma.ToString this

let isValid lemma =
    match separate lemma.Goal with
        | None          ->  false, $"Incomplete lemma %s{lemma.ToString()}"
        | Some(a, c)    ->
            (match List.tryHead (snd lemma.Proof) with
            | Some(Assumption(a'))  when a = a' ->  true, System.String.Empty
            | _                                 ->  false, "Initial assumption in proof must match the assumption from lemma")
            |> (function
                | false, msg    ->  false, msg
                | true, _       ->  match List.tryLast (snd lemma.Proof) with
                                    | Some(Instant(_, c', _))   when c = c' ->  true, System.String.Empty
                                    | _                                     ->  false, "The proofs conclusion must match the conclusion in lemma")

let rules = Map.ofList [
    ("Neg_I",   ([Implication(Variable("0"), Constant(false))],                 Negation(Variable("0"))))
    ("Neg_E",   ([Negation(Variable("0")); Variable("0")],                      Variable("1")))
    ("Con_I",   ([Variable("0"); Variable("1")],                                Conjunction(Variable("0"), Variable("1"))))
    ("Con_E1",  ([Conjunction(Variable("0"), Variable("1"))],                   Variable("0")))
    ("Con_E2",  ([Conjunction(Variable("0"), Variable("1"))],                   Variable("1")))
    ("Dis_I1",  ([Variable("0")],                                               Disjunction(Variable("0"), Variable("1"))))
    ("Dis_I2",  ([Variable("1")],                                               Disjunction(Variable("0"), Variable("1"))))
    ("Dis_E",   ([Disjunction(Variable("0"), Variable("1"))
                  Implication(Variable("0"), Variable("2"))
                  Implication(Variable("1"), Variable("2"))],                   Variable("2")))
    ("Imp_I",   ([Variable("0"); Variable("1")],                                Implication(Variable("0"), Variable("1"))))
    ("Imp_E",   ([Implication(Variable("0"), Variable("1")); Variable("0")],    Variable("1")))
    ("Iff_I",   ([Implication(Variable("0"), Variable("1"))
                  Implication(Variable("1"), Variable("0"))],                   Equivalence(Variable("0"), Variable("1"))))
    ("Iff_E1",  ([Equivalence(Variable("0"), Variable("1"))],                   Implication(Variable("0"), Variable("1"))))
    ("Iff_E2",  ([Equivalence(Variable("0"), Variable("1"))],                   Implication(Variable("1"), Variable("0"))))
]

let tryApply (assumptions, result, rule, ruleset) =
    match Map.tryFind rule ruleset with
    | None          ->  Some($"Rule \"%s{rule}\" does not exist")
    | Some(a', r')  ->  match unify r' result Map.empty with
                        | false, _  ->  Some($"Could not match %s{Formula.ToString result} with %s{Formula.ToString r'}")
                        | true, map ->  match List.tryFind (fun x -> not (Set.exists (fun y -> unify x y map |> fst) assumptions)) a' with
                                        | None      ->  None
                                        | Some(x)   ->  sprintf "Could not apply rule %s: Not all conditions were met\n - Required conditions: [%s]\n - Current assumptions: [%A]\n - Missing %s"
                                                            rule
                                                            (a' |> List.map Formula.ToString |> String.concat "; ")
                                                            (map |> Map.toList |> List.map (fun (k, v) -> $"(%s{k}, %s{Formula.ToString v})") |> String.concat "; ")
                                                            (sprintf "(%s, %s)" (Formula.ToString x) (Map.tryFind (Formula.ToString x) map |> function Some(f) -> Formula.ToString f | _ -> "?"))
                                                        |> Some

type Result =
    | Success of Set<Formula>
    | Fail of string

let rec prove (goal: Formula, (proofRule, statements): Proof, assumptions: Set<Formula>, rules: Map<string, Formula list * Formula>) =
    (Success(assumptions), statements)
    ||> List.fold
        (fun state statement ->
            match state with
            | Fail _        ->  state
            | Success(fs)   ->
                match statement with
                | Assumption(f)         ->  Success(Set.add f fs)
                | Instant(a, f, rule)   ->  match List.tryFind (fun x -> not (Set.contains x fs)) (defaultArg a List.empty) with
                                            | Some(a)   ->  Fail($"The assumption %s{Formula.ToString a} is not in the set of assumptions")
                                            | None      ->  match tryApply(fs, f, rule, rules) with
                                                            | None      ->  Success(Set.add f fs)
                                                            | Some(msg) ->  Fail(msg)
                | Delayed(f, p)         ->  match prove(f, p, fs, rules) with
                                            | Fail(msg) ->  Fail(msg)
                                            | Success _ ->  Success(Set.add f fs))
    |> function
        | Fail(msg)     ->  Fail(msg)
        | Success(fs)   ->  match tryApply(fs, goal, proofRule, rules) with
                            | None      -> Success(Set.add goal fs)
                            | Some(msg) -> Fail(msg)
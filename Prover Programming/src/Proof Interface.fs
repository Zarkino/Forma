module Proof_Interface

open Logic.PL
open Logic.ML

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

let bind statements =
    match List.tryHead statements with
    | Some(Assumption(p))   ->
        match List.tryLast statements with
        | Some(Instant(_, q, _))
        | Some(Delayed(q, _))       -> Ok(Implication(Entity(p), Entity(q)))
        | _                         -> Error("Last statement must be a conclusion")
    | _ -> Error("First statement must be an assumption")

let rules = Map.ofList [
    ("Falsity_E",   ([Entity(Constant(false))],                                         Entity(Variable("0"))))
    ("Truth_I",     ([Entity(Logic.PL.Implication(Constant(false), Constant(false)))],  Entity(Constant(true))))
    ("Neg_I",       ([Implication(Entity(Variable("0")), Entity(Constant(false)))],     Entity(Negation(Variable("0")))))
    ("Neg_E",       ([Entity(Negation(Variable("0"))); Entity(Variable("0"))],          Entity(Variable("1"))))
    ("Con_I",       ([Entity(Variable("0")); Entity(Variable("1"))],                    Entity(Conjunction(Variable("0"), Variable("1")))))
    ("Con_E1",      ([Entity(Conjunction(Variable("0"), Variable("1")))],               Entity(Variable("0"))))
    ("Con_E2",      ([Entity(Conjunction(Variable("0"), Variable("1")))],               Entity(Variable("1"))))
    ("Dis_I1",      ([Entity(Variable("0"))],                                           Entity(Disjunction(Variable("0"), Variable("1")))))
    ("Dis_I2",      ([Entity(Variable("1"))],                                           Entity(Disjunction(Variable("0"), Variable("1")))))
    ("Dis_E",       ([Entity(Disjunction(Variable("0"), Variable("1")))
                      Implication(Entity(Variable("0")), Entity(Variable("2")))
                      Implication(Entity(Variable("1")), Entity(Variable("2")))],       Entity(Variable("2"))))
    ("Imp_I",       ([Implication(Entity(Variable("0")), Entity(Variable("1")))],       Entity(Logic.PL.Implication(Variable("0"), Variable("1")))))
    ("Imp_E",       ([Entity(Logic.PL.Implication(Variable("0"), Variable("1")))
                      Entity(Variable("0"))],                                           Entity(Variable("1"))))
    ("Iff_I",       ([Entity(Logic.PL.Implication(Variable("0"), Variable("1")))
                      Entity(Logic.PL.Implication(Variable("1"), Variable("0")))],      Entity(Equivalence(Variable("0"), Variable("1")))))
    ("Iff_E1",      ([Entity(Equivalence(Variable("0"), Variable("1")))
                      Entity(Variable("0"))],                                           Entity(Variable("1"))))
    ("Iff_E2",      ([Entity(Equivalence(Variable("0"), Variable("1")))
                      Entity(Variable("1"))],                                           Entity(Variable("0"))))
    ("LEM",         ([],                                                                Entity(Disjunction(Variable("0"), Negation(Variable("0"))))))
]

let tryApply (assumptions, result, rule, ruleset) =
    match Map.tryFind rule ruleset with
    | None          ->  Some($"Rule \"%s{rule}\" does not exist")
    | Some(a', r')  ->  match unify r' result Map.empty with
                        | false, _  ->  Some($"Could not match %s{result.ToString()} with %s{r'.ToString()}")
                        | true, map ->  match List.tryFind (fun x -> not (Set.exists (fun y -> unify x y map |> fst) assumptions)) a' with
                                        | None      ->  None
                                        | Some(x)   ->  sprintf "Could not apply rule %s: Not all conditions were met\n - Required conditions: [%s]\n - Current assumptions: [%A]\n - Missing %s"
                                                            rule
                                                            (a' |> List.map (fun x -> x.ToString()) |> String.concat "; ")
                                                            (map |> Map.toList |> List.map (fun (k, v) -> $"(%s{k}, %s{Formula.ToString v})") |> String.concat "; ")
                                                            (sprintf "(%s, %s)" (x.ToString()) (Map.tryFind (x.ToString()) map |> function Some(f) -> Formula.ToString f | _ -> "?"))
                                                        |> Some

let rec prove (goal: Meta, (proofRule, statements): Proof, assumptions: Set<Meta>, rules: Map<string, Meta list * Meta>) =
    (Ok(assumptions), statements)
    ||> List.fold
        (fun state statement ->
            match state with
            | Error _   -> state
            | Ok(fs)    ->
                match statement with
                | Assumption(f)         ->  Ok(Set.add (Entity(f)) fs)
                | Instant(a, f, rule)   ->  match List.tryFind (fun x -> not (Set.contains x fs)) (a |> function Some(v) -> List.map Entity v | None -> List.empty) with
                                            | Some(a)   ->  Error($"The assumption %s{a.ToString()} is not in the set of assumptions")
                                            | None      ->  match tryApply(fs, Entity(f), rule, rules) with
                                                            | None      -> Ok(Set.add (Entity(f)) fs)
                                                            | Some(msg) -> Error(msg)
                | Delayed(f, p)         ->  match prove(Entity(f), p, fs, rules) with
                                            | Error(msg)    -> Error(msg)
                                            | Ok _          -> Ok(Set.add (Entity(f)) fs))
    |> function
        | Error(msg)    ->  Error(msg)
        | Ok(fs)        ->  match bind statements with
                            | Error(msg)    ->  Error(msg)
                            | Ok(value)     ->  match tryApply(Set.add value fs, goal, proofRule, rules) with
                                                | None      -> Ok(Set.add goal fs)
                                                | Some(msg) -> Error(msg)
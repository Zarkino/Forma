module Proof_Interface

open Logic.PL
open Logic.ML

type Method =
    | Trivial
    | This
    | Rule of string

type Proof = Method * Statement list
and Statement =
    | Next
    | Assumption of Meta List
    | Intermediate of Command
    | Conclusion of Command
and Command =
    | Instant of Meta list option * Meta * Method
    | Delayed of Meta * Proof

type Lemma = {
    Name: string option
    Goal: Meta
    Proof: Proof
}
with
    static member ToString lemma = (lemma.Name |> function Some(name) -> $"%s{name}: " | _ -> System.String.Empty) |> (fun s -> $"%s{s}%s{lemma.Goal.ToString()}")
    override this.ToString() = Lemma.ToString this

let rules = Map.ofList [
    "Falsity_E",    Implication(Entity(Constant(false)), Entity(Variable("0")))
    "Truth_I",      Implication(Entity(Logic.PL.Implication(Constant(false), Constant(false))), Entity(Constant(true)))
    "Neg_I",        Implication(Implication(Entity(Variable("0")), Entity(Constant(false))), Entity(Negation(Variable("0"))))
    "Neg_E",        Implication(Entity(Negation(Variable("0"))), Implication(Entity(Variable("0")), Entity(Variable("1"))))
    "Con_I",        Implication(Entity(Variable("0")), Implication(Entity(Variable("1")), Entity(Conjunction(Variable("0"), Variable("1")))))
    "Con_E1",       Implication(Entity(Conjunction(Variable("0"), Variable("1"))), Entity(Variable("0")))
    "Con_E2",       Implication(Entity(Conjunction(Variable("0"), Variable("1"))), Entity(Variable("1")))
    "Dis_I1",       Implication(Entity(Variable("0")), Entity(Disjunction(Variable("0"), Variable("1"))))
    "Dis_I2",       Implication(Entity(Variable("1")), Entity(Disjunction(Variable("0"), Variable("1"))))
    "Dis_E",        Implication(Entity(Disjunction(Variable("0"), Variable("1"))), Implication(Implication(Entity(Variable("0")), Entity(Variable("2"))), Implication(Implication(Entity(Variable("1")), Entity(Variable("2"))), Entity(Variable("2")))))
    "Imp_I",        Implication(Implication(Entity(Variable("0")), Entity(Variable("1"))), Entity(Logic.PL.Implication(Variable("0"), Variable("1"))))
    "Imp_E",        Implication(Entity(Logic.PL.Implication(Variable("0"), Variable("1"))), Implication(Entity(Variable("0")), Entity(Variable("1"))))
    "Iff_I",        Implication(Implication(Entity(Variable("0")), Entity(Variable("1"))), Implication(Implication(Entity(Variable("1")), Entity(Variable("0"))), Entity(Equivalence(Variable("0"), Variable("1")))))
    "Iff_E1",       Implication(Entity(Equivalence(Variable("0"), Variable("1"))), Implication(Entity(Variable("0")), Entity(Variable("1"))))
    "Iff_E2",       Implication(Entity(Equivalence(Variable("0"), Variable("1"))), Implication(Entity(Variable("1")), Entity(Variable("0"))))
    "LEM",          Entity(Disjunction(Variable("0"), Negation(Variable("0"))))
]

let split rule goal =
    let rec f x acc =
        match x with
        | Implication(p, q) ->
            match unify q goal Map.empty with
            | true, map -> Ok(p::acc, map)
            | false, _  -> f q (p::acc)
        | _                 -> Error($"Can only split Meta-implication: %s{x.ToString()}")
    match unify rule goal Map.empty with
    | true, map -> Ok(List.empty, map)
    | false, _  -> f rule List.empty

let rec build list =
    match list with
    | [x]       -> x
    | x::x'::xs -> Implication(x, build (x'::xs))
    | _         -> failwith "Could not build formula"

let tryApply (assumptions, result, method, ruleset) =
    match method with
    | Method.Trivial    -> if Set.contains result assumptions then Ok() else Error($"Could not reach goal %s{result.ToString()} by none")
    | Method.This       ->
        assumptions
        |> Set.exists (fun x -> Logic.ML.split x |> function a', r' when r' = result && List.forall (fun a -> Set.contains a assumptions) a' -> true | _ -> false)
        |> function
            | true  -> Ok()
            | false ->
                match result with
                | Implication(p, p') when p = p'    -> Ok()
                | _                                 -> Error($"Could not achieve goal %s{result.ToString()} by this")
    | Method.Rule(rule) ->
        match Map.tryFind rule ruleset with
        | None          -> Error($"Rule \"%s{rule}\" does not exist")
        | Some(meta)    ->
            match split meta result with
            | Error _       -> Error($"Could not achieve %s{result.ToString()} by rule %s{rule}")
            | Ok(list, map) ->
                match List.tryFind (fun x -> not (Set.exists (fun y -> unify x y map |> fst) assumptions)) list with
                | None      ->  Ok()
                | Some(x)   ->  sprintf "Could not apply rule %s: Not all conditions were met\n - Required conditions: [%s]\n - Current assumptions: [%A]\n - Missing %s"
                                    rule
                                    (list |> List.map (fun x -> x.ToString()) |> String.concat "; ")
                                    (map |> Map.toList |> List.map (fun (k, v) -> $"(%s{k}, %s{Formula.ToString v})") |> String.concat "; ")
                                    (sprintf "(%s, %s)" (x.ToString()) (Map.tryFind (x.ToString()) map |> function Some(f) -> Formula.ToString f | _ -> "?"))
                                |> Error

let rec prove (goal: Meta, (method, statements): Proof, facts: Meta list, rules: Map<string, Meta>) =
    (Ok(facts, List.empty, Set.empty), statements)
    ||> List.fold
        (fun state statement ->
            match state with
            | Error _                           -> state
            | Ok(fs, assumptions, conclusions)  ->
                match statement with
                | Next                  -> Ok(facts, List.empty, conclusions)
                | Assumption(fs')       -> Ok(List.append fs' fs, assumptions@fs', conclusions)
                | Intermediate(command)
                | Conclusion(command)   ->
                    match command with
                    | Instant(a, f, rule)   ->
                        match a with
                        | None      -> Ok(fs)
                        | Some(a')  ->
                            match List.tryFind (fun x -> not (List.contains x fs)) a' with
                            | Some(v)   -> Error($"The assumption %s{v.ToString()} is not in the set of assumptions")
                            | None      -> Ok(a')
                        |> function
                            | Error(msg)    -> Error(msg)
                            | Ok(fs')       ->
                                match tryApply(Set fs', f, rule, rules) with
                                | Error(msg)    -> Error(msg)
                                | Ok()          -> Ok(f)
                    | Delayed(f, p)         ->
                        match prove(f, p, fs, rules) with
                        | Error(msg)    -> Error(msg)
                        | Ok _          -> Ok(f)
                    |> function
                        | Error(msg)    -> Error(msg)
                        | Ok(f)         ->
                            match statement with
                            | Intermediate _    -> Ok(f::fs, assumptions, conclusions)
                            | Conclusion _      -> Ok(fs, assumptions, Set.add (build (assumptions@[f])) conclusions)
                            | _                 -> Error("Something went wrong"))
    |> function
        | Error(msg)            -> Error(msg)
        | Ok(_, _, conclusions) -> tryApply(conclusions, goal, method, rules)
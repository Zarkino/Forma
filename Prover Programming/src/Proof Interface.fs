module Proof_Interface

open Logic.PL
open Logic.ML

type Method =
    | Trivial
    | This
    | Rule of string

type Proof = Method * Statement list
and Statement =
    | Assumption of Meta
    | Intermediate of Command
    | Conclusion of Command
and Command =
    | Instant of Meta list option * Meta * Method
    | Delayed of Meta * Proof
    member this.Goal =
        match this with
        | Instant(_, g, _)
        | Delayed(g, _)     -> g

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
    "Neg_E",        Implication(Entity(Negation(Variable("0"))),Implication(Entity(Variable("0")), Entity(Variable("1"))))
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

let rec insert goal = function Implication(p, q) -> Implication(p, insert goal q) | q -> Implication(goal, q)

let tryApply (assumptions, result, method, ruleset) =
    match method with
    | Method.Trivial    -> if Set.contains result assumptions then Ok() else Error($"Could not reach goal %s{result.ToString()} by none")
    | Method.This       ->
        assumptions
        |> Set.exists
               (fun x ->
                   match Logic.ML.split x with
                   | a', r' when r' = result && List.forall (fun a -> Set.contains a assumptions) a'   -> true
                   | _                                                                                 -> false)
        |> function
        | true  -> Ok()
        | false ->
            match result with
            | Implication(p, p') when p = p'    -> Ok()
            | _                                 -> Error($"Could not achieve goal %s{result.ToString()} by this")
    | Method.Rule(rule) ->
        match Map.tryFind rule ruleset with
        | None          -> Error($"Rule \"%s{rule}\" does not exist")
        | Some(meta)  ->
            match split meta result with
            | Error(msg)    -> Error(msg)
            | Ok(list, map) -> 
                match List.tryFind (fun x -> not (Set.exists (fun y -> unify x y map |> fst) assumptions)) list with
                | None      ->  Ok()
                | Some(x)   ->  sprintf "Could not apply rule %s: Not all conditions were met\n - Required conditions: [%s]\n - Current assumptions: [%A]\n - Missing %s"
                                    rule
                                    (list |> List.map (fun x -> x.ToString()) |> String.concat "; ")
                                    (map |> Map.toList |> List.map (fun (k, v) -> $"(%s{k}, %s{Formula.ToString v})") |> String.concat "; ")
                                    (sprintf "(%s, %s)" (x.ToString()) (Map.tryFind (x.ToString()) map |> function Some(f) -> Formula.ToString f | _ -> "?"))
                                |> Error

let rec prove (goal: Meta, (method, statements): Proof, assumptions: Set<Meta>, rules: Map<string, Meta>) =
    (Ok(assumptions, None), statements)
    ||> List.fold
        (fun state statement ->
            match state with
            | Error _           -> state
            | Ok(fs, subgoal)   ->
                match statement with
                | Assumption(f)         -> Ok(Set.add f fs, Some(subgoal |> function None -> f | Some(f') -> insert f' f))
                | Intermediate(command)
                | Conclusion(command)   ->
                    match command with
                    | Instant(a, f, rule)   ->
                        match a with
                        | None      -> Ok(fs)
                        | Some(a')  ->
                            match List.tryFind (fun x -> not (Set.contains x fs)) a' with
                            | Some(v)   -> Error($"The assumption %s{v.ToString()} is not in the set of assumptions")
                            | None      -> Ok(Set a')
                        |> function
                            | Error(msg)    -> Error(msg)
                            | Ok(fs')        ->
                                match tryApply(fs', f, rule, rules) with
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
                            | Assumption _      -> Error("Something went wrong")
                            | Intermediate _    -> Ok(Set.add f fs, subgoal)
                            | Conclusion _      ->
                                match subgoal with
                                | None      -> Ok(Set.add f fs, subgoal)
                                | Some(f')  -> Ok(Set.union (Set [f; Implication(f', f)]) fs, subgoal))
    |> function
        | Error(msg)    ->  Error(msg)
        | Ok(fs, _)     ->  match tryApply(fs, goal, method, rules) with
                            | Ok()          -> Ok(Set.add goal fs)
                            | Error(msg)    -> Error(msg)
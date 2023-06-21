module Proof_Interface

open Logic.PL
open Logic.ML

type Method =
    | Trivial
    | This
    | Rule of string
with
    static member ToString method =
        match method with
        | Trivial       -> "none"
        | This          -> "this"
        | Rule(name)    -> $"(rule %s{name})"
    override this.ToString() = Method.ToString this

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

let toPlaintext(lemma: Lemma) =
    let rec indent n = String.replicate n "\t"
    
    let rec outer((method, statements): Proof, depth: int) =
        let indentation = indent depth
        sprintf "%sproof %s {\n%s\n%s}"
            indentation
            (method.ToString())
            (List.map (fun statement -> inner(statement, depth+1)) statements |> String.concat "\n")
            indentation
    and inner(statement: Statement, depth: int) =
            let indentation = indent depth
            match statement with
            | Next                  -> sprintf "%snext" (indent(depth-1))
            | Assumption(fs)        -> sprintf "%sassume %s" indentation (List.map Meta.ToString fs |> String.concat " and ")
            | Intermediate(command)
            | Conclusion(command)   ->
                let keyword = statement |> function Intermediate _ -> "have" | _ -> "show"
                match command with
                | Instant(fs, g, m) ->
                    sprintf "%s%s%s %s by %s"
                        indentation
                        (fs |> function Some(fs') -> sprintf "from %s " (List.map Meta.ToString fs' |> String.concat " and ") | None -> System.String.Empty)
                        keyword
                        (g.ToString())
                        (m.ToString())
                | Delayed(g, p)     ->
                    sprintf "%s%s %s\n%s"
                        indentation
                        keyword
                        (g.ToString())
                        (outer(p, depth))
    
    $"lemma %s{lemma.ToString()}\n%s{outer(lemma.Proof, 0)}"

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

let tryApply (fs, result, method, ruleset) =
    match method with
    | Method.Trivial    -> if Set.contains result fs then Ok() else Error($"Could not achieve %s{result.ToString()} by none")
    | Method.This       ->
        fs
        |> Set.exists (fun x -> Logic.ML.split x |> function a', r' when r' = result && List.forall (fun a -> Set.contains a fs) a' -> true | _ -> false)
        |> function
            | true  -> Ok()
            | false ->
                match result with
                | Implication(p, p') when p = p'    -> Ok()
                | _                                 -> Error($"Could not achieve %s{result.ToString()} by this")
    | Method.Rule(rule) ->
        match Map.tryFind rule ruleset with
        | None          -> Error($"Rule \"%s{rule}\" does not exist")
        | Some(meta)    ->
            match split meta result with
            | Error _       -> Error($"Could not achieve %s{result.ToString()} by rule %s{rule}")
            | Ok(list, map) ->
                match List.tryFind (fun x -> not (Set.exists (fun y -> unify x y map |> fst) fs)) list with
                | None      ->  Ok()
                | Some(x)   ->  sprintf "Could not apply rule %s: Not all conditions were met\n - Required conditions: [%s]\n - Current assumptions: [%A]\n - Missing %s"
                                    rule
                                    (list |> List.map (fun x -> x.ToString()) |> String.concat "; ")
                                    (map |> Map.toList |> List.map (fun (k, v) -> $"(%s{k}, %s{Formula.ToString v})") |> String.concat "; ")
                                    (sprintf "(%s, %s)" (x.ToString()) (Map.tryFind (x.ToString()) map |> function Some(f) -> Formula.ToString f | _ -> "?"))
                                |> Error

let rec prove (goal: Meta, (method, statements): Proof, facts: Set<Meta>, rules: Map<string, Meta>) =
    (Ok(facts, List.empty, Set.empty), statements)
    ||> List.fold
        (fun state statement ->
            match state with
            | Error _                           -> state
            | Ok(fs, assumptions, conclusions)  ->
                match statement with
                | Next                  -> Ok(facts, List.empty, conclusions)
                | Assumption(fs')       -> Ok(Set.union (Set fs') fs, assumptions@fs', conclusions)
                | Intermediate(command)
                | Conclusion(command)   ->
                    match command with
                    | Instant(a, f, rule)   ->
                        match a with
                        | None      -> Ok(fs)
                        | Some(a')  ->
                            match List.tryFind (fun x -> not (Set.contains x fs)) a' with
                            | Some(x)   -> Error($"The assumption %s{x.ToString()} is not in the set of assumptions")
                            | None      -> Ok(Set a')
                        |> function
                            | Error(msg)    -> Error(msg)
                            | Ok(fs')       ->
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
                            | Intermediate _    -> Ok(Set.add f fs, assumptions, conclusions)
                            | _                 -> Ok(fs, assumptions, Set.add (build (assumptions@[f])) conclusions))
    |> function
        | Error(msg)            -> Error(msg)
        | Ok(_, _, conclusions) -> tryApply(conclusions, goal, method, rules)
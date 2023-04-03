module Propositional_Logic

type Formula =
    | Variable of string
    | Negation of Formula
    | Conjunction of Formula * Formula
    | Disjunction of Formula * Formula
    | Implication of Formula * Formula
    | Equivalence of Formula * Formula
    static member Extract formula =
        let rec f x =
            match x with
            | Variable(p)       -> seq { p }
            | Negation(p)       -> f p
            | Conjunction(p, q)
            | Disjunction(p, q)
            | Implication(p, q)
            | Equivalence(p, q) -> seq { yield! f p; yield! f q }
        f formula
    static member Replace (formula, old_val, new_val) =
        let rec f (x, o, n) =
            match x with
            | x when x = o  -> n
            | Variable _        -> x
            | Negation(p)       -> Negation(f(p, o, n))
            | Conjunction(p, q) -> Conjunction(f(p, o, n), f(q, o, n))
            | Disjunction(p, q) -> Disjunction(f(p, o, n), f(q, o, n))
            | Implication(p, q) -> Implication(f(p, o, n), f(q, o, n))
            | Equivalence(p, q) -> Equivalence(f(p, o, n), f(q, o, n))
        f (formula, old_val, new_val)
    static member ReplaceVar (formula, old_val, new_val) =
        let rec f (x, o, n) =
            match x with
            | Variable(p) when p = o    -> Variable(n)
            | Variable _                -> x
            | Negation(p)               -> Negation(f(p, o, n))
            | Conjunction(p, q)         -> Conjunction(f(p, o, n), f(q, o, n))
            | Disjunction(p, q)         -> Disjunction(f(p, o, n), f(q, o, n))
            | Implication(p, q)         -> Implication(f(p, o, n), f(q, o, n))
            | Equivalence(p, q)         -> Equivalence(f(p, o, n), f(q, o, n))
        f (formula, old_val, new_val)
    static member IsLiteral formula =
        match formula with
        | Variable _
        | Negation(Variable _)  -> true
        | _                     -> false
    member this.IsLiteral() = Formula.IsLiteral this
    static member GetOperator formula =
        match formula with
        | Variable _    -> ""
        | Negation _    -> "¬"
        | Conjunction _ -> "∧"
        | Disjunction _ -> "∨"
        | Implication _ -> "→"
        | Equivalence _ -> "↔"
    member this.GetOperator() = Formula.GetOperator this
    static member ToString formula =
        let rec f x =
            match x with
            | Variable(p)       ->  p
            | Negation(p)       ->  match p with
                                    | Variable(p')  -> $"¬%s{p'}"
                                    | Negation(p')  -> $"¬¬(%s{f p'})"
                                    | p'            -> $"¬(%s{f p'})"
            | Conjunction(p, q)
            | Disjunction(p, q)
            | Implication(p, q)
            | Equivalence(p, q) ->  let p' = if Formula.IsLiteral p then $"%s{f p}" else $"(%s{f p})"
                                    let q' = if Formula.IsLiteral q then $"%s{f q}" else $"(%s{f q})"
                                    $"%s{p'} %s{Formula.GetOperator x} %s{q'}"
        f formula
    override this.ToString () = Formula.ToString this

let f formula = (formula, Formula.Extract formula |> Seq.indexed) ||> Seq.fold (fun x (i, v) -> Formula.ReplaceVar (x, v, $"%i{i}"))
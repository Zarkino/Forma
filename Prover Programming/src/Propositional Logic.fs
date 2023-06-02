namespace Logic

module PL =
    type Formula =
        | Constant of bool
        | Variable of string
        | Negation of Formula
        | Conjunction of Formula * Formula
        | Disjunction of Formula * Formula
        | Implication of Formula * Formula
        | Equivalence of Formula * Formula
        static member Extract formula =
            let rec f x =
                match x with
                | Constant _        -> Seq.empty
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
                | x when x = o      -> n
                | Constant _
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
                | Constant _
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
            | Constant _
            | Variable _    -> ""
            | Negation _    -> "¬"
            | Conjunction _ -> "∧"
            | Disjunction _ -> "∨"
            | Implication _ -> "⟶"
            | Equivalence _ -> "⟷"
        member this.GetOperator() = Formula.GetOperator this
        static member ToString formula =
            let rec f x =
                match x with
                | Constant(b)       ->  if b then "⊤" else "⊥"
                | Variable(p)       ->  p
                | Negation(p)       ->  match p with
                                        | Constant _    -> $"¬%s{f p}"
                                        | Variable(p')  -> $"¬%s{p'}"
                                        | Negation(p')  -> $"¬¬(%s{f p'})"
                                        | p'            -> $"¬(%s{f p'})"
                | Conjunction(p, q)
                | Disjunction(p, q)
                | Implication(p, q)
                | Equivalence(p, q) ->  $"%s{par p} %s{Formula.GetOperator x} %s{par q}"
            and par x =
                match x with
                | Constant _
                | Negation(Constant _)
                | Variable _
                | Negation(Variable _)  -> $"%s{f x}"
                | _                     -> $"(%s{f x})"
            f formula
        override this.ToString () = Formula.ToString this

    let standardize formula = (formula, Formula.Extract formula |> Seq.indexed) ||> Seq.fold (fun x (i, v) -> Formula.ReplaceVar (x, v, $"%i{i}"))

    /// Splits formula into assumptions and conclusion
    let split formula =
        let rec f x acc =
            match x with
            | Implication(p, q) -> f q (g p@acc)
            | _                 -> acc, x
        and g x =
            match x with
            | Implication(p, q) -> g p@g q
            | _                 -> [x]
        f formula []

    /// Separates formula into assumption and conclusion
    let separate = function
        | Implication(p, q)
        | Equivalence(p, q) -> Some(p, q)
        | _                 -> None

    let rec unify x y map =
        match x, y with
        | Variable(k), p                            ->  match Map.tryFind k map with
                                                        | Some(v)   -> p = v, map
                                                        | None      -> true, Map.add k p map
        | Constant(b), Constant(b')                 ->  b = b', map
        | Negation(p), Negation(p')                 ->  unify p p' map
        | Conjunction(p, q), Conjunction(p', q')
        | Disjunction(p, q), Disjunction(p', q')
        | Implication(p, q), Implication(p', q')
        | Equivalence(p, q), Equivalence(p', q')    ->  match unify p p' map with
                                                        | false, _  -> false, map
                                                        | true, map -> unify q q' map
        | _, _                                      ->  false, map
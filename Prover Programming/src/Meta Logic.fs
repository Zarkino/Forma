module Meta_Logic

open Propositional_Logic

type Meta =
    | Formula of Formula
    | Implication of Meta * Meta
    | Equality of Meta * Meta
    | Universal of string * Meta
    static member ToString formula =
        let rec f x =
            match x with
            | Formula(f)        -> Formula.ToString f
            | Implication(p, q) -> $"(%s{f p}) ⟹ (%s{f q})"
            | Equality(x, y)    -> $"(%s{f x}) ≡ (%s{f y})"
            | Universal(x, m)   -> $"⋀%s{x}. %s{f m}"
        f formula
    override this.ToString () = Meta.ToString this
namespace Logic

module ML =
    type Meta =
        | Entity of PL.Formula
        | Implication of Meta * Meta
        | Equality of Meta * Meta
        | Universal of string * Meta
        static member ToString meta =
            let rec f x =
                match x with
                | Entity(p)         -> PL.Formula.ToString p
                | Implication(p, q) -> $"(%s{f p}) ⟹ (%s{f q})"
                | Equality(x, y)    -> $"(%s{f x}) ≡ (%s{f y})"
                | Universal(x, m)   -> $"⋀%s{x}. %s{f m}"
            f meta
        override this.ToString() = Meta.ToString this
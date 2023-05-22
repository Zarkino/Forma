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
                | Implication(p, q) -> $"%s{f p} ⟹ %s{f q}"
                | Equality(x, y)    -> $"%s{f x} ≡ %s{f y}"
                | Universal(x, m)   -> $"⋀%s{x}. %s{f m}"
            f meta
        override this.ToString() = Meta.ToString this
    
    let rec unify x y map =
        match x, y with
        | Entity(e), Entity(e')                     ->  PL.unify e e' map
        | Implication(p, q), Implication(p', q')    ->  match unify p p' map with
                                                        | false, _  -> false, map
                                                        | true, map -> unify q q' map
        | Equality(x, y), Equality(x', y')          ->  match unify x x' map with
                                                        | false, _  -> false, map
                                                        | true, map -> unify y y' map
        | Universal(x, m), Universal(x', m')        ->  if x = x' then unify m m' map
                                                        else false, map
        | _, _                                      ->  false, map
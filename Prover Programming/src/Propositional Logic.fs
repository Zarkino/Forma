module Propositional_Logic

type Formula =
    | Variable of string
    | Negation of Formula
    | Conjunction of Formula * Formula
    | Disjunction of Formula * Formula
    | Implication of Formula * Formula
    | Equivalence of Formula * Formula
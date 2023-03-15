module Proof_Interface

open Propositional_Logic

type Proof = {
    Statements: Statement list
}
and Statement =
    | Assumption of Formula
    | Intermediate of Formula
    | Conclusion of Formula
    | Subproof of Proof

type Lemma = {
    Identifier: Formula
    Proof: Proof
}
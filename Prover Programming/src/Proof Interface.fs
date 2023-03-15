module Proof_Interface

type Proposition = string

type Proof = {
    Statements: Statement
    Conclusion: string
}
and Statement =
    | Assumption of string
    | Intermediate of string
    | Subproof of Proof

type Lemma = {
    Identifier: Proposition
    Proof: Proof
}
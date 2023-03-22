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

let mutable rules = [
    ("Con_I",   [Variable("0"); Variable("1")],                                                         [Conjunction(Variable("0"), Variable("1"))])
    ("Con_E1",  [Conjunction(Variable("0"), Variable("1"))],                                            [Variable("0")])
    ("Con_E2",  [Conjunction(Variable("0"), Variable("1"))],                                            [Variable("1")])
    ("Dis_I1",  [Variable("0")],                                                                        [Disjunction(Variable("0"), Variable("1"))])
    ("Dis_I2",  [Variable("1")],                                                                        [Disjunction(Variable("0"), Variable("1"))])
    ("Dis_E1",  [Disjunction(Variable("0"), Variable("1"))],                                            [Variable("0")])
    ("Dis_E2",  [Disjunction(Variable("0"), Variable("1"))],                                            [Variable("1")])
    //("Imp_I",   [Variable("0"); Variable("1")],                                                         [Implication(Variable("0"), Variable("1"))])
    ("Imp_E",   [Implication(Variable("0"), Variable("1")); Variable("0")],                             [Variable("1")])
    ("MT",      [Implication(Variable("0"), Variable("1")); Negation(Variable("1"))],                   [Negation(Variable("0"))])
    ("Eqv_I",   [Implication(Variable("0"), Variable("1")); Implication(Variable("1"), Variable("0"))], [Equivalence(Variable("0"), Variable("1"))])
    ("Eqv_E1",  [Equivalence(Variable("0"), Variable("1"))],                                            [Implication(Variable("0"), Variable("1"))])
    ("Eqv_E2",  [Equivalence(Variable("0"), Variable("1"))],                                            [Implication(Variable("1"), Variable("0"))])
    ("DNe_I",   [Variable("0")],                                                                        [Negation(Negation(Variable("0")))])
    ("DNe_E",   [Negation(Negation(Variable("0")))],                                                    [Variable("0")])
]
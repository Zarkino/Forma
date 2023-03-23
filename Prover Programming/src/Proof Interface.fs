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

// Most of the following simply inspired by https://en.wikipedia.org/wiki/List_of_rules_of_inference
let mutable rules = [
    ("Con_I",   [Variable("0"); Variable("1")],                    [Conjunction(Variable("0"), Variable("1"))]) // Conjunction Introduction
    ("Con_E1",  [Conjunction(Variable("0"), Variable("1"))],       [Variable("0")])                             // Conjunction Elimination (right)
    ("Con_E2",  [Conjunction(Variable("0"), Variable("1"))],       [Variable("1")])                             // Conjunction Elimination (left)
    ("Dis_I1",  [Variable("0")],                                   [Disjunction(Variable("0"), Variable("1"))]) // Disjunction Introduction (right)
    ("Dis_I2",  [Variable("1")],                                   [Disjunction(Variable("0"), Variable("1"))]) // Disjunction Introduction (left)
    ("Dis_E1",  [Disjunction(Variable("0"), Variable("1"))
                 Negation(Variable("0"))],                         [Variable("1")])                             // Disjunctive Syllogism (left)
    ("Dis_E2",  [Disjunction(Variable("0"), Variable("1"))
                 Negation(Variable("1"))],                         [Variable("0")])                             // Disjunctive Syllogism (right)
    ("Dis_E3",  [Implication(Variable("0"), Variable("1"))
                 Implication(Variable("2"), Variable("1"))
                 Disjunction(Variable("0"), Variable("2"))],       [Variable("1")])                             // Disjunction Elimination
    ("Dis_E4",  [Implication(Variable("0"), Variable("1"))
                 Implication(Variable("2"), Variable("3"))
                 Disjunction(Variable("0"), Variable("2"))],       [Disjunction(Variable("1"),Variable("3"))])  // Constructive Dilemma
    ("Imp_E1",  [Implication(Variable("0"), Variable("1"))
                 Variable("0")],                                   [Variable("1")])                             // Modus Ponens (Elimination)
    ("Imp_E2",  [Implication(Variable("0"), Variable("1"))
                 Negation(Variable("1"))],                         [Negation(Variable("0"))])                   // Modus Tollens
    ("Eqv_I",   [Implication(Variable("0"), Variable("1"))
                 Implication(Variable("1"), Variable("0"))],       [Equivalence(Variable("0"), Variable("1"))]) //
    ("Eqv_E1",  [Equivalence(Variable("0"), Variable("1"))],       [Implication(Variable("0"), Variable("1"))]) //
    ("Eqv_E2",  [Equivalence(Variable("0"), Variable("1"))],       [Implication(Variable("1"), Variable("0"))]) //
    ("DNe_I",   [Variable("0")],                                   [Negation(Negation(Variable("0")))])         //
    ("DNe_E",   [Negation(Negation(Variable("0")))],               [Variable("0")])                             //
    
    //Implications are introduced via assumptions/assertions
    //("Imp_I",   [Variable("0"); Variable("1")], [Implication(Variable("0"), Variable("1"))])
]
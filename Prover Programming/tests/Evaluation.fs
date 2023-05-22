module Tests.Evaluation

open Fable.Mocha

open Parsec
open Proof_Interface
open Logic.PL

let evaluating_Proofs =
    let evaluate goal proof =
        match runString Language.Proof.proof () proof with
            | Error _       ->  Test.failtest "Could not be parsed"
            | Ok(v, _, _)   ->  match prove(goal, v, Set.empty, rules) with
                                | Ok _  -> Expect.pass()
                                | _     -> Test.failtest "Evaluation failed"

    testList "Basic Proof Evaluation Tests" [
        test "Prove Proof: Conjunction Swap" {
            let goal = Implication(Conjunction(Variable("P"), Variable("Q")), Conjunction(Variable("Q"), Variable("P")))
            let proof =
                "proof (rule Imp_I) {
					assume P & Q
					from P & Q have P by Con_E1
					from P & Q have Q by Con_E2
					from P and Q show Q & P by Con_I
				}"
            evaluate goal proof
        }
        test "Prove Proof: Double Negation" {
            let goal = Implication(Variable("P"), Negation(Negation(Variable("P"))))
            let proof =
                "proof (rule Imp_I) {
					assume P
					show ~~P
					proof (rule Falsity_E) {
						assume ~P
						from ~P and P show F by Neg_E
					}
				}"
            evaluate goal proof
        }
        test "Prove Proof: Bi-implication Swap" {
            let goal = Equivalence(Equivalence(Variable("P"), Variable("Q")), Equivalence(Variable("Q"), Variable("P")))
            let proof =
                "proof (rule Iff_I) {
					show (P <-> Q) -> (Q <-> P)
					proof (rule Imp_I) {
						assume P <-> Q
						from P <-> Q have P -> Q by Iff_E1
						from P <-> Q have Q -> P by Iff_E2
						from P -> Q and Q -> P show Q <-> P by Iff_I
					}
					show (Q <-> P) -> (P <-> Q)
					proof (rule Imp_I) {
						assume Q <-> P
						from Q <-> P have Q -> P by Iff_E1
						from Q <-> P have P -> Q by Iff_E2
						from Q -> P and P -> Q show P <-> Q by Iff_I
					}
				}"
            evaluate goal proof
        }
    ]
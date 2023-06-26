module Tests.Evaluation

open Fable.Mocha

open Parsec
open Proof_Interface
open Logic.PL

let evaluating_Proofs =
    let evaluate goal proof =
        match runString Language.Proof.proof () proof with
            | Error _       ->  Test.failtest "Could not be parsed"
            | Ok(v, _, _)   ->  match prove(Logic.ML.Entity(goal), v, Set.empty, rules) with
                                | Ok _  -> Expect.pass()
                                | _     -> Test.failtest "Evaluation failed"

    testList "Basic Proof Evaluation Tests" [
        test "Prove Proof: Conjunction Swap" {
            let goal = Implication(Conjunction(Variable("P"), Variable("Q")), Conjunction(Variable("Q"), Variable("P")))
            let proof =
                "proof (rule Imp_I) {
					assume P & Q
					from P & Q have P by (rule Con_E1)
					from P & Q have Q by (rule Con_E2)
					from P and Q show Q & P by (rule Con_I)
				}"
            evaluate goal proof
        }
        test "Prove Proof: Double Negation" {
            let goal = Implication(Variable("P"), Negation(Negation(Variable("P"))))
            let proof =
                "proof (rule Imp_I) {
					assume P
					show ~~P
					proof (rule Neg_I) {
						assume ~P
						from ~P and P show F by (rule Neg_E)
					}
				}"
            evaluate goal proof
        }
        test "Prove Proof: Bi-implication Swap" {
            let goal = Equivalence(Equivalence(Variable("P"), Variable("Q")), Equivalence(Variable("Q"), Variable("P")))
            let proof =
                "proof (rule Iff_I) {
                    assume P <--> Q
                    show Q <--> P
                    proof (rule Iff_I) {
                        assume Q
                        show P by (rule Iff_E2)
                    next
                        assume P
                        show Q by (rule Iff_E1)
                    }
                next
                    assume Q <--> P
                    show P <--> Q
                    proof (rule Iff_I) {
                        assume P
                        show Q by (rule Iff_E2)
                    next
                        assume Q
                        show P by (rule Iff_E1)
                    }
                }"
            evaluate goal proof
        }
    ]
module Tests.Evaluation

open Fable.Mocha

open Language.Parser
open Proof_Interface

let evaluating_Proofs =
    testList "Basic Proof Evaluation Tests" [
        let error_msg = "Evaluation failed"
        test "Prove Proof: Empty" {
            match parse_proof "proof {}" with
            | None, _           -> Test.failtest "Could not be parsed"
            | Some(proof), _    -> match prove(proof, Set.empty, Map.empty) with
                                   | Success _  -> Expect.pass()
                                   | _          -> Test.failtest error_msg
        }
        test "Prove Proof: Conjunction Swap" {
            match parse_proof "proof {\n\tassume P & Q\n\thave P by Con_E1\n\thave Q by Con_E2\n\tshow Q & P by Con_I\n}" with
            | None, _           -> Test.failtest "Could not be parsed"
            | Some(proof), _    -> match prove(proof, Set.empty, rules) with
                                   | Success _  -> Expect.pass()
                                   | _          -> Test.failtest error_msg
        }
        test "Prove Proof: Double Negation" {
            match parse_proof "proof {\n\tassume P\n\tproof {\n\t\tassume ~P\n\t\tshow ~P -> F by Neg_E\n\t}\n\tshow ~~P by Neg_I\n}" with
            | None, _           -> Test.failtest "Could not be parsed"
            | Some(proof), _    -> match prove(proof, Set.empty, rules) with
                                   | Success _  -> Expect.pass()
                                   | _          -> Test.failtest error_msg
        }
        test "Prove Proof: Bi-implication Swap" {
            match parse_proof "proof {\n\tassume P <-> Q\n\tproof {\n\t\tassume Q\n\t\tshow Q -> P by Iff_E2\n\t}\n\tproof {\n\t\tassume P\n\t\tshow P -> Q by Iff_E1\n\t}\n\tshow Q <-> P by Iff_I\n}" with
            | None, _           -> Test.failtest "Could not be parsed"
            | Some(proof), _    -> match prove(proof, Set.empty, rules) with
                                   | Success _  -> Expect.pass()
                                   | _          -> Test.failtest error_msg
        }
    ]